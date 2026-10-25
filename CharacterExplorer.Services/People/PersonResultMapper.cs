using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Services.People;

internal static class PersonResultMapper
{
    public static PersonSummaryResult ToSummary(
        PersonSummary person
    ) =>
        new(person.Id, person.Name);

    public static GetPersonResult ToGetResult(
        Person person
    ) => GetPersonResult.Succeeded(new PersonDetailsResult(
        person.Id,
        person.Name,
        person.Height,
        person.Mass,
        person.HairColor,
        person.SkinColor,
        person.EyeColor,
        person.BirthYear,
        person.Gender,
        ToResult(person.Species),
        ToResult(person.Homeworld),
        person.Films.Select(ToResult).ToArray(),
        person.Vehicles.Select(ToResult).ToArray(),
        person.Starships.Select(ToResult).ToArray()));

    private static SpeciesResult? ToResult(Species? value) => value is null
        ? null
        : new(
            value.Name,
            value.Classification,
            value.Designation,
            value.AverageHeight,
            value.AverageLifespan,
            value.Language);

    private static HomeworldResult? ToResult(Homeworld? value) => value is null
        ? null
        : new(
            value.Name,
            value.Population,
            value.Terrain,
            value.Climate,
            value.SurfaceWater,
            value.Diameter,
            value.RotationPeriod,
            value.OrbitalPeriod,
            value.Gravity);

    private static FilmResult ToResult(Film value) =>
        new(
            value.Id,
            value.Title,
            value.EpisodeId,
            value.ReleaseDate,
            value.Appears);

    private static VehicleResult ToResult(Vehicle value) =>
        new(
            value.Id,
            value.Name,
            value.Model,
            value.VehicleClass,
            value.Manufacturer,
            value.Crew,
            value.Passengers);

    private static StarshipResult ToResult(Starship value) =>
        new(
            value.Id,
            value.Name,
            value.Model,
            value.StarshipClass,
            value.Manufacturer,
            value.Crew,
            value.Passengers);
}
