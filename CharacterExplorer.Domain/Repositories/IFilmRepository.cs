using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface IFilmRepository
{
    Task<Film?> GetByIdAsync(int id);
}
