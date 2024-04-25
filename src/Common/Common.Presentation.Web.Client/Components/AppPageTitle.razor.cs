namespace Common.Presentation.Web.Client.Components;

using Microsoft.AspNetCore.Components;

public partial class AppPageTitle
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Description { get; set; }
}
