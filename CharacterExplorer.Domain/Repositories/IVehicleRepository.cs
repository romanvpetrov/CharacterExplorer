using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);
}
