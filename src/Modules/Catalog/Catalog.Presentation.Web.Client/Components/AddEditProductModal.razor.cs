namespace Modules.Catalog.Presentation.Web.Client.Components;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
//using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

public partial class AddEditProductModal
{
    private ErrorBoundary errorBoundary;
    private FluentValidationValidator modelValidator;
    private List<BrandModel> brands = new();
    private List<ProductTypeModel> types = new();

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    [CascadingParameter]
    public HubConnection HubConnection { get; set; }

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    [Parameter]
    public ProductModel Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await this.LoadDataAsync();

        this.HubConnection = this.HubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: really needed?, as it is already injected as a parameter
        await this.HubConnection.Start();
    }

    private async Task LoadDataAsync()
    {
        //await LoadImageAsync();
        await this.LoadBrandsAsync();
        await this.LoadTypesAsync();
    }

    private async Task LoadBrandsAsync()
    {
        var response = await this.apiClient.Catalog_BrandGetAllAsync(null, null, null, "Name", null);
        if (response.IsSuccess)
        {
            this.brands = response.Value?.ToList();
        }
    }

    private async Task LoadTypesAsync()
    {
        var response = await this.apiClient.Catalog_ProductTypesGetAllAsync(null, null, null, "Name");
        if (response.IsSuccess)
        {
            this.types = response.Value?.ToList();
        }
    }

    private async Task SaveAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            if (this.Model.Id == Guid.Empty) // TODO: make this more compact
            {
                // create
                try
                {
                    var response = await this.apiClient.Catalog_ProductPostAsync(this.Model);
                    if (response.IsSuccess)
                    {
                        this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
                        if (this.HubConnection.State == HubConnectionState.Connected)
                        {
                            await this.HubConnection.SendAsync(SignalRHubConstants.SendUpdateDashboard);
                        }

                        this.MudDialog.Close();
                    }
                    else
                    {
                        response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
                    }
                }
                catch (ApiException<ValidationProblemDetailsModel> ex)
                {
                    this.snackbar.Add(ex.Result?.Detail, Severity.Error);
                }
            }
            else
            {
                // update
                try
                {
                    var response = await this.apiClient.Catalog_ProductPutAsync(this.Model.Id.ToString(), this.Model);
                    if (response.IsSuccess)
                    {
                        this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
                        if (this.HubConnection.State == HubConnectionState.Connected)
                        {
                            await this.HubConnection.SendAsync(SignalRHubConstants.SendUpdateDashboard);
                        }

                        this.MudDialog.Close();
                    }
                    else
                    {
                        response.Messages?.ForEach(m => this.snackbar.Add(m, Severity.Error));
                    }
                }
                catch (ApiException<ValidationProblemDetailsModel> ex)
                {
                    this.snackbar.Add(ex.Result?.Detail, Severity.Error);
                }
            }
        }
        else
        {
            this.snackbar.Add("Invalid form", Severity.Warning);
        }
    }

    private async Task<IEnumerable<Guid>> SearchBrands(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        if (string.IsNullOrEmpty(value))
        {
            return this.brands.Select(x => x.Id);
        }

        return this.brands.Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Id);
    }

    private async Task<IEnumerable<Guid>> SearchTypes(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        if (string.IsNullOrEmpty(value))
        {
            return this.types.Select(x => x.Id);
        }

        return this.types.Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Id);
    }

    private void Cancel() => this.MudDialog.Cancel();

    private void CloseError() => this.errorBoundary?.Recover();

    //private async Task LoadImageAsync()
    //{
    //    var data = await ProductManager.GetProductImageAsync(Model.Id);
    //    if (data.Succeeded)
    //    {
    //        var imageData = data.Data;
    //        if (!string.IsNullOrEmpty(imageData))
    //        {
    //            Model.ImageDataURL = imageData;
    //        }
    //    }
    //}

    //private void DeleteAsync()
    //{
    //    Model.ImageDataURL = null;
    //    Model.UploadRequest = new UploadRequest();
    //}

    //private IBrowserFile _file;

    //private async Task UploadFiles(InputFileChangeEventArgs e)
    //{
    //    _file = e.File;
    //    if (_file != null)
    //    {
    //        var extension = Path.GetExtension(_file.Name);
    //        var format = "image/png";
    //        var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
    //        var buffer = new byte[imageFile.Size];
    //        await imageFile.OpenReadStream().ReadAsync(buffer);
    //        Model.ImageDataURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
    //        Model.UploadRequest = new UploadRequest { Data = buffer, UploadType = Application.Enums.UploadType.Product, Extension = extension };
    //    }
    //}
}