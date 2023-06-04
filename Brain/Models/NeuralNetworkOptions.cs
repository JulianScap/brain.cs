namespace Brain.Models;

public class NeuralNetworkOptions
{
    public int[] HiddenLayers { get; set; } = Array.Empty<int>();
    public int InputSize { get; set; }
    public int OutputSize { get; set; }
    public double BinaryThresh { get; set; } = 0.5;

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
