namespace Brain.Models;

public class NeuralNetworkExport
{
    public string Type { get; set; } = string.Empty;
    public int[] Sizes { get; set; } = Array.Empty<int>();
    public List<Layer> Layers { get; set; } = new();
    public NeuralNetworkConfiguration Options { get; set; } = new();
    public NeuralNetworkTrainingOptions? TrainOpt { get; set; } = new();
}
