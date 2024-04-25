namespace Modules.Catalog.Presentation.Web.Client.Pages;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using Common.Presentation.Web.Client.Components;
using Common.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Modules.Catalog.Presentation.Web.Client.Components;
using MudBlazor;

public partial class BrandsPage
{
    private List<BrandModel> models = new();
    private BrandModel brand = new();
    private string searchString = string.Empty;

    private ClaimsPrincipal user;
    private bool canCreate;
    private bool canEdit;
    private bool canDelete;
    private bool canExport;
    private bool canSearch;
    private bool canImport;
    private bool loaded;
    private bool includeDeleted;

    [CascadingParameter]
    public HubConnection HubConnection { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        this.user = (await this.AuthState).User;
        this.canCreate = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Brands.Create)).Succeeded;
        this.canEdit = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Brands.Edit)).Succeeded;
        this.canDelete = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Brands.Delete)).Succeeded;
        this.canExport = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Brands.Export)).Succeeded;
        this.canSearch = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Brands.Search)).Succeeded;
        this.canImport = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Brands.Import)).Succeeded;

        await this.LoadData();

        this.HubConnection = this.HubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: really needed?, as it is already injected as a parameter
        await this.HubConnection.Start();
        this.loaded = true;
    }

    private async Task LoadData()
    {
        var response = await this.apiClient.Catalog_BrandGetAllAsync(null, null, null, null, includeDeleted: this.includeDeleted); // don't pass page/size as all data needs to be retrieved (=client side paging)
        if (response.IsSuccess)
        {
            this.models = response.Value.ToList();
        }
        else
        {
            response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
        }
    }

    private async Task Delete(Guid id)
    {
        string deleteContent = this.localizer["Delete Content"];
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = this.dialogService.Show<DeleteConfirmation>(this.localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await this.apiClient.Catalog_BrandDeleteAsync(id.ToString());
            if (response.IsSuccess)
            {
                await this.Reset();
                if (this.HubConnection.State == HubConnectionState.Connected)
                {
                    await this.HubConnection.SendAsync(SignalRHubConstants.SendUpdateDashboard);
                }

                this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
            }
            else
            {
                await this.Reset();
                response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
            }
        }
    }

    private bool Search(BrandModel brand)
    {
        if (string.IsNullOrWhiteSpace(this.searchString))
        {
            return true;
        }

        if (brand.Name?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return brand.Description?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true;
    }

    private async Task Reset()
    {
        this.brand = new();
        await this.LoadData();
    }

    private async Task ToggleDeleted(bool includeDeleted)
    {
        this.includeDeleted = includeDeleted;
        await this.LoadData();
    }

    private async Task ExportToExcel()
    {
        var response = await this.apiClient.Catalog_BrandExportAsync(this.searchString);
        if (response.IsSuccess)
        {
            await this.jsRuntime.InvokeVoidAsync("Download", new
            {
                ByteArray = response.Value,
                FileName = $"brands_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            });
            this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
        }
        else
        {
            response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
        }
    }

    private async Task InvokeModal(Guid? id = null)
    {
        var parameters = new DialogParameters();
        if (id != null)
        {
            this.brand = this.models.Find(c => c.Id == id);
            if (this.brand != null)
            {
                parameters.Add(nameof(AddEditBrandModal.Model), this.brand);
            }
        }

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = this.dialogService.Show<AddEditBrandModal>(id == null ? this.localizer["Create"] : this.localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await this.Reset();
        }
    }

    private async Task InvokeImportModal()
    {
        var parameters = new DialogParameters
        {
            { nameof(ImportExcelModal.ModelName), this.localizer["Brands"].ToString() }
        };
        Func<UploadRequest, Task<Result<int>>> importFunc = this.ImportExcel;
        parameters.Add(nameof(ImportExcelModal.OnSaved), importFunc);
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
        };
        var dialog = this.dialogService.Show<ImportExcelModal>(this.localizer["Import"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await this.Reset();
        }
    }

    private async Task<Result<int>> ImportExcel(UploadRequest request)
    {
        var result = await this.apiClient.Catalog_BrandImportAsync(
            new Client.UploadRequestModel { Data = request.Data, FileName = request.FileName, Extension = request.Extension, Type = request.Type });

        return new Result<int>()
        {
            IsSuccess = result.IsSuccess,
            //Messages = result.Messages,
            Value = result.Value
        };
    }
}