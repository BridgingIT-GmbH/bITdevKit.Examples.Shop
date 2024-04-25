namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Scriban.Runtime;

public static class SourceCodeGenerator
{
    public static SourceText Generate(ControllerModel controller, Templates templates)
    {
        var output = RenderBody(new
        {
            Usings = templates.GetControllerTemplate(TemplateType.ControllerUsings, controller.Name),
            Attributes = RenderControllerAttributes(controller, templates),
            Body = RenderControllerBody(controller, templates),
            Controller = controller,
            Module = controller.Methods?.FirstOrDefault(m => !string.IsNullOrEmpty(m.Module))?.Module ?? null
        }, templates.GetControllerTemplate(TemplateType.Controller, controller.Name));

        output = Format(output);

        return SourceText.From(output, Encoding.UTF8);
    }

    public static string RenderBody(object body, string templateSource)
    {
        var template = Template.Parse(templateSource);
        var context = CreateContext(body);

        return template.Render(context);
    }

    private static string Format(string output)
    {
        var tree = CSharpSyntaxTree.ParseText(output);
        var root = (CSharpSyntaxNode)tree.GetRoot();

        return root.NormalizeWhitespace(elasticTrivia: true).ToFullString();
    }

    private static string RenderControllerAttributes(ControllerModel controller, Templates templates)
        => RenderBody(controller, templates.GetControllerTemplate(TemplateType.ControllerAttributes, controller.Name));

    private static string RenderControllerBody(ControllerModel controller, Templates templates)
    {
        return RenderBody(new
        {
            Controller = controller,
            controller.Methods,
            templates
        }, templates.GetControllerTemplate(TemplateType.ControllerMethods, controller.Name));
    }

    private static TemplateContext CreateContext(object body)
    {
        var context = new TemplateContext();

        var scriptObject = new ScriptObject();
        scriptObject.Import(body);
        context.PushGlobal(scriptObject);

        var functions = new ScribanFunctions();
        context.PushGlobal(functions);

        return context;
    }
}