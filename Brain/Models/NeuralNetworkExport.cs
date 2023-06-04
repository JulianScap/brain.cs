namespace Brain.Models;

public class NeuralNetworkExport
{
    public string Type = string.Empty;
    public int[] Sizes = Array.Empty<int>();
    public List<Layer> Layers = new();
    public NeuralNetworkOptions Options = new();
    public NeuralNetworkTrainingOptions? TrainOpt = new();
}
