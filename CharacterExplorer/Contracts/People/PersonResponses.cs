namespace CharacterExplorer.Contracts.People;

public sealed record PersonSummaryResponse(int Id, string Name);

public sealed record SpeciesResponse(
    string Name,
    string Classification,
    string Designation,
    string AverageHeight,
    string AverageLifespan,
    string Language
);

public sealed record HomeworldResponse(
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

public sealed record FilmResponse(int Id, string Title, int EpisodeId, string ReleaseDate, bool Appears);

public sealed record VehicleResponse(
    int Id,
    string Name,
    string Model,
    string VehicleClass,
    string Manufacturer,
    string Crew,
    string Passengers
);

public sealed record StarshipResponse(
    int Id,
    string Name,
    string Model,
    string StarshipClass,
    string Manufacturer,
    string Crew,
    string Passengers
);

public sealed record PersonDetailsResponse(
    int Id,
    string Name,
    string Height,
    string Mass,
    string HairColor,
    string SkinColor,
    string EyeColor,
    string BirthYear,
    string Gender,
    SpeciesResponse? Species,
    HomeworldResponse? Homeworld,
    IReadOnlyList<FilmResponse> Films,
    IReadOnlyList<VehicleResponse> Vehicles,
    IReadOnlyList<StarshipResponse> Starships
);
