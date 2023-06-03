namespace Brain;

public class CrossValidate
{
    private readonly Func<NeuralNetwork> _builder;

    public CrossValidate(Func<NeuralNetwork> builder)
    {
        _builder = builder;
    }
}
