namespace CharacterExplorer.Domain.Models;

public sealed record PersonSummary(int Id, string Name);

public sealed class Person
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public Height? Height { get; init; }
    public Mass? Mass { get; init; }
    public string HairColor { get; init; } = "unknown";
    public string SkinColor { get; init; } = "unknown";
    public string EyeColor { get; init; } = "unknown";
    public string BirthYear { get; init; } = "unknown";
    public string Gender { get; init; } = "unknown";
    public int? SpeciesId { get; init; }
    public int? HomeworldId { get; init; }
    public IReadOnlyList<int> FilmIds { get; init; } = [];
    public IReadOnlyList<int> VehicleIds { get; init; } = [];
    public IReadOnlyList<int> StarshipIds { get; init; } = [];
    public Species? Species { get; set; }
    public Homeworld? Homeworld { get; set; }
    public IReadOnlyList<Film> Films { get; set; } = [];
    public IReadOnlyList<Vehicle> Vehicles { get; set; } = [];
    public IReadOnlyList<Starship> Starships { get; set; } = [];
}

public sealed record Species(
    int Id,
    string Name,
    string Classification,
    string Designation,
    Height? AverageHeight,
    Years? AverageLifespan,
    string Language
);

public sealed record Homeworld(
    int Id,
    string Name,
    long? Population,
    string Terrain,
    string Climate,
    Percentage? SurfaceWater,
    Distance? Diameter,
    Days? RotationPeriod,
    Days? OrbitalPeriod,
    string Gravity
);

public sealed record Film(int Id, string Title, int EpisodeId, string ReleaseDate);

public sealed record Vehicle(
    int Id,
    string Name,
    string Model,
    string VehicleClass,
    string Manufacturer,
    string Crew,
    string Passengers
);

public sealed record Starship(
    int Id,
    string Name,
    string Model,
    string StarshipClass,
    string Manufacturer,
    string Crew,
    string Passengers
);
