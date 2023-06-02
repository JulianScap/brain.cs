namespace Brain.Helpers;

public static class ArrayHelper
{
    private static readonly Random Random = new Random();

    public static double[] RandomFloats(int size)
    {
        var result = new double[size];

        for (int i = 0; i < size; i++)
        {
            result[i] = Random.NextDouble() * 0.4d - 0.2d;
        }

        return result;
    }
}
