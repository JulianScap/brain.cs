using Brain.Models;
using FluentAssertions;

namespace Brain.UnitTests;

public class NeuralNetworkTests
{
    [Fact]
    public void Train_ShouldTrain()
    {
        Action toTest = () => new NeuralNetwork(TestDataBuilder.DefaultConfiguration).Train(TestDataBuilder.GetXor());
        toTest.Should().NotThrow();
    }

    [Fact]
    public void Train_ShouldBeCorrect()
    {
        var network = new NeuralNetwork(TestDataBuilder.DefaultConfiguration);
        network.Train(TestDataBuilder.GetXor());

        double[] result = network.Run(new[]
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
        var network = new NeuralNetwork(TestDataBuilder.DefaultConfiguration);
        network.Train(TestDataBuilder.GetXor());

        Func<NeuralNetworkExport> toTest = () => network.Export();
        NeuralNetworkExport export = toTest.Should().NotThrow().Subject;

        export.Should().NotBeNull();

        NeuralNetwork imported = NeuralNetwork.From(export);

        double[] result = imported.Run(new[]
        {
            0d,
            1d
        });

        double[] expected = network.Run(new[]
        {
            0d,
            1d
        });

        result.Should().NotBeNull().And.HaveCount(1);
        result[0].Should().BeApproximately(expected[0], double.Epsilon);
    }
}
