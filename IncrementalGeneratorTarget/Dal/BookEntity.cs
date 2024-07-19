namespace IncrementalGeneratorTarget.Dal;

public sealed partial class BookEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string Isbn { get; set; }
    public int PagesNumber { get; set; }
    public int EditionNumber { get; set; }
    public string Publisher { get; set; }
}