namespace Modules.Identity.Presentation.Web.Client.Pages;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using MudBlazor;

public partial class RegisterPage
{
    private RegisterRequestModel model = new();
    private FluentValidationValidator modelValidator;
    private bool passwordVisibility;
    private InputType passwordInput = InputType.Password;
    private string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private async Task SubmitAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            try
            {
                var response = await this.apiClient.Identity_UserRegisterAsync(this.model);
                if (response.IsSuccess)
                {
                    this.snackbar.Add(response.Messages.FirstOrDefault(), Severity.Success);
                    this.navManager.NavigateTo(IdentityPageConstants.Login);
                    this.model = new RegisterRequestModel();
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
}
