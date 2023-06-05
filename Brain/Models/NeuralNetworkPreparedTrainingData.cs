namespace Brain.Models;

public class NeuralNetworkPreparedTrainingData
{
    public long? EndTime;
    public TrainingDatum[] PreparedData = Array.Empty<TrainingDatum>();
    public NeuralNetworkState Status = new();
}
