using System.Net.Http.Json;

namespace CharacterExplorer.Infrastructure.Swapi;

public sealed class SwapiClient(HttpClient http)
{
    public async Task<T?> GetAsync<T>(string path)
    {
        var raw = await http.GetStringAsync(path);
        return await http.GetFromJsonAsync<T>(path);
    }
}
