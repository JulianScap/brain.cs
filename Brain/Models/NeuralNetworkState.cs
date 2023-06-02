namespace Brain.Models;

public class NeuralNetworkState : ICloneable
{
    public int Iterations { get; set; }
    public double Error { get; set; }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public NeuralNetworkState Clone()
    {
        return new NeuralNetworkState
        {
            Iterations = Iterations,
            Error = Error
        };
    }
}
