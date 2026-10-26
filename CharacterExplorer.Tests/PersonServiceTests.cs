using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;
using CharacterExplorer.Services.People;
using CharacterExplorer.Infrastructure.InMemory;
using Xunit;

namespace CharacterExplorer.Tests;

public sealed class PersonServiceTests
{
    [Fact]
    public async Task Default_InMemory_Repositories_Compose_The_Service_Graph()
    {
        var repositories = InMemoryRepositorySet.CreateDefault();
        var service = new PersonService(
            repositories.People,
            repositories.Species,
            repositories.Homeworlds,
            repositories.Films,
            repositories.Vehicles,
            repositories.Starships);

        var result = await service.GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.Equal("Luke Skywalker", result.Person!.Name);
        Assert.Equal("Tatooine", result.Person.Homeworld!.Name);
        Assert.Equal("A New Hope", result.Person.Films[3].Title);
        Assert.True(result.Person.Films[3].Appears);
        Assert.Equal(4, result.Person.Films.Count(film => film.Appears));
        Assert.Equal(6, result.Person.Films.Count);
        Assert.Contains(result.Person.Starships, starship => starship.Name == "X-wing");
    }

    [Fact]
    public async Task Search_Is_Case_Insensitive_And_Reads_From_Repository()
    {
        var service = CreatePersonService();
        var result = await service.SearchAsync("LUKE");
        Assert.True(result.Success);
        Assert.Single(result.People);
        Assert.Equal("Luke Skywalker", result.People[0].Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Search_With_No_Query_Returns_All_People(string? query)
    {
        var result = await CreatePersonService().SearchAsync(query);

        Assert.True(result.Success);
        Assert.Equal(["Luke Skywalker", "Leia Organa"], result.People.Select(person => person.Name));
    }

    [Fact]
    public async Task Search_With_Empty_Repository_Returns_Empty_Success()
    {
        var result = await CreatePersonService(people: EmptyPeopleRepo()).SearchAsync("Luke");

        Assert.True(result.Success);
        Assert.Empty(result.People);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Search_With_Blank_Query_And_Empty_Repository_Returns_Empty_Success(string? query)
    {
        var result = await CreatePersonService(people: EmptyPeopleRepo()).SearchAsync(query);

        Assert.True(result.Success);
        Assert.Empty(result.People);
    }

    [Theory]
    [InlineData(7, "Seven")]
    [InlineData(42, "Forty Two")]
    public async Task Search_Maps_Both_Person_Id_And_Name(int expectedId, string expectedName)
    {
        var result = await CreatePersonService(
            people: PeopleSummaryRepo([new(expectedId, expectedName)])
        ).SearchAsync(expectedName);

        Assert.True(result.Success);
        Assert.Single(result.People);
        Assert.Equal(expectedId, result.People[0].Id);
        Assert.Equal(expectedName, result.People[0].Name);
    }

    [Fact]
    public async Task Blank_Search_Returns_All_People()
    {
        var service = CreatePersonService();
        var result = await service.SearchAsync(" ");
        Assert.True(result.Success);
        Assert.Equal(2, result.People.Count);
    }

    [Theory]
    [InlineData(1, "Luke Skywalker", "male", "Tatooine", 6, 4, 1, 2)]
    [InlineData(2, "Leia Organa", "female", "Alderaan", 6, 4, 0, 1)]
    public async Task Details_Successfully_Load_People(
        int id,
        string expectedName,
        string? expectedGender,
        string? expectedHomeworld,
        int expectedTotalFilmCount,
        int expectedFilmCount,
        int expectedVehicleCount,
        int expectedStarshipCount)
    {
        var service = CreatePersonService();

        var result = await service.GetByIdAsync(id);

        Assert.True(result.Success);
        Assert.NotNull(result.Person);
        Assert.Equal(expectedName, result.Person.Name);
        Assert.Equal(expectedGender, result.Person.Gender);
        Assert.Equal(expectedHomeworld, result.Person.Homeworld?.Name);
        Assert.Equal(expectedTotalFilmCount, result.Person.Films.Count);
        Assert.Equal(expectedFilmCount, result.Person.Films.Count(film => film.Appears));
        Assert.Equal(expectedVehicleCount, result.Person.Vehicles.Count);
        Assert.Equal(expectedStarshipCount, result.Person.Starships.Count);
    }

    [Theory]
    [InlineData(99, "PersonNotFound")]
    [InlineData(500, "PersonNotFound")]
    public async Task Details_Failures_Return_Failure_Result(int id, string failureCode)
    {
        var service = CreatePersonService();

        var result = await service.GetByIdAsync(id);

        Assert.False(result.Success);
        Assert.Equal(failureCode, result.FailureCode);
        Assert.Null(result.Person);
    }

    [Fact]
    public async Task Search_With_No_Match_Is_Empty()
    {
        var service = CreatePersonService();
        var result = await service.SearchAsync("Boba");
        Assert.True(result.Success);
        Assert.Empty(result.People);
    }

    [Fact]
    public async Task Search_Trims_Query()
    {
        var service = CreatePersonService();
        var result = await service.SearchAsync("  leia  ");
        Assert.True(result.Success);
        Assert.Single(result.People);
    }

    [Theory]
    [InlineData("Luke", "Luke Skywalker")]
    [InlineData("Skywalker", "Luke Skywalker")]
    [InlineData("walker", "Luke Skywalker")]
    public async Task Search_Matches_At_Any_Position(string query, string expectedName)
    {
        var result = await CreatePersonService().SearchAsync(query);

        Assert.True(result.Success);
        Assert.Single(result.People);
        Assert.Equal(expectedName, result.People[0].Name);
    }

    [Theory]
    [InlineData("Luke Skywalker", 1)]
    [InlineData("luke skywalker", 1)]
    [InlineData("Luke", 1)]
    public async Task Search_Supports_Exact_And_Partial_Matches(string query, int expectedId)
    {
        var result = await CreatePersonService().SearchAsync(query);

        Assert.True(result.Success);
        Assert.Single(result.People);
        Assert.Equal(expectedId, result.People[0].Id);
    }

    [Theory]
    [InlineData("Luke  Skywalker")]
    [InlineData("A query longer than every person name")]
    [InlineData("Boba Fett")]
    public async Task Search_With_Non_Matching_Boundary_Query_Is_Empty(string query)
    {
        var result = await CreatePersonService().SearchAsync(query);

        Assert.True(result.Success);
        Assert.Empty(result.People);
    }

    [Theory]
    [InlineData("CAFÉ", "Café")]
    [InlineData("Ж", "ж")]
    public async Task Search_Is_Case_Insensitive_For_Unicode(string query, string name)
    {
        var result = await CreatePersonService(
            people: PeopleSummaryRepo([new(1, name)])
        ).SearchAsync(query);

        Assert.True(result.Success);
        Assert.Single(result.People);
        Assert.Equal(name, result.People[0].Name);
    }

    [Fact]
    public async Task Search_Returns_All_People_With_A_Partial_Query_Matching_Many()
    {
        var result = await CreatePersonService(people: PeopleSummaryRepo([
            new(1, "Leia Organa"),
            new(2, "Luke Skywalker"),
            new(3, "Anakin Skywalker")
        ])).SearchAsync("a");

        Assert.Equal([1, 2, 3], result.People.Select(person => person.Id));
    }

    [Fact]
    public async Task Search_Preserves_Duplicate_Names()
    {
        var result = await CreatePersonService(people: PeopleSummaryRepo([
            new(1, "Luke Skywalker"),
            new(2, "Luke Skywalker")
        ])).SearchAsync("Luke");

        Assert.Equal([1, 2], result.People.Select(person => person.Id));
        Assert.Equal(["Luke Skywalker", "Luke Skywalker"], result.People.Select(person => person.Name));
    }

    [Fact]
    public async Task Search_Does_Not_Mutate_Repository_Results()
    {
        var source = new List<PersonSummary>
        {
            new(1, "Luke Skywalker"),
            new(2, "Leia Organa")
        };

        await CreatePersonService(people: PeopleSummaryRepo(source)).SearchAsync("Luke");

        Assert.Equal(source, [new(1, "Luke Skywalker"), new(2, "Leia Organa")]);
    }

    [Fact]
    public async Task Search_Reads_From_InMemory_Person_Repository()
    {
        var peopleRepo = PeopleSummaryRepo([
            new(1, "Luke Skywalker"),
            new(2, "Leia Organa")
        ]);

        var result = await CreatePersonService(people: peopleRepo).SearchAsync("Luke");

        Assert.Single(result.People);
    }

    [Fact]
    public async Task Search_Preserves_Source_Order_When_Many_People_Match()
    {
        var peopleRepo = PeopleSummaryRepo([
            new(3, "Luke Clone"),
            new(1, "Luke Skywalker"),
            new(2, "Luke Organa")
        ]);

        var result = await CreatePersonService(people: peopleRepo).SearchAsync("luke");

        Assert.Equal([3, 1, 2], result.People.Select(person => person.Id));
    }

    [Fact]
    public async Task Search_Allows_An_Empty_Name()
    {
        var result = await CreatePersonService(people: PeopleSummaryRepo([new(1, "")])).SearchAsync("");

        Assert.True(result.Success);
        Assert.Single(result.People);
        Assert.Equal("", result.People[0].Name);
    }

    [Fact]
    public async Task Search_Returns_Failure_When_Repository_Throws()
    {
        var result = await CreatePersonService(people: new ThrowingPeopleRepo()).SearchAsync("Luke");

        Assert.False(result.Success);
        Assert.Empty(result.People);
        Assert.Equal("PeopleLookupFailed", result.FailureCode);
        Assert.Equal("Unable to retrieve people.", result.Message);
    }

    [Fact]
    public async Task Details_With_Empty_Species_Repository_Remains_Successful()
    {
        var result = await CreatePersonService(species: SpeciesRepo()).GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Person);
        Assert.Equal(1, result.Person.Id);
        Assert.Equal("Luke Skywalker", result.Person.Name);
        Assert.Equal("Tatooine", result.Person.Homeworld!.Name);
        Assert.Equal(6, result.Person.Films.Count);
        Assert.Equal(4, result.Person.Films.Count(film => film.Appears));
        Assert.Equal(2, result.Person.Starships.Count);

        Assert.Null(result.Person.Species);
    }

    [Fact]
    public async Task Details_With_Empty_Homeworld_Repository_Remains_Successful()
    {
        var result = await CreatePersonService(homeworlds: WorldRepo()).GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Person);
        Assert.Equal(1, result.Person.Id);
        Assert.Equal("Luke Skywalker", result.Person.Name);
        Assert.NotNull(result.Person.Species);
        Assert.Equal(6, result.Person.Films.Count);
        Assert.Equal(4, result.Person.Films.Count(film => film.Appears));
        Assert.Equal(2, result.Person.Starships.Count);

        Assert.Null(result.Person.Homeworld);
    }

    [Fact]
    public async Task Details_With_Empty_Film_Repository_Remains_Successful()
    {
        var result = await CreatePersonService(films: new InMemoryFilmRepository([])).GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Person);
        Assert.Equal(1, result.Person.Id);
        Assert.Equal("Luke Skywalker", result.Person.Name);
        Assert.NotNull(result.Person.Species);
        Assert.Equal("Tatooine", result.Person.Homeworld!.Name);
        Assert.Equal(2, result.Person.Starships.Count);
        Assert.Empty(result.Person.Films);
    }

    [Fact]
    public async Task Details_With_Empty_Vehicle_Repository_Remains_Successful()
    {
        var result = await CreatePersonService(vehicles: new InMemoryVehicleRepository([])).GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Person);
        Assert.Equal(1, result.Person.Id);
        Assert.Equal("Luke Skywalker", result.Person.Name);
        Assert.NotNull(result.Person.Species);
        Assert.Equal("Tatooine", result.Person.Homeworld!.Name);
        Assert.Equal(6, result.Person.Films.Count);
        Assert.Equal(4, result.Person.Films.Count(film => film.Appears));
        Assert.Equal(2, result.Person.Starships.Count);

        Assert.Empty(result.Person.Vehicles);
    }

    [Fact]
    public async Task Details_With_Empty_Starship_Repository_Remains_Successful()
    {
        var result = await CreatePersonService(starships: new InMemoryStarshipRepository([])).GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Person);
        Assert.Equal(1, result.Person.Id);
        Assert.Equal("Luke Skywalker", result.Person.Name);
        Assert.NotNull(result.Person.Species);
        Assert.Equal("Tatooine", result.Person.Homeworld!.Name);
        Assert.Equal(6, result.Person.Films.Count);
        Assert.Equal(4, result.Person.Films.Count(film => film.Appears));

        Assert.Empty(result.Person.Starships);
    }

    [Fact]
    public async Task Details_With_No_Related_Ids_Remains_Successful_With_Empty_Collections()
    {
        var person = new Person { Id = 0, Name = "Nobody" };

        var result = await CreatePersonService(
            people: PeopleRepo([person])
        ).GetByIdAsync(0);

        Assert.True(result.Success);
        Assert.Null(result.Person!.Species);
        Assert.Null(result.Person.Homeworld);
        Assert.Equal(0, result.Person.Films.Count(film => film.Appears));
        Assert.Empty(result.Person.Vehicles);
        Assert.Empty(result.Person.Starships);
    }

    [Fact]
    public async Task Details_Maps_All_Scalar_Fields()
    {
        var person = new Person
        {
            Id = 1, Name = "Complete", Height = new Height(180), Mass = new Mass(80),
            HairColor = "brown", SkinColor = "fair", EyeColor = "blue",
            BirthYear = "19BBY", Gender = "male"
        };

        var result = await CreatePersonService(people: PeopleRepo([person])).GetByIdAsync(1);

        Assert.Equal(("Complete", "180 cm", "80 kg", "brown", "fair", "blue", "19BBY", "male"),
            (result.Person!.Name, result.Person.Height, result.Person.Mass,
                result.Person.HairColor, result.Person.SkinColor, result.Person.EyeColor,
                result.Person.BirthYear, result.Person.Gender));
    }

    [Fact]
    public async Task Details_Maps_Many_Related_Records_And_Filters_Missing_Records()
    {
        var person = new Person
        {
            Id = 1, Name = "Many", Height = new Height(180), Mass = new Mass(80),
            SpeciesId = 7, HomeworldId = 8,
            FilmIds = [1, 2, 3], VehicleIds = [4, 5], StarshipIds = [6, 7]
        };

        var result = await CreatePersonService(
            people: PeopleRepo([person]),
            species: SpeciesRepo(new Species(
                7,
                "Human",
                "mammal",
                "sentient",
                new Height(180),
                new Years(120),
                "Basic")),
            homeworlds: WorldRepo(new Homeworld(
                8,
                "Earth",
                2000000,
                "temperate",
                "mild",
                new Percentage(70),
                new Distance(1),
                new Days(24),
                new Days(365),
                "1")),
            films: new InMemoryFilmRepository([
                new(
                    1,
                    "One",
                    1,
                    "1977"),
                new(
                    3,
                    "Three",
                    3,
                    "1983")]),
            vehicles: new InMemoryVehicleRepository([new(
                4,
                "Speeder",
                "X",
                "car",
                "Maker",
                "1",
                "1")]),
            starships: new InMemoryStarshipRepository([new(
                6,
                "Ship",
                "Y",
                "fighter",
                "Maker",
                "1",
                "1")])
        ).GetByIdAsync(1);

        Assert.Equal([1, 3], result.Person!.Films.Select(film => film.Id));
        Assert.Equal([4], result.Person.Vehicles.Select(vehicle => vehicle.Id));
        Assert.Equal([6], result.Person.Starships.Select(starship => starship.Id));
    }

    [Fact]
    public async Task Details_Does_Not_Preserve_Duplicate_Related_Ids()
    {
        var person = new Person { Id = 1, Name = "Duplicate", FilmIds = [4, 4] };

        var result = await CreatePersonService(people: PeopleRepo([person])).GetByIdAsync(1);

        Assert.Equal([1, 2, 3, 4, 5, 6], result.Person!.Films.Select(film => film.Id));
        Assert.Equal([4], result.Person!.Films.Where(film => film.Appears).Select(film => film.Id));
    }

    [Fact]
    public async Task Details_Uses_Related_Repository_Results_For_Each_Related_Id()
    {
        var person = new Person
        {
            Id = 1, Name = "Tracked", Height = new Height(180), Mass = new Mass(80),
            SpeciesId = 7, HomeworldId = 8,
            FilmIds = [1, 2], VehicleIds = [3], StarshipIds = [4]
        };

        var result = await CreatePersonService(
            people: PeopleRepo([person]),
            species: SpeciesRepo(new Species(
                7,
                "Human",
                "mammal",
                "sentient",
                new Height(180),
                new Years(120),
                "Basic")),
            homeworlds: WorldRepo(new Homeworld(
                8,
                "Earth",
                2000000,
                "temperate",
                "mild",
                new Percentage(70),
                new Distance(1),
                new Days(24),
                new Days(365),
                "1")),
            films: new InMemoryFilmRepository([
                new(
                    1,
                    "One",
                    1,
                    "1977"),
                new(
                    2,
                    "Two",
                    2,
                    "1980")
            ]),
            vehicles: new InMemoryVehicleRepository([new(
                3,
                "Speeder",
                "X",
                "car",
                "Maker",
                "1",
                "1")]),
            starships: new InMemoryStarshipRepository([new(
                4,
                "Ship",
                "Y",
                "fighter",
                "Maker",
                "1",
                "1")])
        ).GetByIdAsync(1);

        Assert.Equal("180 cm", result.Person!.Height);
        Assert.Equal("80 kg", result.Person.Mass);
        Assert.Equal("Human", result.Person.Species!.Name);
        Assert.Equal("180 cm", result.Person.Species.AverageHeight);
        Assert.Equal("120 years", result.Person.Species.AverageLifespan);
        Assert.Equal("Earth", result.Person.Homeworld!.Name);
        Assert.Equal("2,000,000", result.Person.Homeworld.Population);
        Assert.Equal("70%", result.Person.Homeworld.SurfaceWater);
        Assert.Equal("1 km", result.Person.Homeworld.Diameter);
        Assert.Equal("24 days", result.Person.Homeworld.RotationPeriod);
        Assert.Equal("365 days", result.Person.Homeworld.OrbitalPeriod);
        Assert.Equal([1, 2], result.Person.Films.Select(film => film.Id));
        Assert.Equal([3], result.Person.Vehicles.Select(vehicle => vehicle.Id));
        Assert.Equal([4], result.Person.Starships.Select(starship => starship.Id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task Details_Passes_Boundary_Ids_To_Person_Repository(int id)
    {
        var result = await CreatePersonService(people: PeopleRepo()).GetByIdAsync(id);

        Assert.False(result.Success);
        Assert.Equal("PersonNotFound", result.FailureCode);
    }

    [Fact]
    public async Task Details_Returns_Failure_When_Person_Repository_Throws()
    {
        var result = await CreatePersonService(people: new ThrowingPeopleRepo()).GetByIdAsync(1);

        AssertFailure(result, "PersonLookupFailed");
    }

    [Fact]
    public async Task Details_Returns_Failure_When_Related_Repository_Throws()
    {
        var person = new Person { Id = 1, Name = "Broken", FilmIds = [4] };
        var result = await CreatePersonService(
            people: PeopleRepo([person]),
            films: new ThrowingFilms()
        ).GetByIdAsync(1);

        AssertFailure(result, "PersonLookupFailed");
    }

    private static void AssertFailure(GetPersonResult result, string code)
    {
        Assert.False(result.Success);
        Assert.Equal(code, result.FailureCode);
        Assert.Null(result.Person);
        Assert.NotNull(result.Message);
    }

    private static PersonService CreatePersonService(
        IPersonRepository? people = null,
        ISpeciesRepository? species = null,
        IHomeworldRepository? homeworlds = null,
        IFilmRepository? films = null,
        IVehicleRepository? vehicles = null,
        IStarshipRepository? starships = null)
    {
        var defaults = InMemoryRepositorySet.CreateDefault();
        return new(
            people ?? defaults.People,
            species ?? defaults.Species,
            homeworlds ?? defaults.Homeworlds,
            films ?? defaults.Films,
            vehicles ?? defaults.Vehicles,
            starships ?? defaults.Starships);
    }

    private static InMemoryPersonRepository PeopleRepo() =>
        InMemoryRepositorySet.CreateDefault().People;

    private static InMemoryPersonRepository EmptyPeopleRepo() =>
        new(Array.Empty<Person>());

    private static InMemoryPersonRepository PeopleSummaryRepo(IEnumerable<PersonSummary> summaries) =>
        new(summaries.Select(summary => new Person { Id = summary.Id, Name = summary.Name }));

    private static InMemoryPersonRepository PeopleRepo(IEnumerable<Person> people) =>
        new(people);

    private static InMemorySpeciesRepository SpeciesRepo(
        params Species[] values) => new(values);

    private static InMemoryHomeworldRepository WorldRepo(
        params Homeworld[] values) => new(values);

    private sealed class ThrowingPeopleRepo : IPersonRepository
    {
        public Task<IReadOnlyList<PersonSummary>> GetAllAsync() => throw new InvalidOperationException("boom");
        public Task<Person?> GetByIdAsync(int _) => throw new InvalidOperationException("boom");
    }

    private sealed class ThrowingFilms : IFilmRepository
    {
        public Task<IEnumerable<Film>> GetAllAsync()
        {
            throw new InvalidOperationException("boom");
        }

        public Task<Film?> GetByIdAsync(int _) => throw new InvalidOperationException("boom");
    }

}
