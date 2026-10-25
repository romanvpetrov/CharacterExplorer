namespace CharacterExplorer.Services.People;

public sealed record PersonSearchResult(
    bool Success,
    IReadOnlyList<PersonSummaryResult> People,
    string? FailureCode,
    string? Message)
{
    public static PersonSearchResult Succeeded(IReadOnlyList<PersonSummaryResult> people) =>
        new(true, people, null, null);

    public static PersonSearchResult Failed(string failureCode, string message) =>
        new(false, [], failureCode, message);
}

public sealed record GetPersonResult(
    bool Success,
    PersonDetailsResult? Person,
    string? FailureCode,
    string? Message)
{
    public static GetPersonResult Succeeded(PersonDetailsResult person) =>
        new(true, person, null, null);

    public static GetPersonResult Failed(string failureCode, string message) =>
        new(false, null, failureCode, message);
}

public sealed record PersonSummaryResult(int Id, string Name);

public sealed record SpeciesResult(
    string Name,
    string Classification,
    string Designation,
    string AverageHeight,
    string AverageLifespan,
    string Language
);

public sealed record HomeworldResult(
    string Name,
    string Population,
    string Terrain,
    string Climate,
    string SurfaceWater,
    string Diameter,
    string RotationPeriod,
    string OrbitalPeriod,
    string Gravity
);

public sealed record FilmResult(int Id, string Title, int EpisodeId, string ReleaseDate, bool Appears);

public sealed record VehicleResult(
    int Id,
    string Name,
    string Model,
    string VehicleClass,
    string Manufacturer,
    string Crew,
    string Passengers
);

public sealed record StarshipResult(
    int Id,
    string Name,
    string Model,
    string StarshipClass,
    string Manufacturer,
    string Crew,
    string Passengers
);

public sealed record PersonDetailsResult(
    int Id,
    string Name,
    string Height,
    string Mass,
    string HairColor,
    string SkinColor,
    string EyeColor,
    string BirthYear,
    string Gender,
    SpeciesResult? Species,
    HomeworldResult? Homeworld,
    IReadOnlyList<FilmResult> Films,
    IReadOnlyList<VehicleResult> Vehicles,
    IReadOnlyList<StarshipResult> Starships
);
