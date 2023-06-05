using Brain.Models;
using Brain.Utils;

namespace Brain.Console;

public static class OnlyImportTest
{
    public static void DoStuff()
    {
        TrainingDatum[] testData = Serialization.ReadFile<TrainingDatum[]>("~/brain.data/test.json");
        var export = Serialization.ReadFile<NeuralNetworkExport>("~/brain.data/network_1.json");

        NeuralNetwork network = new NeuralNetwork().Import(export);

        List<IGrouping<double, double>> results = testData
            .Select(datum => new
            {
                Expected = datum.Output[0],
                Result = network.Run(datum.Input)[0]
            })
            .Select(x => Math.Abs(x.Expected - x.Result))
            .GroupBy(delta => Math.Round(delta * 100))
            .OrderBy(x => x.Key)
            .ToList();

        foreach (IGrouping<double, double> result in results)
        {
            System.Console.WriteLine($"Delta: {result.Key / 100} => Count: {result.Count()}");
        }
    }
}
