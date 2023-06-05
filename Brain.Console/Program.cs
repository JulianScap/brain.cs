namespace Brain.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        NeuralNetworkTest.DoStuff();
        if (args.Length > 0)
        {
            CrossValidateTest.DoStuff();
            OnlyImportTest.DoStuff();
        }
    }
}
