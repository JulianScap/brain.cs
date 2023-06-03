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


nn.Train(new TrainingDatum
    {
        Input = ArrayHelper.ToArray<double>(0, 0),
        Output = ArrayHelper.ToArray<double>(0)
    },
    new TrainingDatum
    {
        Input = ArrayHelper.ToArray<double>(0, 1),
        Output = ArrayHelper.ToArray<double>(1)
    },
    new TrainingDatum
    {
        Input = ArrayHelper.ToArray<double>(1, 0),
        Output = ArrayHelper.ToArray<double>(1)
    },
    new TrainingDatum
    {
        Input = ArrayHelper.ToArray<double>(1, 1),
        Output = ArrayHelper.ToArray<double>(0)
    }
);


Console.WriteLine(nn.Run(ArrayHelper.ToArray<double>(0, 0))[0]);
Console.WriteLine(nn.Run(ArrayHelper.ToArray<double>(1, 0))[0]);
Console.WriteLine(nn.Run(ArrayHelper.ToArray<double>(0, 1))[0]);
Console.WriteLine(nn.Run(ArrayHelper.ToArray<double>(1, 1))[0]);

NeuralNetworkExport export = nn.Export();

Console.WriteLine(JsonConvert.SerializeObject(export));

var imported = new NeuralNetwork();
imported.Import(export);

Console.WriteLine(imported.Run(ArrayHelper.ToArray<double>(0, 0))[0]);
Console.WriteLine(imported.Run(ArrayHelper.ToArray<double>(1, 0))[0]);
Console.WriteLine(imported.Run(ArrayHelper.ToArray<double>(0, 1))[0]);
Console.WriteLine(imported.Run(ArrayHelper.ToArray<double>(1, 1))[0]);
