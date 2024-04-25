namespace Modules.Identity.Presentation.Web.Client.Pages;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class ResetPage
{
    private readonly ResetPasswordRequestModel model = new();
    private FluentValidationValidator modelValidator;
    private bool passwordVisibility;
    private InputType passwordInput = InputType.Password;
    private string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    [Parameter]
    [SupplyParameterFromQuery(Name = "Token")]
    public string TokenDecoded { get; set; }

    protected override void OnInitialized() => this.model.Token = Encoding.UTF8.GetString(Convert.FromBase64String(this.TokenDecoded));

    private async Task SubmitAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            if (!string.IsNullOrEmpty(this.model.Token))
            {
                try
                {
                    var response = await this.apiClient.Identity_UserResetPasswordAsync(this.model);
                    if (response.IsSuccess)
                    {
                        this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
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
                this.snackbar.Add(this.localizer["Token Not Found!"], Severity.Error);
            }
        }
        else
        {
            this.snackbar.Add(this.localizer["Invalid Form!"], Severity.Warning);
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
