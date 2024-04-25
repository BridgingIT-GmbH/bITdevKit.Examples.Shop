namespace Modules.Identity.Presentation.Web.Client.Pages;

using BridgingIT.DevKit.Common;
using Common.Presentation.Web.Client.Components;
using Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

public partial class UserProfilePage
{
    private bool active;
    private char firstLetterOfName;
    private string firstName;
    private string lastName;
    private string phoneNumber;
    private string email;
    private bool loaded;

    [CascadingParameter]
    public HubConnection HubConnection { get; set; }

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Description { get; set; }

    [Parameter]
    public string ImageDataUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.HubConnection = this.HubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: really needed?, as it is already injected as a parameter
        await this.HubConnection.Start();

        await this.LoadData();
        this.loaded = true;
    }

    private async Task LoadData()
    {
        var userId = this.Id;
        var response = await this.apiClient.Identity_UserGetByIdAsync(userId);
        if (response.IsSuccess)
        {
            var user = response.Value;
            if (user != null)
            {
                this.firstName = user.FirstName;
                this.lastName = user.LastName;
                this.email = user.Email;
                this.phoneNumber = user.PhoneNumber;
                this.active = user.IsActive;
                //var response = await this.apiClient.Account_GetProfilePictureAsync(userId);
                //if (response.IsSuccess)
                //{
                //    this.ImageDataUrl = response.Result.Data;
                //}
            }

            this.Title = $"{this.firstName} {this.lastName}'s {this.localizer["Profile"]}";
            this.Description = this.email;
            if (this.firstName.Length > 0)
            {
                this.firstLetterOfName = this.firstName[0];
            }
        }
    }

    private async Task ToggleUserStatus()
    {
        var request = new ToggleUserStatusRequestModel { ActivateUser = this.active, UserId = this.Id };
        try
        {
            var response = await this.apiClient.Identity_UserToggleUserStatusAsync(request);
            if (response.IsSuccess)
            {
                this.snackbar.Add(this.localizer["Updated User Status."], Severity.Success);

                if (!this.active)
                {
                    await this.HubConnection.SendAsync(SignalRHubConstants.OnDeactivateUser, this.Id);
                }

                this.navManager.NavigateTo("/identity/users");
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

    private async Task Delete()
    {
        var dialog = this.dialogService.Show<DeleteConfirmation>(
            this.localizer["Delete"],
            new DialogParameters
            {
                { nameof(DeleteConfirmation.ContentText), string.Format(this.localizer["Delete Content"], this.Id) }
            },
            new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true });
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            try
            {
                var response = await this.apiClient.Identity_UserDeleteAsync(this.Id);
                if (response.IsSuccess)
                {
                    this.snackbar.Add(this.localizer["User Deleted."], Severity.Success);
                    this.navManager.NavigateTo("/identity/users");
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
    }
}