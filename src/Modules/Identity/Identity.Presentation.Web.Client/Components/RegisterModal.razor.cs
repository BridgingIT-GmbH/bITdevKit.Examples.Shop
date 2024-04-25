namespace Modules.Identity.Presentation.Web.Client.Components;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

public partial class RegisterModal
{
    private ErrorBoundary errorBoundary;
    private FluentValidationValidator modelValidator;
    private readonly RegisterRequestModel model = new();
    private bool passwordVisibility;
    private InputType passwordInput = InputType.Password;
    private string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    private async Task SaveAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            var response = await this.apiClient.Identity_UserRegisterAsync(this.model);
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
        else
        {
            this.snackbar.Add("Invalid form", Severity.Warning);
        }
    }

    private void TogglePasswordVisibility()
    {
        if (this.passwordVisibility)
        {
            this.passwordVisibility = false;
            this.passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            this.passwordInput = InputType.Password;
        }
        else
        {
            this.passwordVisibility = true;
            this.passwordInputIcon = Icons.Material.Filled.Visibility;
            this.passwordInput = InputType.Text;
        }
    }

    private void Cancel() => this.MudDialog.Cancel();

    private void CloseError() => this.errorBoundary?.Recover();
}
