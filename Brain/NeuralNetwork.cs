using System.Diagnostics.CodeAnalysis;
using Brain.Models;
using Brain.Utils;

namespace Brain;

[SuppressMessage("ReSharper", "CognitiveComplexity")]
public class NeuralNetwork
{
    private readonly int _errorCheckInterval;
    private readonly int? _id;
    private Action _adjustWeights;
    private double[][] _biasChangesHigh = Array.Empty<double[]>();
    private double[][] _biasChangesLow = Array.Empty<double[]>();
    private double[]?[] _biases = Array.Empty<double[]>();
    private Action<double[]> _calculateDeltas;
    private double[][][] _changes = Array.Empty<double[][]>();
    private double[][][] _changesHigh = Array.Empty<double[][]>();
    private double[][][] _changesLow = Array.Empty<double[][]>();
    private double[][] _deltas = Array.Empty<double[]>();
    private double[][] _errors = Array.Empty<double[]>();
    private int _iterations;
    private NeuralNetworkOptions _options;
    private int _outputLayer = -1;
    private double[][] _outputs = Array.Empty<double[]>();
    private Func<double[], double[]> _runInput;
    private int[] _sizes;
    private NeuralNetworkTrainingOptions _trainOptions = new();
    private double[][]?[] _weights = Array.Empty<double[][]>();

    public NeuralNetwork(NeuralNetworkOptions? configuration = null, int? id = null)
    {
        _id = id;
        _options = configuration ?? new NeuralNetworkOptions();

        List<int> sizes = _options.HiddenLayers.ToList();
        sizes.Insert(0, _options.InputSize);
        sizes.Add(_options.OutputSize);

        _sizes = sizes.ToArray();
        _errorCheckInterval = 1;

        _runInput = RunInput;
        _calculateDeltas = CalculateDeltas;
        _adjustWeights = AdjustWeights;
    }

    public NeuralNetworkState Train(TrainingDatum[] data,
        NeuralNetworkTrainingOptions options
    )
    {
        NeuralNetworkPreparedTrainingData preparedData = PrepareTraining(data, options);
        preparedData.Stopwatch.Start();

        var shouldContinue = true;

        while (shouldContinue && TrainingTick(preparedData.PreparedData, preparedData.Status))
        {
            if (preparedData.EndTime.HasValue)
            {
                shouldContinue = preparedData.Stopwatch.ElapsedMilliseconds < preparedData.EndTime;
            }
        }

        return preparedData.Status;
    }

    private bool TrainingTick(TrainingDatum[] data,
        NeuralNetworkState status)
    {
        Action<NeuralNetworkState>? callback = _trainOptions.Callback;
        int callbackPeriod = _trainOptions.CallbackPeriod;
        double errorThresh = _trainOptions.ErrorThresh;
        double iterations = _trainOptions.Iteration;
        Action<NeuralNetworkState>? logAction = _trainOptions.LogAction;
        int logPeriod = _trainOptions.LogPeriod;

        if (status.Iterations >= iterations || status.Error <= errorThresh)
        {
            return false;
        }

        status.Iterations++;

        if (logAction != null && status.Iterations % logPeriod == 0)
        {
            status.Error = CalculateTrainingError(data);
            logAction(status);
        }
        else if (status.Iterations % _errorCheckInterval == 0)
        {
            status.Error = CalculateTrainingError(data);
        }
        else
        {
            TrainPatterns(data);
        }

        if (callback != null && status.Iterations % callbackPeriod == 0)
        {
            callback(status.Clone());
        }

        return true;
    }

    private double CalculateTrainingError(TrainingDatum[] data)
    {
        double sum = 0;

        for (var i = 0; i < data.Length; ++i)
        {
            sum += TrainPattern(data[i], true);
        }

        return sum / data.Length;
    }

    private void TrainPatterns(TrainingDatum[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            TrainPattern(data[i]);
        }
    }

    private double TrainPattern(TrainingDatum value,
        bool logErrorRate = false)
    {
        // forward propagate
        _runInput(value.Input);

        // back propagate
        _calculateDeltas(value.Output);
        _adjustWeights();

        if (logErrorRate)
        {
            return ArrayHelper.MeanSquaredError(_errors[_outputLayer]);
        }

        return 0d;
    }

    private void AdjustWeights()
    {
        double learningRate = _trainOptions.LearningRate;
        double momentum = _trainOptions.Momentum;

        for (var layer = 1; layer <= _outputLayer; layer++)
        {
            double[] incoming = _outputs[layer - 1];
            int activeSize = _sizes[layer];
            double[] activeDelta = _deltas[layer];
            double[][] activeChanges = _changes[layer];
            double[][]? activeWeights = _weights[layer];
            double[]? activeBiases = _biases[layer];

            for (var node = 0; node < activeSize; node++)
            {
                double delta = activeDelta[node];

                for (var k = 0; k < incoming.Length; k++)
                {
                    double change = activeChanges[node][k];

                    change = learningRate * delta * incoming[k] + momentum * change;

                    activeChanges[node][k] = change;
                    if (activeWeights != null)
                    {
                        activeWeights[node][k] += change;
                    }
                }

                if (activeBiases != null)
                {
                    activeBiases[node] += learningRate * delta;
                }
            }
        }
    }

    private NeuralNetworkPreparedTrainingData PrepareTraining(TrainingDatum[] data,
        NeuralNetworkTrainingOptions options)
    {
        UpdateTrainingOptions(options);

        VerifyIsInitialized(data);
        ValidateData(data);

        return new NeuralNetworkPreparedTrainingData
        {
            PreparedData = data,
            Status = new NeuralNetworkState
            {
                Id = _id,
                Error = 1,
                Iterations = 0
            },
            EndTime = options.TimeoutMilliseconds
        };
    }

    private void ValidateData(TrainingDatum[] data)
    {
        int inputSize = _sizes[0];
        int outputSize = _sizes[^1];
        int length = data.Length;
        for (var i = 0; i < length; i++)
        {
            double[] input = data[i].Input;
            double[] output = data[i].Output;

            if (input.Length != inputSize)
            {
                throw new BrainException($"input at index ${i} length ${input.Length} must be ${inputSize}");
            }

            if (output.Length != outputSize)
            {
                throw new BrainException($"output at index ${i} length ${output.Length} must be ${outputSize}");
            }
        }
    }

    private void VerifyIsInitialized(TrainingDatum[] data)
    {
        if (_sizes.Length > 0 && _outputLayer > 0)
        {
            return;
        }

        var sizes = new List<int>
        {
            data[0].Input.Length
        };

        if (_options.HiddenLayers.IsNullOrEmpty())
        {
            var halfLength = (int) Math.Floor(data[0].Input.Length / 2d);
            sizes.Add(Math.Max(3, halfLength));
        }
        else
        {
            sizes.AddRange(_options.HiddenLayers);
        }

        sizes.Add(data[0].Output.Length);

        _sizes = sizes.ToArray();

        Initialize();
    }

    private void Initialize()
    {
        if (_sizes.IsNullOrEmpty())
        {
            throw new BrainException("Sizes must be set before initializing");
        }

        _outputLayer = _sizes.Length - 1;
        _biases = new double[_sizes.Length][];
        _weights = new double[_sizes.Length][][];
        _outputs = new double[_sizes.Length][];

        // state for training
        _deltas = new double[_sizes.Length][];
        _changes = new double[_sizes.Length][][]; // for momentum
        _errors = new double[_sizes.Length][];

        for (var layerIndex = 0; layerIndex <= _outputLayer; layerIndex++)
        {
            int size = _sizes[layerIndex];
            _deltas[layerIndex] = new double[size];
            _errors[layerIndex] = new double[size];
            _outputs[layerIndex] = new double[size];

            if (layerIndex <= 0)
            {
                continue;
            }

            _biases[layerIndex] = ArrayHelper.RandomFloats(size);
            _weights[layerIndex] = new double[size][];
            _changes[layerIndex] = new double[size][];

            for (var nodeIndex = 0; nodeIndex < size; nodeIndex++)
            {
                int prevSize = _sizes[layerIndex - 1];
                _changes[layerIndex][nodeIndex] = new double[prevSize];

                double[][]? weights = _weights[layerIndex];
                if (weights != null)
                {
                    weights[nodeIndex] = ArrayHelper.RandomFloats(prevSize);
                }
            }
        }

        SetActivation();
        if (_trainOptions.Praxis == Constant.Adam)
        {
            SetupAdam();
        }
    }

    private void SetupAdam()
    {
        int arraySize = _outputLayer + 1;
        _biasChangesLow = new double[arraySize][];
        _biasChangesHigh = new double[arraySize][];
        _changesLow = new double[arraySize][][];
        _changesHigh = new double[arraySize][][];
        _iterations = 0;

        for (var layer = 0; layer <= _outputLayer; layer++)
        {
            int size = _sizes[layer];
            if (layer > 0)
            {
                _biasChangesLow[layer] = new double[size];
                _biasChangesHigh[layer] = new double[size];
                _changesLow[layer] = new double[size][];
                _changesHigh[layer] = new double[size][];

                for (var node = 0; node < size; node++)
                {
                    int prevSize = _sizes[layer - 1];
                    _changesLow[layer][node] = new double[prevSize];
                    _changesHigh[layer][node] = new double[prevSize];
                }
            }
        }

        _adjustWeights = AdjustWeightsAdam;
    }

    private void AdjustWeightsAdam()
    {
        _iterations++;

        int iterations = _iterations;
        double beta1 = _trainOptions.Beta1;
        double beta2 = _trainOptions.Beta2;
        double epsilon = _trainOptions.Epsilon;
        double learningRate = _trainOptions.LearningRate;

        for (var layer = 1; layer <= _outputLayer; layer++)
        {
            double[] incoming = _outputs[layer - 1];
            int currentSize = _sizes[layer];
            double[] currentDeltas = _deltas[layer];
            double[][] currentChangesLow = _changesLow[layer];
            double[][] currentChangesHigh = _changesHigh[layer];
            double[][]? currentWeights = _weights[layer];
            double[]? currentBiases = _biases[layer];
            double[] currentBiasChangesLow = _biasChangesLow[layer];
            double[] currentBiasChangesHigh = _biasChangesHigh[layer];

            for (var node = 0; node < currentSize; node++)
            {
                double delta = currentDeltas[node];

                for (var k = 0; k < incoming.Length; k++)
                {
                    double gradient = delta * incoming[k];
                    double changeLow = currentChangesLow[node][k] * beta1 + (1 - beta1) * gradient;
                    double changeHigh = currentChangesHigh[node][k] * beta2 + (1 - beta2) * gradient * gradient;

                    double momentumCorrection = changeLow / (1 - Math.Pow(beta1, iterations));
                    double gradientCorrection = changeHigh / (1 - Math.Pow(beta2, iterations));

                    currentChangesLow[node][k] = changeLow;
                    currentChangesHigh[node][k] = changeHigh;
                    if (currentWeights != null)
                    {
                        currentWeights[node][k] += learningRate * momentumCorrection / (Math.Sqrt(gradientCorrection) + epsilon);
                    }
                }

                double biasGradient = currentDeltas[node];
                double biasChangeLow = currentBiasChangesLow[node] * beta1 + (1 - beta1) * biasGradient;
                double biasChangeHigh = currentBiasChangesHigh[node] * beta2 + (1 - beta2) * biasGradient * biasGradient;

                double biasMomentumCorrection = currentBiasChangesLow[node] / (1 - Math.Pow(beta1, iterations));
                double biasGradientCorrection = currentBiasChangesHigh[node] / (1 - Math.Pow(beta2, iterations));

                currentBiasChangesLow[node] = biasChangeLow;
                currentBiasChangesHigh[node] = biasChangeHigh;
                if (currentBiases != null)
                {
                    currentBiases[node] += learningRate * biasMomentumCorrection / (Math.Sqrt(biasGradientCorrection) + epsilon);
                }
            }
        }
    }

    private void SetActivation(ActivationType? activation = null)
    {
        ActivationType value = activation ?? _trainOptions.ActivationType;
        switch (value)
        {
            case ActivationType.Sigmoid:
                _runInput = RunInputSigmoid;
                _calculateDeltas = CalculateDeltasSigmoid;
                break;
            case ActivationType.Relu:
                _runInput = RunInputRelu;
                _calculateDeltas = CalculateDeltasRelu;
                break;
            case ActivationType.LeakyRelu:
                _runInput = RunInputLeakyRelu;
                _calculateDeltas = CalculateDeltasLeakyRelu;
                break;
            case ActivationType.Tanh:
                _runInput = RunInputTanh;
                _calculateDeltas = CalculateDeltasTanh;
                break;
            default:
                throw new BrainException($"Unknown activation ${value}");
        }
    }

    private void CalculateDeltasTanh(double[] target)
    {
        throw new NotImplementedException();
    }

    private double[] RunInputTanh(double[] input)
    {
        throw new NotImplementedException();
    }

    private void CalculateDeltasLeakyRelu(double[] target)
    {
        throw new NotImplementedException();
    }

    private double[] RunInputLeakyRelu(double[] input)
    {
        throw new NotImplementedException();
    }

    private void CalculateDeltasRelu(double[] target)
    {
        throw new NotImplementedException();
    }

    private double[] RunInputRelu(double[] input)
    {
        throw new NotImplementedException();
    }

    private void CalculateDeltasSigmoid(double[] target)
    {
        for (int layer = _outputLayer; layer >= 0; layer--)
        {
            int activeSize = _sizes[layer];
            double[] activeOutput = _outputs[layer];
            double[] activeError = _errors[layer];
            double[] activeDeltas = _deltas[layer];

            double[][]? nextLayer = null;
            if (layer + 1 < _weights.Length)
            {
                nextLayer = _weights[layer + 1];
            }

            for (var node = 0; node < activeSize; node++)
            {
                double output = activeOutput[node];

                var error = 0d;
                if (layer == _outputLayer)
                {
                    error = target[node] - output;
                }
                else if (nextLayer != null)
                {
                    double[] deltas = _deltas[layer + 1];
                    for (var k = 0; k < deltas.Length; k++)
                    {
                        error += deltas[k] * nextLayer[k][node];
                    }
                }

                activeError[node] = error;
                activeDeltas[node] = error * output * (1 - output);
            }
        }
    }

    private double[] RunInputSigmoid(double[] input)
    {
        _outputs[0] = input; // set output state of input layer

        double[]? output = null;
        for (var layer = 1; layer <= _outputLayer; layer++)
        {
            int activeLayer = _sizes[layer];
            double[][]? activeWeights = _weights[layer];
            double[]? activeBiases = _biases[layer];

            if (activeWeights == null || activeBiases == null)
            {
                continue;
            }

            double[] activeOutputs = _outputs[layer];
            for (var node = 0; node < activeLayer; node++)
            {
                double[] weights = activeWeights[node];

                double sum = activeBiases[node];
                for (var k = 0; k < weights.Length; k++)
                {
                    sum += weights[k] * input[k];
                }

                // sigmoid
                activeOutputs[node] = 1 / (1 + Math.Exp(-sum));
            }

            input = activeOutputs;
            output = activeOutputs;
        }

        if (output == null)
        {
            throw new BrainException("output was empty");
        }

        return output;
    }

    private void UpdateTrainingOptions(NeuralNetworkTrainingOptions options)
    {
        options.ValidateTrainingOptions();
        _trainOptions = options;
    }

    private double[] RunInput(double[] input)
    {
        SetActivation();
        return _runInput(input);
    }

    private void CalculateDeltas(double[] output)
    {
        SetActivation();
        _calculateDeltas(output);
    }

    public double[] Run(double[] input)
    {
        if (!IsRunnable())
        {
            throw new BrainException("Network not runnable");
        }

        ValidateInput(input);
        double[] rawOutput = _runInput(input);
        var output = new double[rawOutput.Length];
        Array.Copy(rawOutput, output, rawOutput.Length);
        return output;
    }

    private void ValidateInput(double[] input)
    {
        int inputSize = _sizes[0];
        if (input.Length != inputSize)
        {
            throw new BrainException($"input length ${input.Length} must match options.inputSize of ${inputSize}");
        }
    }

    private bool IsRunnable()
    {
        return _sizes.Length > 0;
    }

    public NeuralNetworkExport Export()
    {
        if (!IsRunnable())
        {
            Initialize();
        }

        double[][]?[] jsonLayerWeights =
            _weights.Select(layerLayerWeights =>
                    layerLayerWeights?.Select(
                            layerWeights => layerWeights.ToArray())
                        .ToArray())
                .ToArray();

        double[]?[] jsonLayerBiases = _biases.Select(layerBiases =>
                layerBiases?.ToArray()
            )
            .ToArray();

        var layers = new List<Layer>();
        int outputLength = _sizes.Length - 1;

        for (var i = 0; i <= outputLength; i++)
        {
            var layer = new Layer();

            if (i < jsonLayerWeights.Length)
            {
                layer.Weights = jsonLayerWeights[i] ?? Array.Empty<double[]>();
            }

            if (i < jsonLayerBiases.Length)
            {
                layer.Biases = jsonLayerBiases[i] ?? Array.Empty<double>();
            }

            layers.Add(layer);
        }

        return new NeuralNetworkExport
        {
            Type = nameof(NeuralNetwork),
            Sizes = _sizes.ToArray(),
            Layers = layers,
            Options = _options.Export(),
            TrainOpt = _trainOptions.Export()
        };
    }

    public NeuralNetwork Import(NeuralNetworkExport export)
    {
        _options = export.Options;

        if (export.TrainOpt != null)
        {
            UpdateTrainingOptions(export.TrainOpt);
        }

        _sizes = export.Sizes;
        Initialize();

        List<Layer> layers = export.Layers;

        double[][]?[] layerWeights = _weights.Select((_,
                layerIndex) => layers[layerIndex].Weights.Select(layerWeights => layerWeights.ToArray()).ToArray())
            .ToArray();

        double[]?[] layerBiases = _biases.Select((_,
                    layerIndex) => layers[layerIndex].Biases.ToArray()
            )
            .ToArray();

        for (var i = 0; i <= _outputLayer; i++)
        {
            _weights[i] = layerWeights[i] ?? Array.Empty<double[]>();
            _biases[i] = layerBiases[i] ?? Array.Empty<double>();
        }

        return this;
    }

    public NeuralNetworkTestResult Test(TrainingDatum[] data)
    {
        NeuralNetworkPreparedTrainingData trainingData = PrepareTraining(data, _trainOptions);
        TrainingDatum[] preparedData = trainingData.PreparedData;
        // for binary classification problems with one output node
        bool isBinary = preparedData[0].Output.Length == 1;

        // run each pattern through the trained network and collect
        // error and mis-classification statistics
        if (isBinary)
        {
            return TestBinary(preparedData);
        }

        var misClasses = new List<MisClass>();
        var errorSum = 0d;

        for (var i = 0; i < preparedData.Length; i++)
        {
            double[] output = _runInput(preparedData[i].Input);
            double[] target = preparedData[i].Output;
            int actual = Array.IndexOf(output, output.Max());
            int expected = Array.IndexOf(target, target.Max());

            if (actual != expected)
            {
                TrainingDatum misClass = preparedData[i];
                misClasses.Add(new MisClass
                {
                    Input = misClass.Input,
                    Output = misClass.Output,
                    Actual = actual,
                    Expected = expected
                });
            }

            errorSum += ArrayHelper.MeanSquaredError(output.Select((value, index) => target[index] - value).ToArray());
        }

        return new NeuralNetworkTestResult
        {
            Error = errorSum / preparedData.Length,
            MisClasses = misClasses.ToArray(),
            Total = preparedData.Length
        };
    }

    private NeuralNetworkTestResult TestBinary(TrainingDatum[] preparedData)
    {
        var misClasses = new List<MisClass>();
        var errorSum = 0d;
        var falsePos = 0;
        var falseNeg = 0;
        var truePos = 0;
        var trueNeg = 0;

        for (var i = 0; i < preparedData.Length; i++)
        {
            double[] output = _runInput(preparedData[i].Input);
            double[] target = preparedData[i].Output;
            int actual = output[0] > _options.BinaryThresh ? 1 : 0;
            double expected = target[0];

            if (!expected.EqualEnough(actual))
            {
                TrainingDatum misClass = preparedData[i];
                misClasses.Add(new MisClass
                {
                    Input = misClass.Input,
                    Output = misClass.Output,
                    Actual = actual,
                    Expected = expected
                });
            }

            if (actual == 0 && expected == 0)
            {
                trueNeg++;
            }
            else if (actual == 1 && expected.EqualEnough(1))
            {
                truePos++;
            }
            else if (actual == 0 && expected.EqualEnough(1))
            {
                falseNeg++;
            }
            else if (actual == 1 && expected == 0)
            {
                falsePos++;
            }

            errorSum += ArrayHelper.MeanSquaredError(output.Select((value, index) => target[index] - value).ToArray());
        }

        return new NeuralNetworkTestResult
        {
            Binary = true,
            Error = errorSum / preparedData.Length,
            MisClasses = misClasses.ToArray(),
            Total = preparedData.Length,
            TrueNegatives = trueNeg,
            TruePositives = truePos,
            FalseNegatives = falseNeg,
            FalsePositives = falsePos,
            Precision = truePos > 0 ? truePos / (truePos + falsePos) : 0,
            Recall = truePos > 0 ? truePos / (truePos + falseNeg) : 0,
            Accuracy = (trueNeg + truePos) / preparedData.Length
        };
    }

    public static NeuralNetwork From(NeuralNetworkExport export)
    {
        return new NeuralNetwork().Import(export);
    }
}
