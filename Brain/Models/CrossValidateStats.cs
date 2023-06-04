namespace Brain.Models;

public class CrossValidateStats
{
    public CrossValidateStatsAverages Averages = new();
    public CrossValidateStatsResultStats Stats = new();
    public CrossValidationTestPartitionResults[] Sets = Array.Empty<CrossValidationTestPartitionResults>();
}
