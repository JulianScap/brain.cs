using System.Diagnostics;

namespace Brain.Models;

public class NeuralNetworkPreparedTrainingData
{
    public readonly Stopwatch Stopwatch = new();
    public long? EndTime;
    public TrainingDatum[] PreparedData = Array.Empty<TrainingDatum>();
    public NeuralNetworkState Status = new();
}
