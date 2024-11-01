namespace EventSourcingSourceGeneratorTarget.Models;

public sealed record Article
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required float Weight { get; init; }
}