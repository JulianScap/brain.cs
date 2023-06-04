namespace Brain.Models;

public class NeuralNetworkTestResult
{
    public bool Binary;
    public MisClass[] MisClasses = Array.Empty<MisClass>();
    public double Error;
    public int Total;
    public int TruePositives;
    public int TrueNegatives;
    public int FalsePositives;
    public int FalseNegatives;
    public int Precision;
    public int Recall;
    public int Accuracy;
}
