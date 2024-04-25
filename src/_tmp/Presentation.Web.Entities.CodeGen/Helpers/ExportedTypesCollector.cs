namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

// source: https://stackoverflow.com/questions/64623689/get-all-types-from-compilation-using-roslyn
public class ExportedTypesCollector : SymbolVisitor
{
    private readonly CancellationToken cancellationToken;
    private readonly HashSet<INamedTypeSymbol> exportedTypes;

    public ExportedTypesCollector(CancellationToken cancellation = default)
    {
        this.cancellationToken = cancellation;
        this.exportedTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    }

    public ImmutableArray<INamedTypeSymbol> GetPublicTypes() => this.exportedTypes.ToImmutableArray();

    public override void VisitAssembly(IAssemblySymbol symbol)
    {
        this.cancellationToken.ThrowIfCancellationRequested();
        symbol.GlobalNamespace.Accept(this);
    }

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        foreach (var namespaceOrType in symbol.GetMembers())
        {
            this.cancellationToken.ThrowIfCancellationRequested();
            namespaceOrType.Accept(this);
        }
    }

    public override void VisitNamedType(INamedTypeSymbol type)
    {
        this.cancellationToken.ThrowIfCancellationRequested();

        if (!type.IsAccessibleOutsideOfAssembly() || !this.exportedTypes.Add(type))
        {
            return;
        }

        var nestedTypes = type.GetTypeMembers();

        if (nestedTypes.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var nestedType in nestedTypes)
        {
            this.cancellationToken.ThrowIfCancellationRequested();
            nestedType.Accept(this);
        }
    }
}
