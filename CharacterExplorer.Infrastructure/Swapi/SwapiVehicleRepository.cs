using System.Text.Json.Serialization;
using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Infrastructure.Swapi;

internal sealed class VehicleJson
{
    public string Name { get; set; } = "";
    public string Model { get; set; } = "unknown";
    [JsonPropertyName("vehicle_class")] public string VehicleClass { get; set; } = "unknown";
    public string Manufacturer { get; set; } = "unknown";
    public string Crew { get; set; } = "unknown";
    public string Passengers { get; set; } = "unknown";
}

public sealed class SwapiVehicleRepository(SwapiClient client) : IVehicleRepository
{
    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        var vehicle = await client.GetAsync<VehicleJson>($"vehicles/{id}");
        return vehicle is null ? null : new(id, vehicle.Name, vehicle.Model, vehicle.VehicleClass,
            vehicle.Manufacturer, vehicle.Crew, vehicle.Passengers);
    }
}
