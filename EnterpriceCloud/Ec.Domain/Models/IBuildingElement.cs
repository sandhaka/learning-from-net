namespace Ec.Domain.Models;

public interface IBuildingElement
{
    Location Location { get; }
    string Description { get; }
}