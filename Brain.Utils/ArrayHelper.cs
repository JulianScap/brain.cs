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

    public static T? SafeGet<T>(this IList<T> array,
        int index)
        where T : class
    {
        return index < array.Count ? array[index] : null;
    }

    public static T[] ToArray<T>(params T[] things)
    {
        return things;
    }
}
