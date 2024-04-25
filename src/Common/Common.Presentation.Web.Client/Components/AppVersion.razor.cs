namespace Common.Presentation.Web.Client.Components;

using System.Reflection;
using Microsoft.AspNetCore.Components;

public partial class AppVersion
{
    private readonly string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    [Parameter]
    public bool DrawerMode { get; set; } = true;
}
