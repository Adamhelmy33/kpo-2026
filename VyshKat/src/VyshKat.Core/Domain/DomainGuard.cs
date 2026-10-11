namespace VyshKat.Core.Domain;

/// <summary>Общие проверки входных значений доменных объектов.</summary>
internal static class DomainGuard
{
    public static string NotBlank(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value.Trim();
    }

    public static decimal NonNegativeEnergy(decimal kwh, string paramName)
    {
        if (kwh < 0)
        {
            throw new ArgumentOutOfRangeException(
                paramName, kwh, "Суточный расход энергии не может быть отрицательным.");
        }

        return kwh;
    }

    public static int Positive(int value, string paramName, string message)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, message);
        }

        return value;
    }
}
