using Brain.Models;
using Brain.Utils;
using Newtonsoft.Json;

namespace Brain.UnitTests;

public static class TestDataBuilder
{
    public static readonly NeuralNetworkConfiguration DefaultConfiguration = new()
    {
        HiddenLayers = new[]
        {
            3
        },
        TrainingOptions = new NeuralNetworkTrainingOptions
        {
            ActivationType = ActivationType.Sigmoid,
            LogAction = neuralNetworkState => Console.WriteLine("Log " + JsonConvert.SerializeObject(neuralNetworkState)),
            Callback = neuralNetworkState => Console.WriteLine("Callback " + JsonConvert.SerializeObject(neuralNetworkState))
        }
    };

    public static TrainingDatum[] GetXor()
    {
        return new[]
        {
            new TrainingDatum
            {
                Input = new[]
                {
                    0d,
                    0d
                },
                Output = 0d.YieldToArray()
            },
            new TrainingDatum
            {
                Input = new[]
                {
                    0d,
                    1d
                },
                Output = 1d.YieldToArray()
            },
            new TrainingDatum
            {
                Input = new[]
                {
                    1d,
                    0d
                },
                Output = 1d.YieldToArray()
            },
            new TrainingDatum
            {
                Input = new[]
                {
                    1d,
                    1d
                },
                Output = 0d.YieldToArray()
            }
        };
    }
}
