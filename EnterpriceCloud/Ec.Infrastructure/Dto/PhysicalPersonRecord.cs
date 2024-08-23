using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public sealed record PhysicalPersonRecord(Guid Id, string Name, string Surname, string Vat)
{
    public static implicit operator PhysicalPerson(PhysicalPersonRecord record) => 
        new PhysicalPerson
        {
            PersonId = new PhysicalPersonId(record.Id),
            Name = record.Name,
            Surname = record.Surname,
            Vat = record.Vat
        };
    
    public static implicit operator PhysicalPersonRecord(PhysicalPerson domainObject) =>
        new PhysicalPersonRecord(domainObject.PersonId.Value, domainObject.Name, domainObject.Surname, domainObject.Vat);
}