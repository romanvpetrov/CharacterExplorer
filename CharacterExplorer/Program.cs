using CharacterExplorer.Domain.Repositories;
using CharacterExplorer.Infrastructure.InMemory;
using CharacterExplorer.Infrastructure.Swapi;
using CharacterExplorer.Services.People;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var repositoryImplementation = builder.Configuration["REPOSITORY_IMPLEMENTATION"]?.Trim().ToLowerInvariant()
    ?? "swapi";

switch (repositoryImplementation)
{
    case "inmemory":
        builder.Services.AddInMemoryRepositories();
        break;
    case "swapi":
        builder.Services.AddSwapiRepositories();
        break;
    default:
        throw new InvalidOperationException(
            "REPOSITORY_IMPLEMENTATION must be either 'inmemory' or 'swapi'.");
}

builder.Services.AddTransient<IPersonService, PersonService>();
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Run();

public partial class Program;
