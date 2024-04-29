using System.Collections.Generic;

namespace EventSourcingSourceGenerator.Dto;

internal record EventCodeParameters
{
    public string RootNamespace { get; set; }
    public string TypeNamespace { get; set; }
    public string TypeBaseName { get; set; }

    public IEnumerable<(string Type, string Name)> Properties { get; set; }

    /// <summary>
    /// Names of the concrete event types
    /// </summary>
    public IEnumerable<string> TypeNames { get; set; }
}