namespace Brain.Models;

public class NeuralNetworkState : ICloneable
{
    public int Iterations;
    public double Error;
    public int? Id;

    object ICloneable.Clone()
    {
        return Clone();
    }

    public NeuralNetworkState Clone()
    {
        return new NeuralNetworkState
        {
            Id = Id,
            Iterations = Iterations,
            Error = Error
        };
    }

    public override string ToString()
    {
        return $"N{Id}, Iterations {Iterations:00000}, Errors {Error}";
    }
}
