using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record UserId(Guid Value) : StrongTypedId(Value);

public sealed class User
{
    public required UserId UserId { get; init; }
    public required string Email { get; init; }
    public required PhysicalPersonId PhysicalPersonId { get; init; }
}