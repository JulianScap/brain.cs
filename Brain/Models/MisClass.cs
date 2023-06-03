namespace Brain.Models;

public class MisClass
{
    public double[] Input { get; set; } = Array.Empty<double>();
    public double[] Output { get; set; } = Array.Empty<double>();
    public double Actual { get; set; }
    public double Expected { get; set; }
}
