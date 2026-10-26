using System.Globalization;
using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Services.People;

internal static class PersonResultMapper
{
    public static PersonSummaryResult ToSummary(
        PersonSummary person
    ) =>
        new(person.Id, person.Name);

    public static GetPersonResult ToGetResult(
        Person person,
        IEnumerable<Film> allFilms
    ) => GetPersonResult.Succeeded(new PersonDetailsResult(
        person.Id,
        person.Name,
        Format(person.Height),
        Format(person.Mass),
        person.HairColor,
        person.SkinColor,
        person.EyeColor,
        person.BirthYear,
        person.Gender,
        ToResult(person.Species),
        ToResult(person.Homeworld),
        allFilms.Select(film => ToResult(film, person.FilmIds.Contains(film.Id))).ToArray(),
        person.Vehicles.Select(ToResult).ToArray(),
        person.Starships.Select(ToResult).ToArray()));

    private static SpeciesResult? ToResult(Species? value) => value is null
        ? null
        : new(
            value.Name,
            value.Classification,
            value.Designation,
            Format(value.AverageHeight),
            Format(value.AverageLifespan),
            value.Language);

    private static HomeworldResult? ToResult(Homeworld? value) => value is null
        ? null
        : new(
            value.Name,
            value.Population?.ToString("N0", CultureInfo.InvariantCulture) ?? "unknown",
            value.Terrain,
            value.Climate,
            Format(value.SurfaceWater),
            Format(value.Diameter),
            Format(value.RotationPeriod),
            Format(value.OrbitalPeriod),
            value.Gravity);

    private static FilmResult ToResult(Film value, bool appears) =>
        new(
            value.Id,
            value.Title,
            value.EpisodeId,
            value.ReleaseDate,
            appears);

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

    private static string Format<T>(T? value) where T : struct =>
        value?.ToString() ?? "unknown";
}
