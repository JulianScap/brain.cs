namespace Brain.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        OnlyImportTest.DoStuff();
        if (args.Length > 0)
        {
            CrossValidateTest.DoStuff();
        }
    }
}
