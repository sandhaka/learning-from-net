using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGeneratorSample
{
    [Generator]
    public class SampleGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation; // Get the supplier user code
            var syntaxTrees = compilation.SyntaxTrees;

            foreach (var syntaxTree in syntaxTrees)
            {
                var model = compilation.GetSemanticModel(syntaxTree);
                
                var classes = syntaxTree.GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>();

                foreach (var @class in classes)
                {
                    var classSymbol = model.GetDeclaredSymbol(@class);

                    context.AddSource(
                        $"{classSymbol.Name}_HelloWorld.cs",
                        SourceText.From($$"""

                                          namespace {{classSymbol.ContainingNamespace}}
                                          {
                                              public partial class {{@class.Identifier}}
                                              {
                                                  public void HelloWorld()
                                                  {
                                                      System.Console.WriteLine("Hello, World!");
                                                  }
                                              }
                                          }

                                          """, Encoding.UTF8));
                }
            }
        }
    }
}