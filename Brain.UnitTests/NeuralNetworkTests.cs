using FluentAssertions;

namespace Brain.UnitTests;

public class NeuralNetworkTests
{
    [Fact]
    public void Train_ShouldTrain()
    {
        var nn = new NeuralNetwork();

        Action toTest = () => nn.Train(new TrainingData
            {
                Input = Array(0, 0),
                Output = Array(0),
            },
            new TrainingData
            {
                Input = Array(0, 1),
                Output = Array(1),
            },
            new TrainingData
            {
                Input = Array(1, 0),
                Output = Array(1),
            },
            new TrainingData
            {
                Input = Array(1, 1),
                Output = Array(0),
            }
        );

        toTest.Should().NotThrow();
    }

    private static double[] Array(params double[] ints)
    {
        return ints;
    }
}
