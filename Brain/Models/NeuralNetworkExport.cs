namespace Brain.Models;

public class NeuralNetworkExport
{
    public List<Layer> Layers = new();
    public NeuralNetworkOptions Options = new();
    public int[] Sizes = Array.Empty<int>();
    public NeuralNetworkTrainingOptions? TrainOpt = new();
    public string Type = string.Empty;
}
