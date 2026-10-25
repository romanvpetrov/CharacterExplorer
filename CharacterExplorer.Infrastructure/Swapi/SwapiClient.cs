using System.Net.Http.Json;

namespace CharacterExplorer.Infrastructure.Swapi;

public sealed class SwapiClient(HttpClient http)
{
    public Task<T?> GetAsync<T>(string path)
    {
        return http.GetFromJsonAsync<T>(path);
    }
}
