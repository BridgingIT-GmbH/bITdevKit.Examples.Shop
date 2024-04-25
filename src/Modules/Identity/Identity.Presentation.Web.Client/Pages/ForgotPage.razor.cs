namespace Modules.Identity.Presentation.Web.Client.Pages;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using MudBlazor;

public partial class ForgotPage
{
    private readonly ForgotPasswordRequestModel model = new();
    private FluentValidationValidator modelValidator;

    private async Task SubmitAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            try
            {
                var response = await this.apiClient.Identity_UserForgotPasswordAsync(this.model);
                if (response.IsSuccess)
                {
                    this.snackbar.Add(this.localizer["Done!"], Severity.Success);
                    this.navManager.NavigateTo("/");
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
}
