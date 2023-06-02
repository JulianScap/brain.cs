namespace Brain.Models;

public class NeuralNetworkConfiguration
{
    public int[] HiddenLayers { get; set; } = Array.Empty<int>();
    public int InputSize { get; set; }
    public int OutputSize { get; set; }
    public int BinaryThresh { get; set; }
    public NeuralNetworkTrainingOptions? TrainingOptions { get; set; }
}
