namespace Brain.Models;

public class CrossValidateStatsResultStats
{
    public bool Binary { get; set; }
    public int Total { get; set; }
    public int TestSize { get; set; }
    public int TrainSize { get; set; }

    public int TruePositives { get; set; }
    public int TrueNegatives { get; set; }
    public int FalsePositives { get; set; }
    public int FalseNegatives { get; set; }
    public int Precision { get; set; }
    public int Recall { get; set; }
    public int Accuracy { get; set; }
}
