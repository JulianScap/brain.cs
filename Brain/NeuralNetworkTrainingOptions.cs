namespace Brain;

public class NeuralNetworkTrainingOptions
{
    public ActivationType ActivationType { get; set; } = ActivationType.Sigmoid;
    public int Iteration { get; set; } = 20_000;
    public double ErrorThresh { get; set; } = 0.005f;
    public bool Log { get; set; } = false;
    public Action<NeuralNetworkState>? LogAction { get; set; }
    public int LogPeriod { get; set; } = 10;
    public double LeakyReluAlpha { get; set; } = 0.01f;
    public double LearningRate { get; set; } = 0.3f;
    public double Momentum { get; set; } = 0.1f;
    public Action<NeuralNetworkState>? Callback { get; set; }
    public int CallbackPeriod { get; set; } = 10;

    /// <summary>
    /// Null for infinity
    /// </summary>
    public int? Timeout { get; set; }

    public double Beta1 { get; set; } = 0.9f;
    public double Beta2 { get; set; } = 0.999f;
    public double Epsilon { get; set; } = 1e-8f;
}
