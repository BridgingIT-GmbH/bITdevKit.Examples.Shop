namespace Modules.Identity.Presentation.Web.Client.Components;

using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

public partial class Profile
{
    private readonly UpdateProfileRequestModel model = new();
    private FluentValidationValidator modelValidator;
    private IBrowserFile file;
    private char firstLetterOfName;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter]
    public string ImageDataUrl { get; set; }

    public string UserId { get; set; }

    protected override async Task OnInitializedAsync() => await this.LoadDataAsync();

    private async Task LoadDataAsync()
    {
        if ((await this.AuthState).User is { } user)
        {
            this.model.Email = user.FindFirst(ClaimTypes.Email)?.Value;
            this.model.FirstName = user.FindFirst(ClaimTypes.Name)?.Value;
            this.model.LastName = user.FindFirst(ClaimTypes.Surname)?.Value;
            this.model.PhoneNumber = user.FindFirst(ClaimTypes.MobilePhone)?.Value;
            this.UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var response = await this.apiClient.Account_GetProfilePictureAsync(this.UserId);
            //if (response.IsSuccess)
            //{
            //    this.ImageDataUrl = response.Result.Data;
            //}

            if (this.model.FirstName?.Length > 0)
            {
                this.firstLetterOfName = this.model.FirstName[0];
            }
        }
    }

    private async Task SaveAsync()
    {
        if (this.modelValidator?.Validate() == true)
        {
            var response = await this.apiClient.Identity_AccountUpdateProfileAsync(this.model);
            if (response.IsSuccess)
            {
                await this.authManager.LogoutAsync();
                this.snackbar.Add(this.localizer["Your Profile has been updated. Please Login to Continue."], Severity.Success);
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
        else
        {
            this.snackbar.Add("Invalid form", Severity.Warning);
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        this.file = e.File;
        if (this.file != null)
        {
            var extension = Path.GetExtension(this.file.Name);
            var fileName = $"{this.UserId}-{Guid.NewGuid()}{extension}";
            var imageFile = await e.File.RequestImageFileAsync("image/png", 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            var request = new UpdateProfilePictureRequestModel { Data = buffer, FileName = fileName, Extension = extension/*, UploadType = Application.Enums.UploadType.ProfilePicture*/ };
            var response = await this.apiClient.Identity_AccountUpdateProfilePictureAsync(this.UserId, request);
            if (response.IsSuccess)
            {
                await this.localStorage.SetItemAsync("userImageURL", response.Value);
                this.snackbar.Add(this.localizer["Profile picture added."], Severity.Success);
                this.navManager.NavigateTo("/account", true);
            }
            else
            {
                foreach (var error in response.Messages)
                {
                    this.snackbar.Add(error, Severity.Error);
                }
            }
        }
    }

    //    private async Task DeleteAsync()
    //    {
    //        var parameters = new DialogParameters
    //        {
    //            {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), $"{string.Format(this._localizer["Do you want to delete the profile picture of {0}"], this._profileModel.Email)}?"}
    //        };
    //        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
    //        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(this._localizer["Delete"], parameters, options);
    //        var result = await dialog.Result;
    //        if (!result.Cancelled)
    //        {
    //            var request = new UpdateProfilePictureRequest { Data = null, FileName = string.Empty, UploadType = Application.Enums.UploadType.ProfilePicture };
    //            var data = await _accountManager.UpdateProfilePictureAsync(request, this.UserId);
    //            if (data.Succeeded)
    //            {
    //                await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
    //                this.ImageDataUrl = string.Empty;
    //                _snackBar.Add(this._localizer["Profile picture deleted."], Severity.Success);
    //                _navManager.NavigateTo("/account", true);
    //            }
    //            else
    //            {
    //                foreach (var error in data.Messages)
    //                {
    //                    _snackBar.Add(error, Severity.Error);
    //                }
    //            }
    //        }
    //    }
}
