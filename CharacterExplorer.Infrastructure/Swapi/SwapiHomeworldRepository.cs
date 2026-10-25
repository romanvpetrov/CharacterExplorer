using System.Text.Json.Serialization;
using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Infrastructure.Swapi;

internal sealed class WorldJson
{
    public string Name { get; set; } = "";
    public string Population { get; set; } = "unknown";
    public string Terrain { get; set; } = "unknown";
    public string Climate { get; set; } = "unknown";
    [JsonPropertyName("surface_water")] public string SurfaceWater { get; set; } = "unknown";
    public string Diameter { get; set; } = "unknown";
    [JsonPropertyName("rotation_period")] public string RotationPeriod { get; set; } = "unknown";
    [JsonPropertyName("orbital_period")] public string OrbitalPeriod { get; set; } = "unknown";
    public string Gravity { get; set; } = "unknown";
}

public sealed class SwapiHomeworldRepository(SwapiClient client) : IHomeworldRepository
{
    public async Task<Homeworld?> GetByIdAsync(int id)
    {
        var world = await client.GetAsync<WorldJson>($"planets/{id}");
        return world is null ? null : new(world.Name, world.Population, world.Terrain, world.Climate,
            world.SurfaceWater, world.Diameter, world.RotationPeriod, world.OrbitalPeriod, world.Gravity);
    }
}
