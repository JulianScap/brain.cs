namespace Brain.Models;

public class NeuralNetworkOptions
{
    public int[] HiddenLayers = Array.Empty<int>();
    public int InputSize;
    public int OutputSize;
    public double BinaryThresh = 0.5;

    public NeuralNetworkOptions Export()
    {
        return new NeuralNetworkOptions
        {
            HiddenLayers = HiddenLayers.ToArray(),
            InputSize = InputSize,
            OutputSize = OutputSize
        };
    }
}
