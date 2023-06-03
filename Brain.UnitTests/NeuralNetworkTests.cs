using Brain.Models;
using Brain.Utils;
using FluentAssertions;
using Newtonsoft.Json;

namespace Brain.UnitTests;

public class NeuralNetworkTests
{
    [Fact]
    public void Train_ShouldTrain()
    {
        Action toTest = () => GetAndTrainXorNetwork();
        toTest.Should().NotThrow();
    }

    [Fact]
    public void Train_ShouldBeCorrect()
    {
        NeuralNetwork nn = GetAndTrainXorNetwork();

        double[] result = nn.Run(ArrayHelper.ToArray<double>(0, 1));

        result.Should().NotBeNull().And.HaveCount(1);
        result[0].Should().BeGreaterThan(0.9);
    }

    [Fact]
    public void Train_ShouldExportAndImport()
    {
        NeuralNetwork nn = GetAndTrainXorNetwork();

        Func<NeuralNetworkExport> toTest = () => nn.Export();
        NeuralNetworkExport export = toTest.Should().NotThrow().Subject;

        export.Should().NotBeNull();

        NeuralNetwork imported = new NeuralNetwork().Import(export);

        double[] result = imported.Run(ArrayHelper.ToArray<double>(0, 1));
        double[] expected = nn.Run(ArrayHelper.ToArray<double>(0, 1));

        result.Should().NotBeNull().And.HaveCount(1);
        result[0].Should().BeApproximately(expected[0], double.Epsilon);
    }

    private static NeuralNetwork GetAndTrainXorNetwork()
    {
        var nn = new NeuralNetwork(new NeuralNetworkConfiguration
        {
            HiddenLayers = new[]
            {
                3
            },
            TrainingOptions = new NeuralNetworkTrainingOptions
            {
                ActivationType = ActivationType.Sigmoid,
                LogAction = details => Console.WriteLine("Log " + JsonConvert.SerializeObject(details)),
                Callback = details => Console.WriteLine("Callback " + JsonConvert.SerializeObject(details))
            }
        });

        nn.Train(new TrainingDatum
            {
                Input = ArrayHelper.ToArray<double>(0, 0),
                Output = ArrayHelper.ToArray<double>(0)
            },
            new TrainingDatum
            {
                Input = ArrayHelper.ToArray<double>(0, 1),
                Output = ArrayHelper.ToArray<double>(1)
            },
            new TrainingDatum
            {
                Input = ArrayHelper.ToArray<double>(1, 0),
                Output = ArrayHelper.ToArray<double>(1)
            },
            new TrainingDatum
            {
                Input = ArrayHelper.ToArray<double>(1, 1),
                Output = ArrayHelper.ToArray<double>(0)
            }
        );

        return nn;
    }
}
