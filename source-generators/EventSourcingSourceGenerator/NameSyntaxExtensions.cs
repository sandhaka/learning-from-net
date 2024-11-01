using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventSourcingSourceGenerator;

internal static class NameSyntaxExtensions
{
    public static bool IsAttribute<T> (this NameSyntax nameSyntax)
    {
        var aName = nameSyntax.ToString();
        
        return aName.EndsWith("Attribute")
            ? aName.Equals(typeof(T).Name)
            : $"{aName}Attribute".Equals(typeof(T).Name);
    }
}