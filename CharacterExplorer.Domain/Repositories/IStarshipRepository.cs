using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface IStarshipRepository
{
    Task<Starship?> GetByIdAsync(int id);
}
