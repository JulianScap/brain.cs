using Brain.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Brain.Models;

public class NeuralNetworkTrainingOptions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ActivationType ActivationType = ActivationType.Sigmoid;

    public double Beta1 = 0.9f;
    public double Beta2 = 0.999f;
    public Action<NeuralNetworkState>? Callback;
    public int CallbackPeriod = 100;
    public double Epsilon = 1e-8f;
    public double ErrorThresh = 0.005f;

    public int Iteration = 20_000;
    public double LeakyReluAlpha = 0.01f;
    public double LearningRate = 0.3f;
    public Action<NeuralNetworkState>? LogAction;
    public int LogPeriod = 100;
    public double Momentum = 0.1f;

    public string? Praxis;

    /// <summary>
    ///     Null for infinity
    /// </summary>
    public long? Timeout;

    public NeuralNetworkTrainingOptions Export()
    {
        return new NeuralNetworkTrainingOptions
        {
            ActivationType = ActivationType,
            Iteration = Iteration,
            ErrorThresh = ErrorThresh,
            LeakyReluAlpha = LeakyReluAlpha,
            LearningRate = LearningRate,
            Momentum = Momentum,
            Timeout = Timeout,
            Praxis = Praxis,
            Beta1 = Beta1,
            Beta2 = Beta2,
            Epsilon = Epsilon
        };
    }

    public void ValidateTrainingOptions()
    {
        ActivationType.IsValidEnum(nameof(ActivationType));
        Iteration.StrictlyPositive(nameof(Iteration));
        ErrorThresh.InRangeExclusive(0, 1, nameof(ErrorThresh));

        if (LogAction != null)
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
