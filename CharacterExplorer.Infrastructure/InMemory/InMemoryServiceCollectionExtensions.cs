using CharacterExplorer.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterExplorer.Infrastructure.InMemory;

public static class InMemoryServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryRepositories(
        this IServiceCollection services,
        InMemoryRepositorySet? repositories = null)
    {
        var set = repositories ?? InMemoryRepositorySet.CreateDefault();

        services.AddSingleton<IPersonRepository>(set.People);
        services.AddSingleton<ISpeciesRepository>(set.Species);
        services.AddSingleton<IHomeworldRepository>(set.Homeworlds);
        services.AddSingleton<IFilmRepository>(set.Films);
        services.AddSingleton<IVehicleRepository>(set.Vehicles);
        services.AddSingleton<IStarshipRepository>(set.Starships);

        return services;
    }
}
