namespace Modules.Catalog.Presentation.Web.Client.Pages;

using System;
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

public partial class ProductsPage
{
    private IEnumerable<ProductModel> models;
    private MudTable<ProductModel> table;
    private int totalItems;
    private int currentPage;
    private string searchString = string.Empty;

    private ClaimsPrincipal user;
    private bool canCreate;
    private bool canEdit;
    private bool canDelete;
    private bool canExport;
    private bool canSearch;
    private bool canImport;
    private bool loaded;

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
        this.canCreate = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Products.Create)).Succeeded;
        this.canEdit = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Products.Edit)).Succeeded;
        this.canDelete = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Products.Delete)).Succeeded;
        this.canExport = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Products.Export)).Succeeded;
        this.canSearch = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Products.Search)).Succeeded;
        this.canImport = (await this.AuthService.AuthorizeAsync(this.user, CatalogPermissions.Products.Import)).Succeeded;

        this.HubConnection = this.HubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: really needed?, as it is already injected as a parameter
        await this.HubConnection.Start();
        this.loaded = true;
    }

    private async Task<TableData<ProductModel>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(this.searchString))
        {
            state.Page = 0;
        }

        await this.LoadData(state.Page, state.PageSize, state);
        return new TableData<ProductModel> { TotalItems = this.totalItems, Items = this.models };
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state)
    {
        string[] orderings = null;
        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
        }

        //var request = new GetAllPagedProductsRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
        var response = await this.apiClient.Catalog_ProductGetAllAsync(pageNumber + 1, pageSize, this.searchString, orderings.ToString(",")); //await ProductManager.GetProductsAsync(request);
        if (response.IsSuccess)
        {
            this.totalItems = response.TotalCount.To<int>();
            this.currentPage = response.CurrentPage;
            this.models = response.Value;
        }
        else
        {
            response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
        }
    }

    private void Search(string text)
    {
        this.searchString = text;
        this.table.ReloadServerData();
    }

    private async Task Delete(Guid id)
    {
        string deleteContent = this.localizer["Delete Content"];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = this.dialogService.Show<DeleteConfirmation>(this.localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            try
            {
                var response = await this.apiClient.Catalog_ProductDeleteAsync(id.ToString());
                if (response.IsSuccess)
                {
                    this.Search(string.Empty);
                    if (this.HubConnection.State == HubConnectionState.Connected)
                    {
                        await this.HubConnection.SendAsync(SignalRHubConstants.SendUpdateDashboard);
                    }

                    this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
                }
                else
                {
                    this.Search(string.Empty);
                    response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
                }
            }
            catch (Client.ApiException<ValidationProblemDetailsModel> ex)
            {
                this.snackbar.Add(ex.Result?.Detail, Severity.Error);
            }
        }
    }

    private async Task ExportToExcel()
    {
        var response = await this.apiClient.Catalog_ProductExportAsync(this.searchString);
        if (response.IsSuccess)
        {
            await this.jsRuntime.InvokeVoidAsync("Download", new
            {
                ByteArray = response.Value,
                FileName = $"products_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
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
            var product = this.models.FirstOrDefault(c => c.Id == id);
            if (product != null)
            {
                parameters.Add(nameof(AddEditProductModal.Model), product);
            }
        }

        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = this.dialogService.Show<AddEditProductModal>(id == null ? this.localizer["Create"] : this.localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            this.Search(string.Empty);
        }
    }

    private async Task InvokeImportModal()
    {
        var parameters = new DialogParameters
        {
            { nameof(ImportExcelModal.ModelName), this.localizer["Products"].ToString() }
        };
        Func<UploadRequest, Task<Result<int>>> importFunc = this.ImportExcel;
        parameters.Add(nameof(ImportExcelModal.OnSaved), importFunc);
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            DisableBackdropClick = true
        };
        var dialog = this.dialogService.Show<ImportExcelModal>(this.localizer["Import"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            this.Search(string.Empty);
        }
    }

    private async Task<Result<int>> ImportExcel(UploadRequest request)
    {
        var result = await this.apiClient.Catalog_ProductImportAsync(
            new Client.UploadRequestModel { Data = request.Data, FileName = request.FileName, Extension = request.Extension, Type = request.Type });

        return new Result<int>()
        {
            IsSuccess = result.IsSuccess,
            //Messages = result.Messages,
            Value = result.Value
        };
    }
}