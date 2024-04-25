namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;

using Scriban.Runtime;

internal class ScribanFunctions : ScriptObject
{
    // called from scriban templace by using method_body
    public static string MethodBody(string controllerName, ActionMethodModel method, Templates templates)
        => SourceCodeGenerator
            .RenderBody(method, templates.GetMethodBodyTemplate(controllerName, method.HttpMethod, method.Name));

    public static string Pluralize(string controllerName)
        => Pluralizer.Pluralize(100, controllerName);
}