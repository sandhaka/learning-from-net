using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace EventSourcingSourceGeneratorTarget.Models;

public sealed class Port
{
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions { WriteIndented = true };
    
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    [SetsRequiredMembers]
    public Port(string name)
    {
        Id = Guid.Empty;
        Name = name;
    }

    [SetsRequiredMembers]
    public Port(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not Port objPort)
            return false;

        if (objPort.Name == Name)
            return true;

        return false;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() =>
        JsonSerializer.Serialize(this, _jsonSerializerOptions);
}