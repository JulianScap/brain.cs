namespace Brain.Models;

public class CrossValidateStatsAverages
{
    public TimeSpan TrainTime { get; set; }
    public TimeSpan TestTime { get; set; }
    public int Iterations { get; set; }
    public double Error { get; set; }
}
