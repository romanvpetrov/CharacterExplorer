using CharacterExplorer.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterExplorer.Infrastructure.Swapi;

public static class SwapiServiceCollectionExtensions
{
    public static IServiceCollection AddSwapiRepositories(this IServiceCollection services)
    {
        services.AddHttpClient<SwapiClient>(client =>
        {
            client.BaseAddress = new Uri("https://swapi.info/api/");
        });

        services.AddTransient<IPersonRepository, SwapiPersonRepository>();
        services.AddTransient<ISpeciesRepository, SwapiSpeciesRepository>();
        services.AddTransient<IHomeworldRepository, SwapiHomeworldRepository>();
        services.AddTransient<IFilmRepository, SwapiFilmRepository>();
        services.AddTransient<IVehicleRepository, SwapiVehicleRepository>();
        services.AddTransient<IStarshipRepository, SwapiStarshipRepository>();

        return services;
    }
}
