using System.Text.RegularExpressions;

namespace CharacterExplorer.Infrastructure.Swapi;

internal static class SwapiLink
{
    public static int? Id(string? url)
    {
        var match = Regex.Match(url ?? "", @"/(\d+)/?$");
        return match.Success ? int.Parse(match.Groups[1].Value) : null;
    }
}
