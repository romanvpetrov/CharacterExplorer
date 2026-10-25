using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface ISpeciesRepository
{
    Task<Species?> GetByIdAsync(int id);
}
