using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace IncrementalGeneratorSample;

[Generator]
public sealed class MapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Emit attributes
        context.RegisterPostInitializationOutput(static postInitializationContext =>
        {
            postInitializationContext.AddSource("MapToGeneratedAttribute.g.cs", SourceText.From(
                """
                using System;
                
                namespace GeneratedNamespace
                {
                    /// <summary>
                    /// [AttributeUsage(AttributeTargets.Class)]
                    /// Specifies the object to map
                    /// </summary>
                    /// <param name="entityType">The entity class type</param>
                    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                    internal sealed class MapToAttribute(Type entityType) : Attribute {  }
                }
                """, 
                Encoding.UTF8));
        });

        // Define transform pipeline
        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "GeneratedNamespace.MapToAttribute",
            predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax { AttributeLists: { Count: 1 } },
            transform: static (context, token) =>
            {
                // var containingClass = context.TargetSymbol.ContainingType;
                return new
                {
                    
                };
            });
        
        // Emit generated sources
        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            var sourceText = SourceText.From(
                $$"""
                  
                  """, Encoding.UTF8);
            
            // context.AddSource("");
        });
    }
}