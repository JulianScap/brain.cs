namespace Brain.Models;

public class NeuralNetworkPreparedTrainingData
{
    public TrainingDatum[] PreparedData { get; set; } = Array.Empty<TrainingDatum>();
    public NeuralNetworkState Status { get; set; } = new();
    public DateTime? EndTime { get; set; }
}
