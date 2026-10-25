using System.Text.Json.Serialization;
using CharacterExplorer.Domain.Models;
using CharacterExplorer.Domain.Repositories;

namespace CharacterExplorer.Infrastructure.Swapi;

internal sealed class FilmJson
{
    public string Title { get; set; } = "";
    [JsonPropertyName("episode_id")] public int EpisodeId { get; set; }
    [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } = "";
}

public sealed class SwapiFilmRepository(SwapiClient client) : IFilmRepository
{
    public async Task<Film?> GetByIdAsync(int id)
    {
        var film = await client.GetAsync<FilmJson>($"films/{id}");
        return film is null ? null : new(id, film.Title, film.EpisodeId, film.ReleaseDate, true);
    }
}
