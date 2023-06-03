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

        double[] result = nn.Run(new[]
            {
                0d,
                1d
            }
        );

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

        double[] result = imported.Run(new[]
        {
            0d,
            1d
        });

        double[] expected = nn.Run(new[]
        {
            0d,
            1d
        });

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

        nn.Train(new[]
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
            }
        );

        return nn;
    }
}
