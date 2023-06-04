using Brain;
using Brain.Models;
using Brain.Utils;
using Newtonsoft.Json;

var configuration = new NeuralNetworkConfiguration
{
    HiddenLayers = new[]
    {
        3
    },
    TrainingOptions = new NeuralNetworkTrainingOptions
    {
        ActivationType = ActivationType.Sigmoid,
        LogPeriod = 1,
        CallbackPeriod = 1,
        LogAction = details => Console.WriteLine("Log " + JsonConvert.SerializeObject(details)),
        Callback = details => Console.WriteLine("Callback " + JsonConvert.SerializeObject(details))
    }
};

TrainingDatum[] trainingData = Serialization.ReadFile<TrainingDatum[]>("~/brain.data/speeds.json") ?? throw new BrainException("No training data found");
TrainingDatum[] testData = Serialization.ReadFile<TrainingDatum[]>("~/brain.data/test.json") ?? throw new BrainException("No training data found");

var crossValidate = new CrossValidate(() => new NeuralNetwork(configuration));
crossValidate.Train(trainingData, configuration.TrainingOptions);

var network = crossValidate.ToNeuralNetwork();

foreach (TrainingDatum testDatum in testData)
{
    double[] output = network.Run(testDatum.Input);
    Console.WriteLine($"{string.Join(", ", testDatum.Input)} => {output[0]} | {testDatum.Output[0]}");
}
