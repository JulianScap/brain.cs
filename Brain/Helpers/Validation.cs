using System.Diagnostics.CodeAnalysis;

namespace Brain.Helpers;

public static class Validation
{
    public static void IsValidEnum<T>(this T value,
        string propertyName)
        where T : struct
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            throw new BrainException($"{typeof(T).FullName} does not support {value}");
        }
    }

    public static void StrictlyPositive(this int value,
        string propertyName)
    {
        value.GreaterThan(0, propertyName);
    }

    public static void StrictlyPositiveOrNull(this int? value,
        string propertyName)
    {
        value?.GreaterThan(0, propertyName);
    }

    public static void GreaterThan(this int value,
        int min,
        string propertyName)
    {
        if (value < min)
        {
            throw new BrainException($"{propertyName}: {value} should be greater than {min}");
        }
    }

    public static void InRangeExclusive(this double value,
        double min,
        double max,
        string propertyName)
    {
        if (value <= min || value >= max)
        {
            throw new BrainException($"{propertyName}: {value} should be in range ({min}..{max})");
        }
    }

    public static void NotNull([NotNull] this object? that,
        string propertyName)
    {
        if (that == null)
        {
            throw new BrainException($"{propertyName}: should not be null");
        }
    }
}
