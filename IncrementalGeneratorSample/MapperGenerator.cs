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

        // Define working pipeline
        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "GeneratedNamespace.MapToAttribute",
            predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax { AttributeLists: { Count: 1 } },
            transform: static (context, _) =>
            {
                // Get details from context
                var destinationClassTypeSymbol = context.Attributes.Single(a => a.AttributeClass?.Name == "MapToAttribute")
                    .ConstructorArguments.Single().Value as ITypeSymbol;

                var targetTypeName = destinationClassTypeSymbol!.Name;
                var targetTypeNamespace = destinationClassTypeSymbol.ContainingNamespace.ToDisplayString();
                
                var sourceTypeName = context.TargetSymbol.Name;
                var sourceTypeNamespace = context.TargetSymbol.ContainingNamespace.ToDisplayString();

                var sourceTypeProperties = (context.TargetSymbol as ITypeSymbol)!.GetMembers()
                    .Where(member => member.Kind == SymbolKind.Property)
                    .OfType<IPropertySymbol>()
                    .Select(m => m.Name);
                
                // Map public properties declared from both types
                var mapping = destinationClassTypeSymbol.GetMembers()
                    .Where(member => member.Kind == SymbolKind.Property && sourceTypeProperties.Contains(member.Name))
                    .OfType<IPropertySymbol>()
                    .Select(m => (m.Type, m.Name));
                
                return new
                {
                    TargetTypeName = targetTypeName,
                    TargetTypeNamespace = targetTypeNamespace,
                    SourceTypeName = sourceTypeName,
                    SourceTypeNamespace = sourceTypeNamespace,
                    Mapping = mapping
                };
            });
        
        // Emit generated sources
        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine($"using {model.TargetTypeNamespace};");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"namespace {model.SourceTypeNamespace}");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine($"\tpublic sealed partial class {model.SourceTypeName}");
            sourceBuilder.AppendLine("\t{");
            sourceBuilder.AppendLine($"\t\tpublic {model.TargetTypeName} To{model.TargetTypeName}()");
            sourceBuilder.AppendLine("\t\t{");
            sourceBuilder.AppendLine($"\t\t\treturn new {model.TargetTypeName}");
            sourceBuilder.AppendLine("\t\t\t{");

            foreach (var (_, name) in model.Mapping)
                sourceBuilder.AppendLine($"\t\t\t\t{name} = {name},");
            
            sourceBuilder.AppendLine("\t\t\t};");
            sourceBuilder.AppendLine("\t\t}");
            sourceBuilder.AppendLine("\t}");
            sourceBuilder.AppendLine("}");
            
            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            
            context.AddSource($"{model.SourceTypeName}.g.cs", sourceText);
        });
    }
}