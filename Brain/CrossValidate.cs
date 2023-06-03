using Brain.Models;
using Brain.Utils;

namespace Brain;

public class CrossValidate
{
    private readonly Func<NeuralNetwork> _initClassifier;

    public CrossValidate(Func<NeuralNetwork> initClassifier)
    {
        _initClassifier = initClassifier;
    }

    public CrossValidateStats Stats { get; set; } = new();

    public CrossValidateStats Train(TrainingDatum[] data,
        NeuralNetworkTrainingOptions options,
        int k = 4)
    {
        if (data.Length < k)
        {
            throw new BrainException($"Training set size is too small for ${data.Length} k folds of ${k}");
        }

        data = data.Shuffle();
        int size = data.Length / k;

        var stats = new CrossValidateStatsResultStats();
        var averages = new CrossValidateStatsAverages();
        var results = new List<CrossValidationTestPartitionResults>();
        bool? binary = null;

        for (var i = 0; i < k; i++)
        {
            TrainingDatum[] dataClone = data.ToArray();
            var testSet = new TrainingDatum[size];
            Array.Copy(dataClone, i * size, testSet, 0, size);

            CrossValidationTestPartitionResults result = TestPartition(options, dataClone, testSet);

            if (!binary.HasValue)
            {
                binary = result.Binary;
                stats.Binary = result.Binary;
            }

            averages.Iterations += result.Iterations;
            averages.TestTime += result.TestTime;
            averages.TrainTime += result.TrainTime;
            averages.Error += result.Error;
            stats.Total += result.Total;

            if (stats.Binary && result.Binary)
            {
                stats.Accuracy += result.Accuracy;
                stats.FalseNegatives += result.FalseNegatives;
                stats.FalsePositives += result.FalsePositives;
                stats.Precision += result.Precision;
                stats.Recall += result.Recall;
                stats.TrueNegatives += result.TrueNegatives;
                stats.TruePositives += result.TruePositives;
            }

            results.Add(result);
        }

        averages.Error /= k;
        averages.Iterations /= k;
        averages.TestTime /= k;
        averages.TrainTime /= k;

        if (stats.Binary)
        {
            stats.Precision = stats.TruePositives / (stats.TruePositives + stats.FalsePositives);
            stats.Recall = stats.TruePositives / (stats.TruePositives + stats.FalseNegatives);
            stats.Accuracy = (stats.TrueNegatives + stats.TruePositives) / stats.Total;
        }

        stats.TestSize = size;
        stats.TrainSize = data.Length - size;

        Stats = new CrossValidateStats
        {
            Averages = averages,
            Stats = stats,
            Sets = results.ToArray()
        };

        return Stats;
    }

    private CrossValidationTestPartitionResults TestPartition(NeuralNetworkTrainingOptions options, TrainingDatum[] trainSet, TrainingDatum[] testSet)
    {
        NeuralNetwork classifier = _initClassifier();
        DateTime beginTrain = DateTime.Now;
        DateTime beginTest = DateTime.Now;

        NeuralNetworkState trainingStats = classifier.Train(trainSet, options);
        NeuralNetworkTestResult testStats = classifier.Test(testSet);
        DateTime endTest = DateTime.Now;

        return new CrossValidationTestPartitionResults
        {
            Binary = testStats.Binary,
            Accuracy = testStats.Accuracy,
            TrueNegatives = testStats.TrueNegatives,
            TruePositives = testStats.TruePositives,
            FalseNegatives = testStats.FalseNegatives,
            FalsePositives = testStats.FalsePositives,
            Recall = testStats.Recall,
            Precision = testStats.Precision,
            MisClasses = testStats.MisClasses,
            TrainTime = beginTest - beginTrain,
            TestTime = endTest - beginTest,
            Iterations = trainingStats.Iterations,
            Error = trainingStats.Error,
            Total = testStats.Total,
            Network = classifier.Export()
        };
    }

    public NeuralNetwork ToNeuralNetwork()
    {
        var network = new NeuralNetwork();

        CrossValidationTestPartitionResults? winning = Stats.Sets.MinBy(set => set.Error);

        if (winning == null)
        {
            throw new BrainException("No winning plan found");
        }

        network.Import(winning.Network);

        return network;
    }
}
