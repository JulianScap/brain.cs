namespace Brain.Models;

public class NeuralNetworkOptions
{
    public double BinaryThresh = 0.5;
    public int[] HiddenLayers = Array.Empty<int>();
    public int InputSize;
    public int OutputSize;

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
