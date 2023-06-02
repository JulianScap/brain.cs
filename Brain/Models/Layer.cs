namespace Brain.Models;

public class Layer
{
    public double[][] Weights { get; set; } = Array.Empty<double[]>();
    public double[] Biases { get; set; } = Array.Empty<double>();
}
