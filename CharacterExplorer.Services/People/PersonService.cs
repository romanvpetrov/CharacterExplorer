using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Services.People;

public interface IPersonService
{
    Task<PersonSearchResult> SearchAsync(
        string? query
    );
    Task<GetPersonResult> GetByIdAsync(
        int id
    );
}

public sealed class PersonService(
    IPersonRepository people,
    ISpeciesRepository speciesRepository,
    IHomeworldRepository homeworldRepository,
    IFilmRepository filmsRepository,
    IVehicleRepository vehiclesRepository,
    IStarshipRepository starshipsRepository
) : IPersonService
{
    public async Task<PersonSearchResult> SearchAsync(
        string? query
    )
    {
        try
        {
            var all = await people.GetAllAsync();
            var matchingPeople = string.IsNullOrWhiteSpace(query)
                ? all
                : all.Where(person => person.Name.Contains(
                    query.Trim(), StringComparison.OrdinalIgnoreCase));

            return PersonSearchResult.Succeeded(
                matchingPeople.Select(PersonResultMapper.ToSummary).ToArray());
        }
        catch (Exception)
        {
            return PersonSearchResult.Failed(
                "PeopleLookupFailed",
                "Unable to retrieve people.");
        }
    }

    public async Task<GetPersonResult> GetByIdAsync(int id)
    {
        try
        {
            var person = await people.GetByIdAsync(id);

            if (person is null)
                return GetPersonResult.Failed("PersonNotFound", $"No person exists with id {id}.");

            var speciesTask = person.SpeciesId is int speciesId
                ? speciesRepository.GetByIdAsync(speciesId)
                : Task.FromResult<Species?>(null);

            var homeworldTask = person.HomeworldId is int homeworldId
                ? homeworldRepository.GetByIdAsync(homeworldId)
                : Task.FromResult<Homeworld?>(null);

            var allFilms = await filmsRepository.GetAllAsync();

            var vehicleTasks = person.VehicleIds.Select(v =>
                vehiclesRepository.GetByIdAsync(v)
            );

            var starshipTasks = person.StarshipIds.Select(s =>
                starshipsRepository.GetByIdAsync(s)
            );

            var species = await speciesTask;
            var homeworld = await homeworldTask;

            var vehicles = await Task.WhenAll(vehicleTasks);
            var starships = await Task.WhenAll(starshipTasks);

            person.Species = species;
            person.Homeworld = homeworld;
            person.Vehicles = vehicles.OfType<Vehicle>().ToArray();
            person.Starships = starships.OfType<Starship>().ToArray();

            return PersonResultMapper.ToGetResult(person, allFilms);
        }
        catch (Exception)
        {
            return GetPersonResult.Failed(
                "PersonLookupFailed",
                $"Unable to retrieve person details for id {id}.");
        }
    }

}
