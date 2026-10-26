using System.Globalization;

namespace CharacterExplorer.Domain.Models;

public readonly record struct Height(decimal Centimeters)
{
    public override string ToString() => $"{Centimeters.ToString("G", CultureInfo.InvariantCulture)} cm";
}

public readonly record struct Mass(decimal Kilograms)
{
    public override string ToString() => $"{Kilograms.ToString("G", CultureInfo.InvariantCulture)} kg";
}

public readonly record struct Years(decimal Value)
{
    public override string ToString() => $"{Value.ToString("G", CultureInfo.InvariantCulture)} years";
}

public readonly record struct Percentage(decimal Value)
{
    public override string ToString() => $"{Value.ToString("G", CultureInfo.InvariantCulture)}%";
}

public readonly record struct Distance(decimal Kilometers)
{
    public override string ToString() => $"{Kilometers.ToString("G", CultureInfo.InvariantCulture)} km";
}

public readonly record struct Days(decimal Value)
{
    public override string ToString() => $"{Value.ToString("G", CultureInfo.InvariantCulture)} days";
}

public static class MeasurementParser
{
    public static Height? Height(string value) => TryParse(value, number => new Height(number));
    public static Mass? Mass(string value) => TryParse(value, number => new Mass(number));
    public static Years? Years(string value) => TryParse(value, number => new Years(number));
    public static Percentage? Percentage(string value) => TryParse(value, number => new Percentage(number));
    public static Distance? Distance(string value) => TryParse(value, number => new Distance(number));
    public static Days? Days(string value) => TryParse(value, number => new Days(number));
    public static long? Population(string value) =>
        long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            ? number
            : null;

    private static T? TryParse<T>(string value, Func<decimal, T> factory) where T : struct
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("unknown", StringComparison.OrdinalIgnoreCase))
            return null;

        var normalized = value.Trim();
        if (!normalized.Contains('.') && normalized.Count(c => c == ',') == 1)
            normalized = normalized.Replace(',', '.');

        return decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            ? factory(number)
            : null;
    }
}
