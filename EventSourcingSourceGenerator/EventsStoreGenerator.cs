using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /* Return as soon as possible to avoid resources wasting */
        
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
        
        var derivedTypes = EnumerateDerivedTypes(context.Compilation, eventType);

        foreach (var derivedType in derivedTypes)
        {
            Console.WriteLine($"With derived type: {derivedType.Name}");
        }
        
        

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

    private static IEnumerable<ITypeSymbol> EnumerateDerivedTypes(Compilation compilation, ISymbol typeSymbol)
    {
        var derivedTypes = new Collection<ITypeSymbol>();
        
        // All syntax tree in source code
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(syntaxTree);
            
            // All declared classes
            var allClasses = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            // Check for all classes if is a derived type of typeSymbol
            foreach (var classDeclarationSyntax in allClasses)
            {
                var candidateSymbol = model.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
                if (!IsDerivedFrom(candidateSymbol?.BaseType, typeSymbol))
                    continue;
                
                derivedTypes.Add(candidateSymbol);
            }
        }

        return derivedTypes;
    }
    
    private static bool IsDerivedFrom(INamedTypeSymbol candidate, ISymbol baseType)
    {
        while (candidate is not null)
        {
            if (candidate.Equals(baseType, SymbolEqualityComparer.Default))
                return true;
            candidate = candidate.BaseType;
        }

        return false;
    }

    private static void BuildEventEntity(ISymbol symbol)
    {
        // TODO: create a event entity class with domain type conversion...
    }
}