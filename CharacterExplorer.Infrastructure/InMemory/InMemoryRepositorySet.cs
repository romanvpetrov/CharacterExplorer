using CharacterExplorer.Domain.Models;

namespace CharacterExplorer.Infrastructure.InMemory;

public sealed class InMemoryRepositorySet
{
    public InMemoryPersonRepository People { get; }
    public InMemorySpeciesRepository Species { get; }
    public InMemoryHomeworldRepository Homeworlds { get; }
    public InMemoryFilmRepository Films { get; }
    public InMemoryVehicleRepository Vehicles { get; }
    public InMemoryStarshipRepository Starships { get; }

    public InMemoryRepositorySet(
        IEnumerable<Person> people,
        IEnumerable<Species> species,
        IEnumerable<Homeworld> homeworlds,
        IEnumerable<Film> films,
        IEnumerable<Vehicle> vehicles,
        IEnumerable<Starship> starships)
    {
        People = new(people);
        Species = new(species);
        Homeworlds = new(homeworlds);
        Films = new(films);
        Vehicles = new(vehicles);
        Starships = new(starships);
    }

    public static InMemoryRepositorySet CreateDefault() => new(
        [
            new Person
            {
                Id = 1,
                Name = "Luke Skywalker",
                Height = new Height(172),
                Mass = new Mass(77),
                HairColor = "blond",
                SkinColor = "fair",
                EyeColor = "blue",
                BirthYear = "19BBY",
                Gender = "male",
                SpeciesId = 1,
                HomeworldId = 1,
                FilmIds = [3, 4, 5, 6],
                VehicleIds = [14],
                StarshipIds = [12, 22]
            },
            new Person
            {
                Id = 2,
                Name = "Leia Organa",
                Height = new Height(150),
                Mass = new Mass(49),
                HairColor = "brown",
                SkinColor = "light",
                EyeColor = "brown",
                BirthYear = "19BBY",
                Gender = "female",
                SpeciesId = 1,
                HomeworldId = 2,
                FilmIds = [3, 4, 5, 6],
                StarshipIds = [5]
            }
        ],

        [
            new Species(
                1,
                "Human",
                "mammal",
                "sentient",
                new Height(180),
                new Years(120),
                "Galactic Basic")
        ],

        [
            new Homeworld(
                1,
                "Tatooine",
                200000,
                "desert",
                "arid",
                new Percentage(5),
                new Distance(10465),
                new Days(23),
                new Days(304),
                "1 standard"),
            new Homeworld(
                2,
                "Alderaan",
                2000000000,
                "grasslands, mountains",
                "temperate",
                new Percentage(40),
                new Distance(12500),
                new Days(24),
                new Days(364),
                "1 standard")
        ],

        [
            new(
                1,
                "The Phantom Menace",
                1,
                "1999-05-19"),
            new(
                2,
                "Attack of the Clones",
                2,
                "2002-05-16"),
            new(
                3,
                "Revenge of the Sith",
                3,
                "2005-05-19"),
            new(
                4,
                "A New Hope",
                4,
                "1977-05-25"),
            new(
                5,
                "The Empire Strikes Back",
                5,
                "1980-05-17"),
            new(
                6,
                "Return of the Jedi",
                6,
                "1983-05-25")
        ],

        [
            new(
                14,
                "Snowspeeder",
                "t-47 airspeeder",
                "airspeeder",
                "Incom corporation",
                "2",
                "0")
        ],

        [
            new(
                12,
                "X-wing",
                "T-65",
                "Starfighter",
                "Incom corporation",
                "1",
                "0"),
            new(
                5,
                "Tantive IV",
                "CR90 corvette",
                "corvette",
                "Corellian Engineering Corporation",
                "30-165",
                "600"),
            new(
                22,
                "Imperial shuttle",
                "Lambda-class T-4a shuttle",
                "Armed government transport",
                "Sienar Fleet Systems",
                "5",
                "20")
        ]);
}
