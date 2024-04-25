namespace Modules.Identity.Presentation.Web.Client.Components;

using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class UserCard
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter]
    public string Class { get; set; }

    [Parameter]
    public string ImageDataUrl { get; set; }

    private string FirstName { get; set; }

    private string SecondName { get; set; }

    private string Email { get; set; }

    private char FirstLetterOfName { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await this.LoadDataAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        if ((await this.AuthState).User is { } user)
        {
            this.Email = user.FindFirst(ClaimTypes.Email)?.Value?.Replace(".com", string.Empty);
            this.FirstName = user.FindFirst(ClaimTypes.Name)?.Value;
            this.SecondName = user.FindFirst(ClaimTypes.Surname)?.Value;
            if (this.FirstName.Length > 0)
            {
                this.FirstLetterOfName = this.FirstName[0];
            }

            var imageResponse = await this.localStorage.GetItemAsync<string>(Common.StorageConstants.Local.UserImageURL);
            if (!string.IsNullOrEmpty(imageResponse))
            {
                this.ImageDataUrl = imageResponse;
            }

            this.StateHasChanged();
        }
    }
}
