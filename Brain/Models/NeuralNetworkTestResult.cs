namespace Brain.Models;

public class NeuralNetworkTestResult
{
    public int Accuracy;
    public bool Binary;
    public double Error;
    public int FalseNegatives;
    public int FalsePositives;
    public MisClass[] MisClasses = Array.Empty<MisClass>();
    public int Precision;
    public int Recall;
    public int Total;
    public int TrueNegatives;
    public int TruePositives;
}
