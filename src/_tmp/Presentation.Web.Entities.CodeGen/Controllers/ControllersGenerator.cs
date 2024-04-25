namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using Microsoft.CodeAnalysis;

[Generator]
public class ControllersGenerator : ISourceGenerator
{
    private const string HttpAttributeName = "GeneratedControllerApi";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ControllerReceiver());
    }

    // https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
    // https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
    // https://medium.com/c-sharp-progarmming/mastering-at-source-generators-18125a5f3fca
    public void Execute(GeneratorExecutionContext context)
    {
        //#if DEBUG
        //        if (!System.Diagnostics.Debugger.IsAttached)
        //        {
        //            System.Diagnostics.Debugger.Launch();
        //        }

        //#endif

        if (context.SyntaxReceiver is ControllerReceiver)
        {
            var actionCandidates = ResolveActionCandidates(context.GetReferencedAssemblies().GetTypes()).ToList();

            var builder = ControllerModel.Builder(context);
            foreach (var actionCandidate in actionCandidates)
            {
                builder.AddCandidate(actionCandidate);
            }

            var templates = LoadTemplates(context);
            var controllers = builder.Build();

            foreach (var controller in controllers)
            {
                context.AddSource($"{controller.Name}EntityController.g.cs", SourceCodeGenerator.Generate(controller, templates));
            }
        }
    }

    private static IEnumerable<ActionCandidate> ResolveActionCandidates(IEnumerable<INamedTypeSymbol> types)
    {
        // find commands/queries that have the HttpRequestattribute and are EntityCommands/Queries
        foreach (var type in types.Where(m => m.AllInterfaces.Any(i =>
            i.Name == "IEntityFindAllQuery" || i.Name == "IEntityFindOneQuery" ||
            i.Name == "IEntityCreateCommand" || i.Name == "IEntityUpdateCommand" || i.Name == "IEntityDeleteCommand")))
        {
            var attribute = type.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name.StartsWith(HttpAttributeName));
            if (attribute == null)
            {
                continue;
            }

            yield return new ActionCandidate
            {
                RequestType = type, // EntityCreateCommand
                RequestTypeNamespace = type.ContainingNamespace, // Modules.Catalog.Application.Commands
                BaseType = type.BaseType, // EntityXXXcommandBase or EntityXXXqueryBase
                EntityType = type.BaseType.TypeArguments.FirstOrDefault(), // Entity
                EntityNamespace = type.BaseType.TypeArguments.FirstOrDefault().ContainingNamespace,
                BaseInterface = type.BaseType.AllInterfaces.FirstOrDefault(i =>
                    i.Name == "IEntityFindAllQuery" || i.Name == "IEntityFindOneQuery" ||
                    i.Name == "IEntityCreateCommand" || i.Name == "IEntityUpdateCommand" || i.Name == "IEntityDeleteCommand"),
                Attribute = attribute
            };
        }
    }

    private static Templates LoadTemplates(GeneratorExecutionContext context)
    {
        Templates templates = new();

        //foreach (var file in context.AdditionalFiles) // TODO: remove the additional templates feature?
        //{
        //    if (Path.GetExtension(file.Path).Equals(".txt", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var options = context.AnalyzerConfigOptions.GetOptions(file);
        //        if (options.TryGetValue("build_metadata.additionalfiles.TemplateType", out var type) &&
        //            Enum.TryParse(type, ignoreCase: true, out TemplateType templateType))
        //        {
        //            var controllerName = TryGetValue(options, "ControllerName");
        //            var template = file.GetText(context.CancellationToken).ToString();

        //            if (templateType != TemplateType.MethodBody)
        //            {
        //                templates.AddTemplate(templateType, controllerName, template);
        //            }
        //            else
        //            {
        //                var methodType = TryGetValue(options, "MethodType");
        //                var methodName = TryGetValue(options, "MethodName");
        //                templates.AddMethodBodyTemplate(controllerName, methodType, methodName, template);
        //            }
        //        }
        //    }
        //}

        return templates;
    }

    //private static string TryGetValue(AnalyzerConfigOptions options, string type)
    //    => options.TryGetValue($"build_metadata.additionalfiles.{type}", out var value) ? value : string.Empty;
}