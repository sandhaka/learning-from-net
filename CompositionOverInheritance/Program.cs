// ReSharper disable UnusedType.Global

// A new book
var book = new Book
{
    Author = "John Doe",
    Title = "The Great Book",
    NumberOfPages = 300
};

var pub = new Publishing();
// Publishing the book
var bookPublishingPlanned = pub.Schedule(book, DateTime.Now.AddDays(7).ToDateOnly());

Console.WriteLine(bookPublishingPlanned);


/*
 * Try to model a book as best and avoiding:
 *
 * # Class derivation misusing
 * # Static method abusing
 * # Using boolean flags
 */

internal class Book
{
    public required string Author { get; init; }
    public required string Title { get; init; }
    public required int NumberOfPages { get; init; }

    // # Modelling publishing state as separate concept
    public PublishingInfo Publication { get; init; } = new NotYetPublished();

    public override string ToString() => $"{Author} {Title} {Publication}";
}

internal class Publishing(string publisher = "John Doe")
{
    // # Preserve immutable design - Book class can be immutable
    public Book Schedule(Book book, DateOnly date) =>
        new Book
        {
            Author = book.Author, Title = book.Title, NumberOfPages = book.NumberOfPages,
            Publication = new PublishingPlanned(publisher, date)
        };
}

internal record PublishingInfo();

internal record NotYetPublished : PublishingInfo;

internal record Published(string Publisher, DateOnly OnDate) : PublishingInfo;

internal record PublishingPlanned(string Publisher, DateOnly PlannedDate) : PublishingInfo;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime date) =>
        new DateOnly(date.Year, date.Month, date.Day);
}