namespace Brain.Models;

public class CrossValidateStats
{
    public CrossValidateStatsAverages Averages = new();
    public CrossValidationTestPartitionResults[] Sets = Array.Empty<CrossValidationTestPartitionResults>();
    public CrossValidateStatsResultStats Stats = new();
}
