using Brain.Models;
using FluentAssertions;

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

        double[] result = nn.Run(ToArray(0, 1));

        result.Should().NotBeNull().And.HaveCount(1);
        result[0].Should().BeGreaterThan(0.9);
    }

    private static NeuralNetwork GetAndTrainXorNetwork()
    {
        var nn = new NeuralNetwork(new NeuralNetworkConfiguration
        {
            HiddenLayers = new[]
            {
                3
            },
            InputSize = 2,
            OutputSize = 1,
            BinaryThresh = 0.5,
            TrainingOptions = new NeuralNetworkTrainingOptions
            {
                ActivationType = ActivationType.Sigmoid,
            }
        });

        nn.Train(new TrainingDatum
            {
                Input = ToArray(0, 0),
                Output = ToArray(0)
            },
            new TrainingDatum
            {
                Input = ToArray(0, 1),
                Output = ToArray(1)
            },
            new TrainingDatum
            {
                Input = ToArray(1, 0),
                Output = ToArray(1)
            },
            new TrainingDatum
            {
                Input = ToArray(1, 1),
                Output = ToArray(0)
            }
        );

        return nn;
    }

    private static double[] ToArray(params double[] ints)
    {
        return ints;
    }
}
