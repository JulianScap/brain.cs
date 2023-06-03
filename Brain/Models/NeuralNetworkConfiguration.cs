namespace Brain.Models;

public class NeuralNetworkConfiguration
{
    public int[] HiddenLayers { get; set; } = Array.Empty<int>();
    public int InputSize { get; set; }
    public int OutputSize { get; set; }
    public NeuralNetworkTrainingOptions? TrainingOptions { get; set; }
    public double BinaryThresh { get; set; } = 0.5;

    public NeuralNetworkConfiguration Export()
    {
        return new NeuralNetworkConfiguration
        {
            HiddenLayers = HiddenLayers.ToArray(),
            InputSize = InputSize,
            OutputSize = OutputSize,
            TrainingOptions = TrainingOptions?.Export()
        };
    }
}
