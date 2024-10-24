using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public record UserRecord(Guid Id, string Email, Guid PhysicalPersonId)
{
    public static implicit operator User(UserRecord record) =>
        new()
        {
            UserId = new UserId(record.Id),
            Email = record.Email,
            PhysicalPersonId = new PhysicalPersonId(record.PhysicalPersonId)
        };

    public static implicit operator UserRecord(User user) =>
        new(user.UserId.Value, user.Email, user.PhysicalPersonId.Value);
}