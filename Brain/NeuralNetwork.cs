namespace Brain;

public class NeuralNetwork
{
    private readonly NeuralNetworkConfiguration _configuration;
    private readonly int[] _sizes;

    public NeuralNetwork() : this(new NeuralNetworkConfiguration())
    {
    }

    public NeuralNetwork(NeuralNetworkConfiguration configuration)
    {
        _configuration = configuration;

        List<int> sizes = configuration.HiddenLayers.ToList();
        sizes.Insert(0, configuration.InputSize);
        sizes.Add(configuration.OutputSize);

        _sizes = sizes.ToArray();
    }

    public void Train(params TrainingData[] trainingData)
    {
    }

    public void Train(double[] trainingData)
    {
    }
}
