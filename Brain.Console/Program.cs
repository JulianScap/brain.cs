using Brain;
using Brain.Models;
using Brain.Utils;

var configuration = new NeuralNetworkOptions
{
    HiddenLayers = new[]
    {
        3
    }
};

var trainingOptions = new NeuralNetworkTrainingOptions
{
    ActivationType = ActivationType.Sigmoid,
    LogPeriod = 10,
    LogAction = Console.WriteLine
};

TrainingDatum[] trainingData = Serialization.ReadFile<TrainingDatum[]>("~/brain.data/speeds.json");
TrainingDatum[] testData = Serialization.ReadFile<TrainingDatum[]>("~/brain.data/test.json");

var crossValidate = new CrossValidate(id => new NeuralNetwork(configuration, id));
crossValidate.Train(trainingData, trainingOptions, 2);

var network = crossValidate.ToNeuralNetwork();

List<IGrouping<double, double>> results = testData
    .Select(datum => new
    {
        Expected = datum.Output[0],
        Result = network.Run(datum.Input)[0]
    })
    .Select(x => Math.Abs(x.Expected - x.Result))
    .GroupBy(delta => Math.Round(delta * 100))
    .OrderBy(x => x.Key)
    .ToList();

foreach (IGrouping<double, double> result in results)
{
    Console.WriteLine($"{result.Key} => {result.Count()}");
}
