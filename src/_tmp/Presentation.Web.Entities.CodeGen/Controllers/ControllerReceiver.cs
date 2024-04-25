namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class ControllerReceiver : ISyntaxReceiver
{
    private readonly List<TypeDeclarationSyntax> candidates = new();

    public IEnumerable<TypeDeclarationSyntax> Candidates => this.candidates;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        //if (syntaxNode is TypeDeclarationSyntax typeDeclaration
        //    && typeDeclaration.HaveAnyOfAttributes(HttpMethods.Attributes))
        //{
        //    this.candidates.Add(typeDeclaration);
        //}
    }
}
