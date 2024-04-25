namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System;
using System.Collections.Generic;

public class Templates
{
    private readonly Dictionary<string, string> templates = new();

    public void AddTemplate(TemplateType type, string controllerName, string template)
    {
        this.templates[GetName(type, controllerName)] = template;
    }

    public void AddMethodBodyTemplate(string controllerName, string httpType, string requestType, string template) // TODO: httptype can be removed here
    {
        this.templates[GetMethodBodyTemplateName(controllerName, httpType, requestType)] = template;
    }

    public string GetControllerTemplate(TemplateType type, string controllerName)
        => this.templates.ContainsKey(GetName(type, controllerName))
            ? this.templates[GetName(type, controllerName)]
            : this.templates.ContainsKey(GetName(type, string.Empty))
                ? this.templates[GetName(type, string.Empty)]
                : type switch
                {
                    TemplateType.Controller => EmbeddedResource.GetContent("Controllers.Templates.EntityController.scriban"),
                    TemplateType.ControllerAttributes => EmbeddedResource.GetContent("Controllers.Templates.EntityControllerAttributes.scriban"),
                    TemplateType.ControllerUsings => EmbeddedResource.GetContent("Controllers.Templates.EntityControllerUsings.scriban"),
                    TemplateType.ControllerMethods => EmbeddedResource.GetContent("Controllers.Templates.EntityControllerMethods.scriban"),
                    //TemplateType.MethodAttributes => null,
                    //TemplateType.MethodBody => null,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unexpected template type: {type}.")
                };

    public string GetMethodBodyTemplate(string controllerName, string httpType, string requestType) // TODO: httptype can be removed here
        => this.templates.ContainsKey(GetMethodBodyTemplateName(controllerName, httpType, requestType))
            ? this.templates[GetMethodBodyTemplateName(controllerName, httpType, requestType)]
            : this.templates.ContainsKey(GetMethodBodyTemplateName(string.Empty, httpType, string.Empty))
                ? this.templates[GetMethodBodyTemplateName(string.Empty, httpType, string.Empty)]
                : EmbeddedResource.GetContent($"Controllers.Templates.{requestType}MethodBody.scriban");

    private static string GetName(TemplateType type, string controllerName)
        => $"{type}-{controllerName}";

    private static string GetMethodBodyTemplateName(string controllerName, string httpType, string requestType)
        => $"{controllerName}-{httpType}-{requestType}";
}