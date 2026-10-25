using System.Text.Json.Serialization;
using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Infrastructure.Swapi;

internal sealed class StarshipJson
{
    public string Name { get; set; } = "";
    public string Model { get; set; } = "unknown";
    [JsonPropertyName("starship_class")] public string StarshipClass { get; set; } = "unknown";
    public string Manufacturer { get; set; } = "unknown";
    public string Crew { get; set; } = "unknown";
    public string Passengers { get; set; } = "unknown";
}

public sealed class SwapiStarshipRepository(SwapiClient client) : IStarshipRepository
{
    public async Task<Starship?> GetByIdAsync(int id)
    {
        var starship = await client.GetAsync<StarshipJson>($"starships/{id}");
        return starship is null ? null : new(id, starship.Name, starship.Model, starship.StarshipClass,
            starship.Manufacturer, starship.Crew, starship.Passengers);
    }
}
