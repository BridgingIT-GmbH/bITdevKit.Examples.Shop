namespace Modules.Identity.Presentation.Web.Client.Pages;

using BridgingIT.DevKit.Presentation.Web.Client;

//using System.Security.Claims;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

public partial class LoginPage
{
    private readonly TokenRequestModel model = new();
    private FluentValidationValidator modelValidator;
    private bool passwordVisibility;
    private InputType passwordInput = InputType.Password;
    private string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery]
    public string RedirectUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("000000000000000000");

        if (this.authManager.GetType().Name.StartsWith("AzureAd"))
        {
            Console.WriteLine("111111111111111");
            this.authManager.NavigateToExternalLogin(this.navManager.Uri);
            return;
        }

        Console.WriteLine("22222222222222222222");
        var authState = await this.AuthState;
        if (authState.User.Identity?.IsAuthenticated is true)
        {
            this.navManager.NavigateTo("/");
        }
    }

    private async Task SubmitAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            try
            {
                var result = await this.authManager.LoginAsync(this.model.Email, this.model.Password);

                if (result.IsFailure)
                {
                    await this.jsRuntime.LogAsync("1 - login failed");
                    foreach (var message in result.Messages)
                    {
                        this.snackbar.Add(message, Severity.Error);
                    }
                }
                else
                {
                    await this.jsRuntime.LogAsync("1 - login finished");
                }
            }
            catch (ApiException<ValidationProblemDetailsModel> ex)
            {
                this.snackbar.Add(ex.Result?.Detail, Severity.Error);
            }

            if (!string.IsNullOrEmpty(this.RedirectUrl) &&
                !this.RedirectUrl.Contains(IdentityPageConstants.Login, StringComparison.OrdinalIgnoreCase))
            {
                await this.jsRuntime.LogAsync("2 - login navigate " + this.RedirectUrl);
                this.navManager.NavigateTo(Uri.UnescapeDataString(this.RedirectUrl));
            }
            else
            {
                await this.jsRuntime.LogAsync("2 - login navigate /");
                this.navManager.NavigateTo("/");
            }

            await this.jsRuntime.LogAsync("3 - login done");
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

    private void FillAdministratorCredentials()
    {
        this.model.Email = "admin@acmeshop.com";
        this.model.Password = "Fidespic032";
    }

    private void FillBasicUserCredentials()
    {
        this.model.Email = "john.doe@acmeshop.com";
        this.model.Password = "Fidespic032";
    }
}