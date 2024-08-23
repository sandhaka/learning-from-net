using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record PhysicalPersonId(Guid Value) : StrongTypedId(Value);

public sealed class PhysicalPerson
{
    public required PhysicalPersonId PersonId { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Vat { get; init; }
}