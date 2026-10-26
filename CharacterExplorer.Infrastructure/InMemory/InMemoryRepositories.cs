using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Infrastructure.InMemory;

public sealed class InMemoryPersonRepository : IPersonRepository
{
    private readonly IReadOnlyList<Person> people;

    public InMemoryPersonRepository(IEnumerable<Person> people)
    {
        this.people = people.ToArray();
    }

    public Task<IReadOnlyList<PersonSummary>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<PersonSummary>>(
            people.Select(person => new PersonSummary(person.Id, person.Name)).ToArray());

    public Task<Person?> GetByIdAsync(int id)
    {
        var person = people.FirstOrDefault(person => person.Id == id);
        return Task.FromResult(person is null ? null : Copy(person));
    }

    private static Person Copy(Person person) => new()
    {
        Id = person.Id,
        Name = person.Name,
        Height = person.Height,
        Mass = person.Mass,
        HairColor = person.HairColor,
        SkinColor = person.SkinColor,
        EyeColor = person.EyeColor,
        BirthYear = person.BirthYear,
        Gender = person.Gender,
        SpeciesId = person.SpeciesId,
        HomeworldId = person.HomeworldId,
        FilmIds = person.FilmIds.ToArray(),
        VehicleIds = person.VehicleIds.ToArray(),
        StarshipIds = person.StarshipIds.ToArray()
    };
}

public sealed class InMemorySpeciesRepository : ISpeciesRepository
{
    private readonly IReadOnlyDictionary<int, Species> values;

    public InMemorySpeciesRepository(IEnumerable<Species> values) =>
        this.values = values.ToDictionary(value => value.Id);

    public Task<Species?> GetByIdAsync(int id) =>
        Task.FromResult(values.GetValueOrDefault(id));
}

public sealed class InMemoryHomeworldRepository : IHomeworldRepository
{
    private readonly IReadOnlyDictionary<int, Homeworld> values;

    public InMemoryHomeworldRepository(IEnumerable<Homeworld> values) =>
        this.values = values.ToDictionary(value => value.Id);

    public Task<Homeworld?> GetByIdAsync(int id) =>
        Task.FromResult(values.GetValueOrDefault(id));
}

public sealed class InMemoryFilmRepository : IFilmRepository
{
    private readonly IReadOnlyDictionary<int, Film> values;

    public InMemoryFilmRepository(IEnumerable<Film> values) =>
        this.values = values.ToDictionary(value => value.Id);

    public Task<IEnumerable<Film>> GetAllAsync()
    {
        return Task.FromResult(values.Values);
    }

    public Task<Film?> GetByIdAsync(int id) =>
        Task.FromResult(values.GetValueOrDefault(id));
}

public sealed class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly IReadOnlyDictionary<int, Vehicle> values;

    public InMemoryVehicleRepository(IEnumerable<Vehicle> values) =>
        this.values = values.ToDictionary(value => value.Id);

    public Task<Vehicle?> GetByIdAsync(int id) =>
        Task.FromResult(values.GetValueOrDefault(id));
}

public sealed class InMemoryStarshipRepository : IStarshipRepository
{
    private readonly IReadOnlyDictionary<int, Starship> values;

    public InMemoryStarshipRepository(IEnumerable<Starship> values) =>
        this.values = values.ToDictionary(value => value.Id);

    public Task<Starship?> GetByIdAsync(int id) =>
        Task.FromResult(values.GetValueOrDefault(id));
}
