namespace Brain.Helpers;

public static class EnumerableHelper
{
    public static bool IsNullOrEmpty<T>(this T[]? that)
    {
        return that == null || that.Length == 0;
    }
}
