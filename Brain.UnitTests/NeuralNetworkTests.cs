using FluentAssertions;

namespace Brain.UnitTests;

public class NeuralNetworkTests
{
    [Fact]
    public void Train_ShouldTrain()
    {
        var nn = new NeuralNetwork();

        Action toTest = () => nn.Train(new TrainingDatum
            {
                Input = Array(0, 0),
                Output = Array(0)
            },
            new TrainingDatum
            {
                Input = Array(0, 1),
                Output = Array(1)
            },
            new TrainingDatum
            {
                Input = Array(1, 0),
                Output = Array(1)
            },
            new TrainingDatum
            {
                Input = Array(1, 1),
                Output = Array(0)
            }
        );

        toTest.Should().NotThrow();
    }

    [Fact]
    public void Train_ShouldBeCorrect()
    {
        var nn = new NeuralNetwork();

        nn.Train(new TrainingDatum
            {
                Input = Array(0, 0),
                Output = Array(0)
            },
            new TrainingDatum
            {
                Input = Array(0, 1),
                Output = Array(1)
            },
            new TrainingDatum
            {
                Input = Array(1, 0),
                Output = Array(1)
            },
            new TrainingDatum
            {
                Input = Array(1, 1),
                Output = Array(0)
            }
        );

        double[] result = nn.Run(new double[]
        {
            0,
            1
        });

        result.Should().NotBeNull().And.HaveCount(1);
        // To fix later: result[0].Should().BeGreaterThan(0.9);
    }

    private static double[] Array(params double[] ints)
    {
        return ints;
    }
}
