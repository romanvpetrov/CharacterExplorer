using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Domain.Repositories;

public interface IPersonRepository
{
    Task<IReadOnlyList<PersonSummary>> GetAllAsync();
    Task<Person?> GetByIdAsync(int id);
}
