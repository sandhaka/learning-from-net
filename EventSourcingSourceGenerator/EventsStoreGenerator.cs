using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EventSourcingSourceGenerator.Attributes;
using EventSourcingSourceGenerator.Dto;
using EventSourcingSourceGenerator.Option;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using EventTypeDetailedResult = (
    Microsoft.CodeAnalysis.ITypeSymbol TypeSymbol,
    string DateTimeEventPropertyName,
    string DbName,
    string TableOrCollectionName
    );

/*
 * Emitting infrastructure source code for event sourcing based project from Root Aggregate and his event types.
 */

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

        var (eventType, dateTimeEventPropertyName, dbName, tableOrCollectionName) = searchResult.Reduce();

        Console.WriteLine($"Found a target type name for event store: {eventType.Name}");
        
        var eventTypeProperties = eventType.GetMembers()
            .Where(m => m.Kind == SymbolKind.Property)
            .Cast<IPropertySymbol>()
            .Select(p => (p.Type.Name, p.Name));

        var derivedTypes = EnumerateDerivedTypes(context.Compilation, eventType).ToArray();

        foreach (var derivedType in derivedTypes)
            Console.WriteLine($"With derived type: {derivedType.Name}");
        
        var assembly = Assembly.GetExecutingAssembly();

        var templateContent = ReadEmbeddedResource(assembly, "EventSourcingSourceGenerator.events_store.templ");
        var rootNamespaceName = ReadEmbeddedResource(assembly, "EventSourcingSourceGenerator.rootnamespace.name");

        var eventCodeParameters = new EventCodeParameters
        {
            RootNamespace = rootNamespaceName,
            TypeNamespace = eventType.ContainingNamespace.ToString(),
            TypeBaseName = eventType.Name,
            Properties = eventTypeProperties,
            TypeNames = derivedTypes.Select(x => x.Name)
        };

        var eventsStoreCodeParameters = new EventsStoreCodeParameter
        {
            RootNamespace = rootNamespaceName,
            TypeNamespace = eventType.ContainingNamespace.ToString(),
            ClassName = classSyntax.Identifier.ValueText,
            EventClassName = eventType.Name,
            DateTimeEventPropertyName = dateTimeEventPropertyName,
            TableOrCollectionName = tableOrCollectionName,
            Template = templateContent,
            DatabaseName = dbName
        };

        var eventDataSourceText = BuildEventEntitySourceCode(eventCodeParameters);
        var eventsStoreSourceText = BuildStoreSourceCode(eventsStoreCodeParameters);
        var optionSourceText = BuildOptionUtilsSourceCode(rootNamespaceName);
        var someSourceText = BuildSomeUtilsSourceCode(rootNamespaceName);
        var noneSourceText = BuildNoneUtilsSourceCode(rootNamespaceName);
        
        context.AddSource($"Infrastructure/{eventType.Name}Data.g.cs", eventDataSourceText);
        context.AddSource($"Infrastructure/{eventType.Name}EventsStore.g.cs", eventsStoreSourceText);
        
        context.AddSource("Option/Option.g.cs", optionSourceText);
        context.AddSource("Option/Some.g.cs", someSourceText);
        context.AddSource("Option/None.g.cs", noneSourceText);
    }

    private string ReadEmbeddedResource(Assembly assembly, string resourceName)
    {
        var resourceStream = assembly.GetManifestResourceStream(resourceName) ??
                             throw new ArgumentNullException();
        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }

    private Option<EventTypeDetailedResult> SearchEventTypeInTargetClass(
        SemanticModel model,
        TypeDeclarationSyntax classDeclarationSyntax)
    {
        var variablesDeclarations = classDeclarationSyntax.Members
            .Where(x => x is FieldDeclarationSyntax)
            .Cast<FieldDeclarationSyntax>();

        // Navigate class fields
        foreach (var variableDeclarationSyntax in variablesDeclarations)
        {
            // Search member attribute
            var attrSearchResult = variableDeclarationSyntax.AttributeLists.SelectMany(sm => sm.Attributes)
                .FirstOrNone(a => a.Name.IsAttribute<EventBaseTypeTargetAttribute>());
            
            if (attrSearchResult.IsNone())
                continue;

            var attribute = attrSearchResult.Reduce();
            
            var dateTimeEventPropertyName = attribute.ArgumentList!.Arguments[0].Expression.ToString().Trim('"');
            var dbName = attribute.ArgumentList!.Arguments[1].Expression.ToString().Trim('"');
            var tableOrCollectionName = attribute.ArgumentList!.Arguments[2].Expression.ToString().Trim('"');

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
                    return new Some<EventTypeDetailedResult>((
                        TypeSymbol: namedTypeSymbol.TypeArguments.Single(), 
                        DateTimeEventPropertyName: dateTimeEventPropertyName,
                        DbName: dbName,
                        TableOrCollectionName: tableOrCollectionName));
                }
            }
        }

        return new None<EventTypeDetailedResult>();
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

    private static SourceText BuildEventEntitySourceCode(EventCodeParameters codeParameters)
    {
        var sourceCode = new StringBuilder();

        var eventEntityName = $"{codeParameters.TypeBaseName}Data";

        // Declaration
        
        sourceCode.AppendLine($"using {codeParameters.TypeNamespace};");
        sourceCode.AppendLine("using MongoDB.Bson.Serialization.Attributes;");
        sourceCode.AppendLine("using MongoDB.Bson.Serialization.IdGenerators;");
        sourceCode.AppendLine();
        sourceCode.AppendLine($"namespace {codeParameters.RootNamespace}.Infrastructure");
        sourceCode.AppendLine("{");
        sourceCode.AppendLine($"\tpublic sealed class {eventEntityName}");
        sourceCode.AppendLine("\t{");
        
        // Type conversion generations

        sourceCode.AppendLine($"\t\tpublic static implicit operator {eventEntityName}({codeParameters.TypeBaseName} @event) =>");
        sourceCode.AppendLine($"\t\t\tnew {eventEntityName}");
        sourceCode.AppendLine("\t\t\t{");

        foreach (var (_, pName) in codeParameters.Properties)
            sourceCode.AppendLine($"\t\t\t\t{pName} = @event.{pName},");
        
        sourceCode.AppendLine("\t\t\t\tTypeName = @event.GetType().Name");
        sourceCode.AppendLine("\t\t\t};");
        sourceCode.AppendLine();

        if (codeParameters.TypeNames.Any())
        {
            sourceCode.AppendLine($"\t\tpublic static implicit operator {codeParameters.TypeBaseName}({eventEntityName} data) =>");
            sourceCode.AppendLine("\t\t\tdata.TypeName switch");
            sourceCode.AppendLine("\t\t\t{");
            
            foreach (var typeName in codeParameters.TypeNames)
            {
                sourceCode.AppendLine($"\t\t\t\t\"{typeName}\" => new {typeName}");
                sourceCode.AppendLine("\t\t\t\t{");
                foreach (var (_, pName) in codeParameters.Properties)
                    sourceCode.AppendLine($"\t\t\t\t\t{pName} = data.{pName},");
                sourceCode.AppendLine("\t\t\t\t},");
            }

            sourceCode.AppendLine("\t\t\t\t_ => throw new ArgumentOutOfRangeException(data.TypeName)");
            sourceCode.AppendLine("\t\t\t};");
            sourceCode.AppendLine();
        }
        else
        {
            sourceCode.AppendLine($"\t\tpublic static implicit operator {codeParameters.TypeBaseName}({eventEntityName} @event) =>");
            sourceCode.AppendLine($"\t\t\tnew {codeParameters.TypeBaseName}");
            sourceCode.AppendLine("\t\t\t{");
            foreach (var (_, pName) in codeParameters.Properties)
                sourceCode.AppendLine($"\t\t\t\t{pName} = @event.{pName},");
            sourceCode.AppendLine("\t\t\t};");
            sourceCode.AppendLine();
        }
        
        // Properties

        const string accessors = "{ get; set; }";
        foreach (var (pType, pName) in codeParameters.Properties)
        {
            if (string.Compare("Id", pName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                switch (pType)
                {
                    case "Guid":
                        sourceCode.AppendLine("\t\t[BsonId(IdGenerator = typeof(GuidGenerator))]");
                        break;
                    case "int":
                        sourceCode.AppendLine("\t\t[BsonId]");
                        break;
                    case "string":
                        sourceCode.AppendLine("\t\t[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]");
                        break;
                    default:
                        throw new ApplicationException($"Unsupported Id of type: {pType}");
                }
            }
            
            sourceCode.AppendLine($"\t\tpublic {pType} {pName} {accessors}");
        }
        
        sourceCode.AppendLine("\t\tpublic string TypeName { get; set; }");
        
        sourceCode.AppendLine("\t}");
        sourceCode.AppendLine("}");

        return SourceText.From(sourceCode.ToString(), Encoding.UTF8);
    }

    public static SourceText BuildOptionUtilsSourceCode(string rootNamespace)
    {
        const string sourceCode = """
                                  namespace {rootNamespace}.Option;

                                  public interface IOption<out T>
                                  {
                                      T Reduce();
                                      bool IsNone();
                                  }

                                  public abstract class Option<T> : IOption<T>
                                  {
                                      public static implicit operator Option<T>(T value) => new Some<T>(value);
                                      public static implicit operator Option<T>(None _) => new None<T>();
                                  
                                      public abstract T Reduce();
                                      public abstract bool IsNone();
                                      
                                      public IOption<T> AsOption() => this;
                                  }
                                  """;

        return SourceText.From(sourceCode.Replace("{rootNamespace}", rootNamespace), Encoding.UTF8); 
    }

    public static SourceText BuildSomeUtilsSourceCode(string rootNamespace)
    {
        const string sourceCode = """
                                  namespace {rootNamespace}.Option;
                                  
                                  public class Some<T> : Option<T>
                                  {
                                      public Some(T item)
                                      {
                                          Item = item;
                                      }
                                      
                                      public T Item { get; }
                                      
                                      public static implicit operator T(Some<T> some) =>
                                          some.Item;
                                  
                                      public override T Reduce() => Item;
                                      public override bool IsNone() => false;
                                  }
                                  """;

        return SourceText.From(sourceCode.Replace("{rootNamespace}", rootNamespace), Encoding.UTF8);  
    }

    public static SourceText BuildNoneUtilsSourceCode(string rootNamespace)
    {
        const string sourceCode = """
                                  namespace {rootNamespace}.Option;
                                  
                                  public class None<T> : Option<T>
                                  {
                                      public override T Reduce() => default;
                                      public override bool IsNone() => true;
                                  }
                                  
                                  public sealed class None
                                  {
                                      private static None Value { get; } = new();
                                      
                                      private None() { }
                                  }
                                  """;

        return SourceText.From(sourceCode.Replace("{rootNamespace}", rootNamespace), Encoding.UTF8);   
    }

    private static SourceText BuildStoreSourceCode(EventsStoreCodeParameter codeParameters)
    {
        var sourceCode = new StringBuilder();

        var eventEntityName = $"{codeParameters.EventClassName}Data";

        var generatedFromTemplate = codeParameters.Template
            .Replace("{{RootNamespace}}", codeParameters.RootNamespace)
            .Replace("{{ClassName}}", codeParameters.ClassName)
            .Replace("{{EventClassName}}", eventEntityName)
            .Replace("{{DatabaseName}}", codeParameters.DatabaseName)
            .Replace("{{DateTimeEventPropertyName}}", codeParameters.DateTimeEventPropertyName)
            .Replace("{{TableOrCollectionName}}", codeParameters.TableOrCollectionName);
        
        sourceCode.Append(generatedFromTemplate);
        
        return SourceText.From(sourceCode.ToString(), Encoding.UTF8);
    }
}