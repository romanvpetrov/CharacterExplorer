using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface IFilmRepository
{
    Task<IEnumerable<Film>> GetAllAsync();
    Task<Film?> GetByIdAsync(int id);
}
