using System.Text.Json.Serialization;
using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Infrastructure.Swapi;

internal sealed class SpeciesJson
{
    public string Name { get; set; } = "";
    public string Classification { get; set; } = "unknown";
    public string Designation { get; set; } = "unknown";
    [JsonPropertyName("average_height")] public string AverageHeight { get; set; } = "unknown";
    [JsonPropertyName("average_lifespan")] public string AverageLifespan { get; set; } = "unknown";
    public string Language { get; set; } = "unknown";
}

public sealed class SwapiSpeciesRepository(SwapiClient client) : ISpeciesRepository
{
    public async Task<Species?> GetByIdAsync(int id)
    {
        var species = await client.GetAsync<SpeciesJson>($"species/{id}");
        return species is null ? null : new Species(
            id,
            species.Name,
            species.Classification,
            species.Designation,
            species.AverageHeight,
            species.AverageLifespan,
            species.Language);
    }
}
