namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using Microsoft.CodeAnalysis;

public class ActionCandidate
{
    public INamedTypeSymbol RequestType { get; internal set; }

    public INamespaceSymbol RequestTypeNamespace { get; internal set; }

    public ITypeSymbol EntityType { get; internal set; }

    public INamespaceSymbol EntityNamespace { get; internal set; }

    public INamedTypeSymbol BaseInterface { get; internal set; }

    public INamedTypeSymbol BaseType { get; internal set; }

    public AttributeData Attribute { get; internal set; }
}