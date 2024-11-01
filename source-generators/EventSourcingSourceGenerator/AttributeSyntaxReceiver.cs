using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventSourcingSourceGenerator;

public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
    where TAttribute : Attribute
{
    public ClassDeclarationSyntax StoreTargetClass { get; private set; }
    
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
            return;
        
        if (!classDeclarationSyntax.AttributeLists.Any())
            return;

        if (classDeclarationSyntax.AttributeLists
            .Any(al => al.Attributes
                .Any(a => a.Name.IsAttribute<TAttribute>())))
        {
            StoreTargetClass = classDeclarationSyntax;
        }
    }
}