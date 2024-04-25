namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class GeneratorExecutionContextExtensions
{
    private static readonly DiagnosticDescriptor MissingArgument = new(
        id: "ENT001",
        title: "Missing attribute argument",
        messageFormat: "Argument '{0}' of '{1}Attribute' is required",
        category: "BridgingIT.DevKit.Presentation.Web.Entities",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static void ReportMissingArgument(
        this GeneratorExecutionContext context,
        AttributeSyntax attribute,
        string argumentName)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                MissingArgument,
                attribute.GetLocation(),
                argumentName,
                (attribute.Name as IdentifierNameSyntax)?.Identifier.Text));
}