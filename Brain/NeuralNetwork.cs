using Brain.Helpers;
using Brain.Models;

namespace Brain;

public class NeuralNetwork
{
    private readonly NeuralNetworkConfiguration _configuration;
    private Action _adjustWeights;
    private double[][] _biasChangesHigh;
    private double[][] _biasChangesLow;
    private double[][] _biases;
    private Action<double[]> _calculateDeltas;
    private double[][][] _changes;
    private double[][][] _changesHigh;
    private double[][][] _changesLow;
    private double[][] _deltas;
    private readonly int _errorCheckInterval;
    private double[][] _errors;
    private int _inputLookupLength;
    private int _iterations;
    private int _outputLayer = -1;
    private int _outputLookupLength;
    private double[][] _outputs;
    private Func<double[], double[]> _runInput;
    private int[] _sizes;
    private NeuralNetworkTrainingOptions _trainOpts;
    private double[][][] _weights;

    public NeuralNetwork() : this(new NeuralNetworkConfiguration())
    {
    }

    public NeuralNetwork(NeuralNetworkConfiguration configuration)
    {
        _configuration = configuration;

        List<int> sizes = configuration.HiddenLayers.ToList();
        sizes.Insert(0, configuration.InputSize);
        sizes.Add(configuration.OutputSize);

        _sizes = sizes.ToArray();
        _errorCheckInterval = 1;

        _runInput = RunInput;
        _calculateDeltas = CalculateDeltas;
        _adjustWeights = AdjustWeights;
    }

    public NeuralNetworkState Train(NeuralNetworkTrainingOptions options,
        params TrainingDatum[] data)
    {
        NeuralNetworkPreparedTrainingData preparedData = PrepareTraining(data, options);

        while (true)
        {
            if (!TrainingTick(preparedData.PreparedData, preparedData.Status, preparedData.EndTime))
            {
                break;
            }
        }

        return preparedData.Status;
    }

    private bool TrainingTick(TrainingDatum[] data,
        NeuralNetworkState status,
        DateTime? endTime)
    {
        Action<NeuralNetworkState>? callback = _trainOpts.Callback;
        int callbackPeriod = _trainOpts.CallbackPeriod;
        double errorThresh = _trainOpts.ErrorThresh;
        double iterations = _trainOpts.Epsilon;
        bool log = _trainOpts.Log;
        Action<NeuralNetworkState>? logAction = _trainOpts.LogAction;
        int logPeriod = _trainOpts.LogPeriod;

        if (status.Iterations >= iterations || status.Error <= errorThresh || DateTime.Now >= endTime)
        {
            return false;
        }

        status.Iterations++;

        if (log && status.Iterations % logPeriod == 0)
        {
            status.Error = CalculateTrainingError(data);
            logAction?.Invoke(status);
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
        double learningRate = _trainOpts.LearningRate;
        double momentum = _trainOpts.Momentum;

        for (var layer = 1; layer <= _outputLayer; layer++)
        {
            double[] incoming = _outputs[layer - 1];
            int activeSize = _sizes[layer];
            double[] activeDelta = _deltas[layer];
            double[][] activeChanges = _changes[layer];
            double[][] activeWeights = _weights[layer];
            double[] activeBiases = _biases[layer];

            for (var node = 0; node < activeSize; node++)
            {
                double delta = activeDelta[node];

                for (var k = 0; k < incoming.Length; k++)
                {
                    double change = activeChanges[node][k];

                    change = learningRate * delta * incoming[k] + momentum * change;

                    activeChanges[node][k] = change;
                    activeWeights[node][k] += change;
                }

                activeBiases[node] += learningRate * delta;
            }
        }
    }

    public NeuralNetworkState Train(params TrainingDatum[] data)
    {
        return Train(
            _configuration.TrainingOptions ?? new NeuralNetworkTrainingOptions(),
            data
        );
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
                Error = 1,
                Iterations = 0
            },
            EndTime = options.Timeout.HasValue
                ? DateTime.Now + TimeSpan.FromMilliseconds(options.Timeout.Value)
                : null
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

        if (_configuration.HiddenLayers.IsNullOrEmpty())
        {
            var halfLength = (int) Math.Floor(data[0].Input.Length / 2d);
            sizes.Add(Math.Max(3, halfLength));
        }
        else
        {
            sizes.AddRange(_configuration.HiddenLayers);
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

            if (layerIndex > 0)
            {
                _biases[layerIndex] = ArrayHelper.RandomFloats(size);
                _weights[layerIndex] = new double[size][];
                _changes[layerIndex] = new double[size][];

                for (var nodeIndex = 0; nodeIndex < size; nodeIndex++)
                {
                    int prevSize = _sizes[layerIndex - 1];
                    _weights[layerIndex][nodeIndex] = ArrayHelper.RandomFloats(prevSize);
                    _changes[layerIndex][nodeIndex] = new double[prevSize];
                }
            }
        }

        SetActivation();
        if (_trainOpts.Praxis == Constant.Adam)
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
        double beta1 = _trainOpts.Beta1;
        double beta2 = _trainOpts.Beta2;
        double epsilon = _trainOpts.Epsilon;
        double learningRate = _trainOpts.LearningRate;

        for (var layer = 1; layer <= _outputLayer; layer++)
        {
            double[] incoming = _outputs[layer - 1];
            int currentSize = _sizes[layer];
            double[] currentDeltas = _deltas[layer];
            double[][] currentChangesLow = _changesLow[layer];
            double[][] currentChangesHigh = _changesHigh[layer];
            double[][] currentWeights = _weights[layer];
            double[] currentBiases = _biases[layer];
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
                    currentWeights[node][k] += learningRate * momentumCorrection / (Math.Sqrt(gradientCorrection) + epsilon);
                }

                double biasGradient = currentDeltas[node];
                double biasChangeLow = currentBiasChangesLow[node] * beta1 + (1 - beta1) * biasGradient;
                double biasChangeHigh = currentBiasChangesHigh[node] * beta2 + (1 - beta2) * biasGradient * biasGradient;

                double biasMomentumCorrection = currentBiasChangesLow[node] / (1 - Math.Pow(beta1, iterations));
                double biasGradientCorrection = currentBiasChangesHigh[node] / (1 - Math.Pow(beta2, iterations));

                currentBiasChangesLow[node] = biasChangeLow;
                currentBiasChangesHigh[node] = biasChangeHigh;
                currentBiases[node] += learningRate * biasMomentumCorrection / (Math.Sqrt(biasGradientCorrection) + epsilon);
            }
        }
    }

    private void SetActivation(ActivationType? activation = null)
    {
        ActivationType value = activation ?? _trainOpts.ActivationType;
        switch (value)
        {
            case ActivationType.Sigmoid:
                _runInput = RunInputSigmoid;
                _calculateDeltas = CalculateDeltasSigmoid;
                break;
            // case ActivationType.Relu:
            //     _runInput = this._runInputRelu;
            //     _calculateDeltas = this._calculateDeltasRelu;
            //     break;
            // case ActivationType.LeakyRelu:
            //     _runInput = this._runInputLeakyRelu;
            //     _calculateDeltas = this._calculateDeltasLeakyRelu;
            //     break;
            // case ActivationType.Tanh:
            //     _runInput = this._runInputTanh;
            //     _calculateDeltas = this._calculateDeltasTanh;
            //     break;
            default:
                throw new BrainException($"Unknown activation ${value}");
        }
    }

    private void CalculateDeltasSigmoid(double[] target)
    {
        for (int layer = _outputLayer; layer >= 0; layer--)
        {
            int activeSize = _sizes[layer];
            double[] activeOutput = _outputs[layer];
            double[] activeError = _errors[layer];
            double[] activeDeltas = _deltas[layer];
            double[][]? nextLayer = _weights.SafeGet(layer + 1);

            for (var node = 0; node < activeSize; node++)
            {
                double output = activeOutput[node];

                var error = 0d;
                if (layer == _outputLayer)
                {
                    error = target[node] - output;
                }
                else
                {
                    double[] deltas = _deltas[layer + 1];
                    for (var k = 0; k < deltas.Length; k++)
                    {
                        error += deltas[k] * (nextLayer?[k][node] ?? 0);
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

        double[] output = Array.Empty<double>();
        for (var layer = 1; layer <= _outputLayer; layer++)
        {
            int activeLayer = _sizes[layer];
            double[][] activeWeights = _weights[layer];
            double[] activeBiases = _biases[layer];
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

            output = input = activeOutputs;
        }

        if (output == null)
        {
            throw new BrainException("output was empty");
        }

        return output;
    }

    private void UpdateTrainingOptions(NeuralNetworkTrainingOptions options)
    {
        NeuralNetworkTrainingOptions merged = options.Merge(_configuration.TrainingOptions);
        merged.ValidateTrainingOptions();
        _trainOpts = merged;
        SetLogMethod();
    }

    private void SetLogMethod()
    {
        if (_trainOpts.Log && _trainOpts.LogAction == null)
        {
            _trainOpts.LogAction = status => Console.WriteLine($"Iterations: {status.Iterations}, Training error: {status.Error}");
        }
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
}
