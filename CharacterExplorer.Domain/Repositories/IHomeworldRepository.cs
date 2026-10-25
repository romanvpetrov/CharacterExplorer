using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface IHomeworldRepository
{
    Task<Homeworld?> GetByIdAsync(int id);
}
