namespace Brain.Utils;

public static class ArrayHelper
{
    private static readonly Random Random = new();

    public static double[] RandomFloats(int size)
    {
        var result = new double[size];

        for (var i = 0; i < size; i++)
        {
            result[i] = Random.NextDouble() * 0.4d - 0.2d;
        }

        return result;
    }

    public static double MeanSquaredError(double[] errors)
    {
        var sum = 0d;

        for (var i = 0; i < errors.Length; i++)
        {
            sum += errors[i] * errors[i];
        }

        return sum / errors.Length;
    }

    public static T[] YieldToArray<T>(this T that)
    {
        return new[]
        {
            that
        };
    }

    public static T[] Shuffle<T>(this T[] array)
    {
        if (array.IsNullOrEmpty())
        {
            return Array.Empty<T>();
        }

        var random = new Random();
        return array.OrderBy(_ => random.Next()).ToArray();
    }
}
