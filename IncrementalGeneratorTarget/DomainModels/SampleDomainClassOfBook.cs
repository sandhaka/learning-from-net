using GeneratedNamespace;
using IncrementalGeneratorTarget.Dal;

namespace IncrementalGeneratorTarget.DomainModels;

[MapTo(typeof(BookEntity))]
public sealed class SampleDomainClassOfBook
{
    public required string Title { get; init; }
    public required string AuthorName { get; init; }
    public required string Isbn { get; init; }
    public required int PagesNumber { get; init; }
    public required int EditionNumber { get; init; }
    public required string Publisher { get; init; }

    public string Describe() => $"{Title} ({AuthorName})";

    public string DescribeFull() =>
        $"{Title} ({AuthorName}), Publisher: {Publisher}, Edition: {EditionNumber}, Pages: {PagesNumber}";
}