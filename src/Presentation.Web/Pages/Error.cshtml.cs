namespace Presentation.Web.Pages;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
#pragma warning disable SA1649 // File name should match first type name
public class ErrorModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
{
    private readonly ILogger<ErrorModel> logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        this.logger = logger;
    }

    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

    public void OnGet()
    {
        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    }
}
