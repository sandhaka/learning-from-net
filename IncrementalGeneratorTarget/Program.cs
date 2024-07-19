
/*
 * Suppose to want to automate mapping from domain objects and plain entity classes
 * You can imagine this example about different mapping use cases like mapping from domain to dto or view models.
 */

using IncrementalGeneratorTarget.Dal;
using IncrementalGeneratorTarget.DomainModels;

var book = new SampleDomainClassOfBook
{
 Title = "Gelo",
 AuthorName = "Thomas Bernhard",
 EditionNumber = 9,
 Isbn = "978-88-459-3800-9",
 PagesNumber = 356,
 Publisher = "ADELPHY"
};

Console.WriteLine(book.DescribeFull());

// var bookEntity = BookEntity.From(book);