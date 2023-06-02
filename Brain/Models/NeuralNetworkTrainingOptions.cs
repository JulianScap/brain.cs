using Brain.Utils;

namespace Brain.Models;

public class NeuralNetworkTrainingOptions
{
    public string Praxis { get; set; } = string.Empty;
    public ActivationType ActivationType { get; set; } = ActivationType.Sigmoid;
    public int Iteration { get; set; } = 20_000;
    public double ErrorThresh { get; set; } = 0.005f;
    public Action<NeuralNetworkState>? LogAction { get; set; }
    public int LogPeriod { get; set; } = 10;
    public double LeakyReluAlpha { get; set; } = 0.01f;
    public double LearningRate { get; set; } = 0.3f;
    public double Momentum { get; set; } = 0.1f;
    public Action<NeuralNetworkState>? Callback { get; set; }
    public int CallbackPeriod { get; set; } = 10;

    /// <summary>
    ///     Null for infinity
    /// </summary>
    public int? Timeout { get; set; }

    public double Beta1 { get; set; } = 0.9f;
    public double Beta2 { get; set; } = 0.999f;
    public double Epsilon { get; set; } = 1e-8f;

    public void ValidateTrainingOptions()
    {
        ActivationType.IsValidEnum(nameof(ActivationType));
        Iteration.StrictlyPositive(nameof(Iteration));
        ErrorThresh.InRangeExclusive(0, 1, nameof(ErrorThresh));

        if (LogPeriod != null)
        {
            LogPeriod.StrictlyPositive(nameof(LogPeriod));
        }

        LeakyReluAlpha.InRangeExclusive(0, 1, nameof(LeakyReluAlpha));
        LearningRate.InRangeExclusive(0, 1, nameof(LearningRate));
        Momentum.InRangeExclusive(0, 1, nameof(Momentum));

        if (Callback != null)
        {
            CallbackPeriod.StrictlyPositive(nameof(CallbackPeriod));
        }

        Timeout.StrictlyPositiveOrNull(nameof(Timeout));
        Beta1.InRangeExclusive(0, 1, nameof(Beta1));
        Beta2.InRangeExclusive(0, 1, nameof(Beta2));
        Epsilon.InRangeExclusive(0, 1, nameof(Epsilon));
    }
}
