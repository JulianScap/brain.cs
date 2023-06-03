namespace Brain.Models;

public class CrossValidationTestPartitionResults
{
    public TimeSpan TrainTime { get; set; }
    public TimeSpan TestTime { get; set; }
    public int Iterations { get; set; }
    public double Error { get; set; }
    public int Total { get; set; }
    public NeuralNetworkExport Network { get; set; } = new();
    public MisClass[] MisClasses { get; set; } = Array.Empty<MisClass>();
    public bool Binary { get; set; }
    public int TruePositives { get; set; }
    public int TrueNegatives { get; set; }
    public int FalsePositives { get; set; }
    public int FalseNegatives { get; set; }
    public int Precision { get; set; }
    public int Recall { get; set; }
    public int Accuracy { get; set; }
}
