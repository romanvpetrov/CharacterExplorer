using CharacterExplorer.Services.People;

namespace CharacterExplorer.Contracts.People;

internal static class PersonResponseMapper
{
    public static PersonSummaryResponse ToResponse(PersonSummaryResult person) =>
        new(person.Id, person.Name);

    public static PersonDetailsResponse ToResponse(PersonDetailsResult person) => new(
        person.Id, person.Name, person.Height, person.Mass, person.HairColor,
        person.SkinColor, person.EyeColor, person.BirthYear, person.Gender,
        ToResponse(person.Species), ToResponse(person.Homeworld),
        person.Films.Select(ToResponse).ToArray(),
        person.Vehicles.Select(ToResponse).ToArray(),
        person.Starships.Select(ToResponse).ToArray());

    private static SpeciesResponse? ToResponse(SpeciesResult? value) => value is null
        ? null
        : new(value.Name, value.Classification, value.Designation,
            value.AverageHeight, value.AverageLifespan, value.Language);

    private static HomeworldResponse? ToResponse(HomeworldResult? value) => value is null
        ? null
        : new(value.Name, value.Population, value.Terrain, value.Climate,
            value.SurfaceWater, value.Diameter, value.RotationPeriod,
            value.OrbitalPeriod, value.Gravity);

    private static FilmResponse ToResponse(FilmResult value) =>
        new(value.Id, value.Title, value.EpisodeId, value.ReleaseDate, value.Appears);

    private static VehicleResponse ToResponse(VehicleResult value) =>
        new(value.Id, value.Name, value.Model, value.VehicleClass,
            value.Manufacturer, value.Crew, value.Passengers);

    private static StarshipResponse ToResponse(StarshipResult value) =>
        new(value.Id, value.Name, value.Model, value.StarshipClass,
            value.Manufacturer, value.Crew, value.Passengers);
}
