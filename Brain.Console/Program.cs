using Brain;
using Brain.Models;
using Brain.Utils;
using Newtonsoft.Json;

var nn = new NeuralNetwork(new NeuralNetworkConfiguration
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
});


nn.Train(new[]
    {
        new TrainingDatum
        {
            Input = new[]
            {
                0d,
                0d
            },
            Output = 0d.YieldToArray()
        },
        new TrainingDatum
        {
            Input = new[]
            {
                0d,
                1d
            },
            Output = 1d.YieldToArray()
        },
        new TrainingDatum
        {
            Input = new[]
            {
                1d,
                0d
            },
            Output = 1d.YieldToArray()
        },
        new TrainingDatum
        {
            Input = new[]
            {
                1d,
                1d
            },
            Output = 0d.YieldToArray()
        }
    }
);


Console.WriteLine(nn.Run(new[]
{
    0d,
    0d
})[0]);

Console.WriteLine(nn.Run(new[]
{
    0d,
    1d
})[0]);

Console.WriteLine(nn.Run(new[]
{
    1d,
    0d
})[0]);

Console.WriteLine(nn.Run(new[]
{
    1d,
    1d
})[0]);

NeuralNetworkExport export = nn.Export();

Console.WriteLine(JsonConvert.SerializeObject(export));

var imported = new NeuralNetwork();
imported.Import(export);


Console.WriteLine(imported.Run(new[]
{
    0d,
    0d
})[0]);

Console.WriteLine(imported.Run(new[]
{
    0d,
    1d
})[0]);

Console.WriteLine(imported.Run(new[]
{
    1d,
    0d
})[0]);

Console.WriteLine(imported.Run(new[]
{
    1d,
    1d
})[0]);
