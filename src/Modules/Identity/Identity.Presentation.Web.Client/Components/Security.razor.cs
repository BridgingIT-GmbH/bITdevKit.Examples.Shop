namespace Modules.Identity.Presentation.Web.Client.Components;

using Blazored.FluentValidation;
using MudBlazor;

public partial class Security
{
    private readonly ChangePasswordRequestModel model = new();
    private FluentValidationValidator modelValidator;
    private bool currentPasswordVisibility;
    private InputType currentPasswordInput = InputType.Password;
    private string currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool newPasswordVisibility;
    private InputType newPasswordInput = InputType.Password;
    private string newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private async Task SaveAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            try
            {
                var response = await this.apiClient.Identity_AccountChangePasswordAsync(this.model);
                if (response.IsSuccess)
                {
                    this.snackbar.Add(this.localizer["Password Changed!"], Severity.Success);
                    this.model.Password = string.Empty;
                    this.model.NewPassword = string.Empty;
                    this.model.ConfirmNewPassword = string.Empty;
                }
                else
                {
                    foreach (var error in response.Messages)
                    {
                        this.snackbar.Add(error, Severity.Error);
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

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (this.newPasswordVisibility)
            {
                this.newPasswordVisibility = false;
                this.newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                this.newPasswordInput = InputType.Password;
            }
            else
            {
                this.newPasswordVisibility = true;
                this.newPasswordInputIcon = Icons.Material.Filled.Visibility;
                this.newPasswordInput = InputType.Text;
            }
        }
        else
        {
            if (this.currentPasswordVisibility)
            {
                this.currentPasswordVisibility = false;
                this.currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                this.currentPasswordInput = InputType.Password;
            }
            else
            {
                this.currentPasswordVisibility = true;
                this.currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                this.currentPasswordInput = InputType.Text;
            }
        }
    }
}
