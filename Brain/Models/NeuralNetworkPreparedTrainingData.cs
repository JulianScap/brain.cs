namespace Brain.Models;

public class NeuralNetworkPreparedTrainingData
{
    public TrainingDatum[] PreparedData = Array.Empty<TrainingDatum>();
    public NeuralNetworkState Status = new();
    public long? EndTime;
}
