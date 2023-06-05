namespace Brain.Models;

public class CrossValidationTestPartitionResults
{
    public int Accuracy;
    public bool Binary;
    public double Error;
    public int FalseNegatives;
    public int FalsePositives;
    public int Iterations;
    public MisClass[] MisClasses = Array.Empty<MisClass>();
    public NeuralNetworkExport Network = new();
    public int Precision;
    public int Recall;
    public long TestTime;
    public int Total;
    public long TrainTime;
    public int TrueNegatives;
    public int TruePositives;
}
