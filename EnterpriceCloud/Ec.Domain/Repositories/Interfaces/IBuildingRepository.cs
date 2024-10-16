using Ec.Domain.Models;

namespace Ec.Domain.Repositories.Interfaces;

public interface IBuildingRepository
{
    Building GetBuilding(BuildingId buildingId);
}