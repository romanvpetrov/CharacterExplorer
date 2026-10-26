using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace CharacterExplorer.Infrastructure.Swapi;

internal sealed class PersonJson
{
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
    public string Height { get; set; } = "unknown";
    public string Mass { get; set; } = "unknown";
    [JsonPropertyName("hair_color")] public string HairColor { get; set; } = "unknown";
    [JsonPropertyName("skin_color")] public string SkinColor { get; set; } = "unknown";
    [JsonPropertyName("eye_color")] public string EyeColor { get; set; } = "unknown";
    [JsonPropertyName("birth_year")] public string BirthYear { get; set; } = "unknown";
    public string Gender { get; set; } = "unknown";
    public string Homeworld { get; set; } = "";
    public List<string> Films { get; set; } = [];
    public List<string> Species { get; set; } = [];
    public List<string> Vehicles { get; set; } = [];
    public List<string> Starships { get; set; } = [];
}

public sealed class SwapiPersonRepository(SwapiClient client) : IPersonRepository
{
    private IReadOnlyList<PersonJson>? cache;

    public async Task<IReadOnlyList<PersonSummary>> GetAllAsync()
    {
        cache ??= await client.GetAsync<List<PersonJson>>("people") ?? [];
        return cache.Select(p => new PersonSummary(SwapiLink.Id(p.Url) ?? 0, p.Name))
            .OrderBy(p => p.Id).ToArray();
    }

    public async Task<Person?> GetByIdAsync(int id)
    {
        var p = await client.GetAsync<PersonJson>($"people/{id}");
        if (p is null) return null;
        return new Person
        {
            Id = SwapiLink.Id(p.Url) ?? id, Name = p.Name,
            Height = MeasurementParser.Height(p.Height), Mass = MeasurementParser.Mass(p.Mass),
            HairColor = p.HairColor, SkinColor = p.SkinColor, EyeColor = p.EyeColor,
            BirthYear = p.BirthYear, Gender = p.Gender, HomeworldId = SwapiLink.Id(p.Homeworld),
            SpeciesId = p.Species.Select(SwapiLink.Id).FirstOrDefault(x => x.HasValue),
            FilmIds = p.Films.Select(SwapiLink.Id).Where(x => x.HasValue).Select(x => x!.Value).ToArray(),
            VehicleIds = p.Vehicles.Select(SwapiLink.Id).Where(x => x.HasValue).Select(x => x!.Value).ToArray(),
            StarshipIds = p.Starships.Select(SwapiLink.Id).Where(x => x.HasValue).Select(x => x!.Value).ToArray(),
        };
    }
}
