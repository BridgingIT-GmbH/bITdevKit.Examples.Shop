namespace Modules.Identity.Presentation.Web.Client.Components;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

public partial class RoleModal
{
    private ErrorBoundary errorBoundary;
    private FluentValidationValidator modelValidator;

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public RoleRequestModel Model { get; set; } = new();

    private async Task SaveAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            try
            {
                var response = await this.apiClient.Identity_RoleCreateAsync(this.Model);
                if (response.IsSuccess)
                {
                    this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
                    this.MudDialog.Close();
                }
                else
                {
                    foreach (var message in response.Messages.SafeNull())
                    {
                        this.snackbar.Add(message, Severity.Error);
                    }
                }
            }
            catch (ApiException<ValidationProblemDetailsModel> ex)
            {
                this.snackbar.Add(ex.Result?.Detail, Severity.Error);
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
