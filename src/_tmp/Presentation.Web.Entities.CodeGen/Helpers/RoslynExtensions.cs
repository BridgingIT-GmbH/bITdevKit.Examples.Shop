namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable SA1202 // Elements should be ordered by access
internal static class RoslynExtensions // TODO: move to Common.Utilities?
{
    public static string GetAttributeName<TAttribute>()
        where TAttribute : Attribute
        => typeof(TAttribute).Name.Replace("Attribute", string.Empty);

    public static CompilationUnitSyntax GetCompilationUnit(this SyntaxNode syntaxNode)
        => syntaxNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();

    public static bool HaveAttribute(this TypeDeclarationSyntax typeDeclaration, string attributeName)
        => typeDeclaration?.AttributeLists.Count > 0
            && typeDeclaration
                .AttributeLists
                   .SelectMany(SelectWithAttributes(attributeName))
                   .Any();

    public static bool HaveAnyOfAttributes(this TypeDeclarationSyntax typeDeclaration, ISet<string> attributesName)
        => typeDeclaration?.AttributeLists.Count > 0
            && typeDeclaration
                .AttributeLists
                   .SelectMany(SelectWithAttributes(attributesName))
                   .Any();

    private static IEnumerable<AttributeSyntax> GetAttributes(
        this TypeDeclarationSyntax typeDeclaration,
        string attributeName)
        => typeDeclaration
            .AttributeLists
                .SelectMany(SelectWithAttributes(attributeName));

    public static IEnumerable<AttributeSyntax> GetAttributes(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> attributesName)
        => typeDeclaration
            .AttributeLists
                .SelectMany(SelectWithAttributes(attributesName));

    public static AttributeSyntax GetAttribute(this TypeDeclarationSyntax typeDeclaration, string attributeName)
        => typeDeclaration.GetAttributes(attributeName).FirstOrDefault();

    public static AttributeSyntax GetAttribute(this TypeDeclarationSyntax typeDeclaration, ISet<string> attributesName)
        => typeDeclaration.GetAttributes(attributesName).FirstOrDefault();

    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> SelectWithAttributes(string attributeName)
        => l => l?.Attributes.Where(a => (a.Name as IdentifierNameSyntax)?.Identifier.Text == attributeName);

    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> SelectWithAttributes(ISet<string> attributes)
        => l => l?.Attributes.Where(a => attributes.Contains((a.Name as IdentifierNameSyntax)?.Identifier.Text));

    public static string GetTypeName(this TypeDeclarationSyntax typeDeclaration)
        => typeDeclaration.Identifier.Text;

    public static string GetClassModifier(this TypeDeclarationSyntax typeDeclaration)
        => typeDeclaration.Modifiers.ToFullString().Trim();

    public static string GetNamespace(this CompilationUnitSyntax root)
        => root.ChildNodes()
            .OfType<NamespaceDeclarationSyntax>()
            .First().Name.ToString();

    public static bool ContainsArguments(this AttributeSyntax attribute, string argumentName)
        => attribute
           .ArgumentList
           .Arguments
           .Any(p => p.NameEquals.Name.Identifier.ValueText == argumentName);

    public static string GetFirstArgumentWithoutName(this AttributeSyntax attribute)
    {
        var value = attribute
            .ArgumentList
            .Arguments
            .FirstOrDefault(p => p.NameEquals is null)?.Expression as LiteralExpressionSyntax;

        return value?.Token.ValueText;
    }

    public static string GetStringArgument(
        this AttributeSyntax attribute,
        string argumentName)
    {
        var value = attribute.GetArgument<LiteralExpressionSyntax>(argumentName);

        return value?.Token.ValueText;
    }

    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
        => type.GetBaseTypesAndThis().SelectMany(n => n.GetMembers());

    public static Dictionary<string, ITypeSymbol> GetProperties(this INamedTypeSymbol symbol)
        => symbol.GetAllMembers()
            .Where(x => x.Kind == SymbolKind.Property)
            .OfType<IPropertySymbol>()
            .ToDictionary(p => p.Name, p => p.Type, StringComparer.OrdinalIgnoreCase);

    public static MethodDeclarationSyntax GetMethodSymbol(this TypeDeclarationSyntax typeDeclaration, string name)
        => typeDeclaration
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == name);

    public static HashSet<string> GetArrayArguments(
        this AttributeSyntax attribute,
        string argumentName,
        SemanticModel semanticModel,
        Func<string, string> processItem)
    {
        HashSet<string> ret = new();
        var expressions =
            attribute.GetArgument<ArrayCreationExpressionSyntax>(argumentName)?.Initializer.Expressions;

        if (expressions != null)
        {
            foreach (var expression in expressions)
            {
                var value = semanticModel.GetConstantValue(expression);
                if (value.HasValue)
                {
                    ret.Add(processItem(value.Value.ToString()));
                }
            }
        }

        return ret;
    }

    public static string GetConstantAttribute(
        this AttributeSyntax attribute,
        string argumentName,
        SemanticModel semanticModel)
    {
        var expression = attribute.GetArgument<ExpressionSyntax>(argumentName);
        var value = semanticModel.GetConstantValue(expression);

        return value.HasValue ? value.Value.ToString() : null;
    }

    public static T GetArgument<T>(this AttributeSyntax attribute, string argumentName)
        where T : ExpressionSyntax
        => attribute
            .ArgumentList
            .Arguments
            .FirstOrDefault(p => p.NameEquals?.Name.Identifier.ValueText == argumentName)?.Expression as T;

    public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    public static INamedTypeSymbol GetTypeArgument(
        this AttributeSyntax attribute,
        string argumentName,
        SemanticModel semanticModel)
    {
        var typeOfExpression = attribute.GetArgument<TypeOfExpressionSyntax>(argumentName)?.Type;
        if (typeOfExpression is not null)
        {
            var typeInfo = semanticModel.GetTypeInfo(typeOfExpression);

            return (INamedTypeSymbol)typeInfo.Type;
        }
        else
        {
            return null;
        }
    }

    public static IEnumerable<IAssemblySymbol> GetReferencedAssemblies(this GeneratorExecutionContext context)
    {
        return context.Compilation.SourceModule.ReferencedAssemblySymbols.Where(a => !a.Name.StartsWith("Bit.Baukasten", StringComparison.OrdinalIgnoreCase));
    }

    public static IAssemblySymbol GetReferencedAssembly(this GeneratorExecutionContext context, string assemblyName)
    {
        return context.Compilation.SourceModule.ReferencedAssemblySymbols.FirstOrDefault(a => a.Name == assemblyName);
    }

    public static IEnumerable<INamedTypeSymbol> GetTypes(this IAssemblySymbol symbol)
    {
        if (symbol == null)
        {
            yield return default;
        }

        var visitor = new ExportedTypesCollector();
        visitor.VisitAssembly(symbol);

        foreach (var type in visitor.GetPublicTypes())
        {
            yield return type;
        }
    }

    public static IEnumerable<INamedTypeSymbol> GetTypes(this IEnumerable<IAssemblySymbol> symbols)
    {
        if (symbols == null)
        {
            yield return default;
        }

        foreach (var symbol in symbols)
        {
            var visitor = new ExportedTypesCollector();
            visitor.VisitAssembly(symbol);

            foreach (var type in visitor.GetPublicTypes())
            {
                yield return type;
            }
        }
    }

    public static bool IsAccessibleOutsideOfAssembly(this ISymbol symbol) =>
        symbol.DeclaredAccessibility switch
        {
            Accessibility.Private => false,
            Accessibility.Internal => false,
            Accessibility.ProtectedAndInternal => false,
            Accessibility.Protected => true,
            Accessibility.ProtectedOrInternal => true,
            Accessibility.Public => true,
            _ => true,
        };
}
#pragma warning restore SA1202 // Elements should be ordered by access