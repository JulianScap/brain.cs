namespace Brain.Models;

public class CrossValidationTestPartitionResults
{
    public long TrainTime;
    public long TestTime;
    public int Iterations;
    public double Error;
    public int Total;
    public NeuralNetworkExport Network = new();
    public MisClass[] MisClasses = Array.Empty<MisClass>();
    public bool Binary;
    public int TruePositives;
    public int TrueNegatives;
    public int FalsePositives;
    public int FalseNegatives;
    public int Precision;
    public int Recall;
    public int Accuracy;
}
