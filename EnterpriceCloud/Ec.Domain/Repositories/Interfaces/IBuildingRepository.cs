using Ec.Domain.Models;
using Monads.Optional;

namespace Ec.Domain.Repositories.Interfaces;

public interface IBuildingRepository
{
    Option<Building> GetBuilding(BuildingId buildingId);
}