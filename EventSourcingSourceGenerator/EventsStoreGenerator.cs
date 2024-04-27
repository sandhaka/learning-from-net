using System;
using System.Linq;
using EventSourcingSourceGenerator.Attributes;
using EventSourcingSourceGenerator.Option;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventSourcingSourceGenerator;

[Generator]
public class EventsStoreGenerator : ISourceGenerator
{
    /// <summary>
    /// Will work at the beginning while initializing analyzer.
    /// You can register for some events or one-time configurations can be performed here.
    /// </summary>
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => 
            new AttributeSyntaxReceiver<EventsStoreGeneratorTargetAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<EventsStoreGeneratorTargetAttribute> syntaxReceiver)
            return;

        if (syntaxReceiver.StoreTargetClass is null)
            return;

        var classSyntax = syntaxReceiver.StoreTargetClass;
        
        // Converting the class to semantic model to access much more meaningful data.
        var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
        
        // Search the Event type to use
        var searchResult = SearchEventTypeInTargetClass(model, classSyntax);
        
        if (searchResult.IsNone())
            return;

        var eventType = searchResult.Reduce();
        
        Console.WriteLine($"Found a target type name for event store: {eventType.Name}");

        // // Finding attribute over class
        // var attribute = classSyntax.AttributeLists
        //     .SelectMany(sm => sm.Attributes)
        //     .First(x => x.Name.IsAttribute<EventsStoreGeneratorTargetAttribute>());
        //
        // // Read the content of the embedded resource (text file)
        // var assembly = Assembly.GetExecutingAssembly();
        // using var stream = assembly.GetManifestResourceStream("EventSourcingSourceGenerator.events_store.templ") ??
        //                    throw new ArgumentNullException();
        //
        // using var reader = new StreamReader(stream);
        // var templateContent = reader.ReadToEnd();


    }

    private Option<ITypeSymbol> SearchEventTypeInTargetClass(SemanticModel model, TypeDeclarationSyntax classDeclarationSyntax)
    {
        var variablesDeclarations = classDeclarationSyntax.Members
            .Where(x => x is FieldDeclarationSyntax)
            .Cast<FieldDeclarationSyntax>();
        
        // Navigate class fields
        foreach (var variableDeclarationSyntax in variablesDeclarations)
        {
            // Search member attribute
            var eventTypeDeclarationLocated = variableDeclarationSyntax.AttributeLists
                .Any(sm => sm.Attributes
                    .Any(a => a.Name.IsAttribute<EventTypeTargetAttribute>()));

            if (!eventTypeDeclarationLocated) 
                continue;
            
            // Get the symbol for the variable declarator
            foreach (var variableDeclaratorSyntax in variableDeclarationSyntax.Declaration.Variables)
            {
                var symbol = model.GetDeclaredSymbol(variableDeclaratorSyntax);

                if (symbol is not IFieldSymbol fieldSymbol)
                    continue;

                if (fieldSymbol.Type is not INamedTypeSymbol namedTypeSymbol)
                    continue;

                if (IsIEnumerableGenericType(namedTypeSymbol))
                {
                    return new Some<ITypeSymbol>(namedTypeSymbol.TypeArguments.Single());
                }
            }
        }

        return new None<ITypeSymbol>();
    }

    private static bool IsIEnumerableGenericType(INamedTypeSymbol namedTypeSymbol)
    {
        if (!namedTypeSymbol.IsGenericType || namedTypeSymbol.TypeParameters.Length != 1)
            return false;

        var implementedInterfaces = namedTypeSymbol.AllInterfaces.Select(i => i.Name);

        return namedTypeSymbol.Name == "IEnumerable" ||
               implementedInterfaces.Contains("IEnumerable");
    }
}