using CharacterExplorer.Infrastructure.Swapi;
using CharacterExplorer.Domain.Repositories;
using CharacterExplorer.Services.People;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient<SwapiClient>(client =>
    client.BaseAddress = new Uri("https://swapi.info/api/")
);
builder.Services.AddSingleton<IPersonRepository, SwapiPersonRepository>();
builder.Services.AddTransient<ISpeciesRepository, SwapiSpeciesRepository>();
builder.Services.AddTransient<IHomeworldRepository, SwapiHomeworldRepository>();
builder.Services.AddTransient<IFilmRepository, SwapiFilmRepository>();
builder.Services.AddTransient<IVehicleRepository, SwapiVehicleRepository>();
builder.Services.AddTransient<IStarshipRepository, SwapiStarshipRepository>();
builder.Services.AddTransient<IPersonService, PersonService>();
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Run();

public partial class Program;
