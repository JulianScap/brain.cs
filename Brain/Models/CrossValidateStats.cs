namespace Brain.Models;

public class CrossValidateStats
{
    public CrossValidateStatsAverages Averages { get; set; } = new();
    public CrossValidateStatsResultStats Stats { get; set; } = new();
    public CrossValidationTestPartitionResults[] Sets { get; set; } = Array.Empty<CrossValidationTestPartitionResults>();
}
