namespace Modules.Catalog.Presentation.Web.Client.Components;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

public partial class AddEditBrandModal
{
    private ErrorBoundary errorBoundary;
    private FluentValidationValidator modelValidator;

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    [CascadingParameter]
    public HubConnection HubConnection { get; set; }

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    [Parameter]
    public BrandModel Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await this.LoadDataAsync();

        this.HubConnection = this.HubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: really needed?, as it is already injected as a parameter
        await this.HubConnection.Start();
    }

    private async Task LoadDataAsync() => await Task.CompletedTask;

    private async Task SaveAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            if (this.Model.Id == Guid.Empty) // TODO: make this more compact
            {
                try // create
                {
                    var response = await this.apiClient.Catalog_BrandPostAsync(this.Model);
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
                try // update
                {
                    var response = await this.apiClient.Catalog_BrandPutAsync(this.Model.Id.ToString(), this.Model);
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

    private void Cancel() => this.MudDialog.Cancel();

    private void CloseError() => this.errorBoundary?.Recover();
}