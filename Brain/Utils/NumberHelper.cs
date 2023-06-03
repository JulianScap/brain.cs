namespace Brain.Utils;

public static class NumberHelper
{
    public static bool EqualEnough(this double number1, double number2, double epsilon = 1e-9)
    {
        double delta = number1 > number2
            ? number1 - number2
            : number2 - number1;

        return delta < epsilon;
    }
}
