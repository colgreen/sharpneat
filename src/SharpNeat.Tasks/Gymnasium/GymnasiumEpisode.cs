using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Gymnasium;

public sealed class GymnasiumEpisode
{
    readonly int _inputCount;
    readonly int _outputCount;
    readonly bool _isContinious;
    readonly bool _test;

    public GymnasiumEpisode(int inputCount, int outputCount, bool isContinious, bool test)
    {
        _inputCount = inputCount;
        _outputCount = outputCount;
        _isContinious = isContinious;
        _test = test;
    }

    public FitnessInfo Evaluate(IBlackBox<double> phenome)
    {
        var uuid = Guid.NewGuid();

        var start = new ProcessStartInfo
        {
            FileName = @"pythonw.exe",
            WorkingDirectory = @"./",
            Arguments = string.Format(CultureInfo.InvariantCulture, @"gymnasium/main.py -uuid {0} -render {1}", uuid.ToString(), _test),
            UseShellExecute = false,
            RedirectStandardOutput = false
        };

        var process = Process.Start(start) ?? throw new InvalidOperationException("No proccess resource is started");
        var totalReward = 0.0;

        try
        {
            var namedPipeClientStream = new NamedPipeClientStream(".", $"gymnasium_pipe_{uuid}", PipeDirection.InOut);
            namedPipeClientStream.Connect(10000);
            namedPipeClientStream.ReadMode = PipeTransmissionMode.Message;

            // Clear any prior agent state.
            phenome.Reset();

            while (true)
            {
                // Determine agent sensor input values.
                // Reset all inputs.
                var inputs = phenome.Inputs.Span;
                inputs.Clear();

                var observationTuple = ReadObservation(namedPipeClientStream, _inputCount);
                var observation = observationTuple.observation;
                totalReward = observationTuple.reward[0];
                var done = observationTuple.done[0];

                if (done == 1)
                {
                    break;
                }

                observation.CopyTo(phenome.Inputs);
                phenome.Activate();

                // var clampedOutputs = outputs.Select(output => Math.Clamp(output, -1.0, 1.0)).ToArray();
                if (_isContinious)
                {
                    var outputBuffer = new byte[_outputCount * sizeof(float)];
                    var outputs = new double[_outputCount];
                    phenome.Outputs.CopyTo(outputs);
                    Buffer.BlockCopy(Array.ConvertAll(outputs, x => (float)x), 0, outputBuffer, 0, outputBuffer.Length);
                    namedPipeClientStream.Write(outputBuffer, 0, outputBuffer.Length);
                }
                else
                {
                    int maxSigIndex = ReadMaxSigIndex(phenome);
                    var outputBuffer = new byte[sizeof(int)];
                    Buffer.BlockCopy(new int[] { maxSigIndex }, 0, outputBuffer, 0, outputBuffer.Length);
                    namedPipeClientStream.Write(outputBuffer, 0, outputBuffer.Length);
                }
            }

            namedPipeClientStream.Close();
        }
        catch
        {
            if (!_test)
            {
                throw;
            }
        }
        finally
        {
            process.WaitForExit();
        }

        var maskedReward = totalReward < 1 ? Math.Pow(Math.E, totalReward - 1) : totalReward;
        return new FitnessInfo(maskedReward);
    }

    static (double[] observation, double[] reward, int[] done) ReadObservation(NamedPipeClientStream namedPipeClientStream, int count)
    {
        var count0 = count * sizeof(double);
        var count1 = sizeof(double);
        var count2 = sizeof(int);
        var inputBuffer = new byte[count0 + count1 + count2];
        namedPipeClientStream.Read(inputBuffer, 0, inputBuffer.Length);
        double[] observation = new double[count];
        double[] reward = new double[1];
        int[] done = new int[1];
        var offset1 = count0;
        var offset2 = count0 + count1;
        Buffer.BlockCopy(inputBuffer, 0, observation, 0, count0);
        Buffer.BlockCopy(inputBuffer, offset1, reward, 0, count1);
        Buffer.BlockCopy(inputBuffer, offset2, done, 0, count2);
        return (observation, reward, done);
    }

    static double[] ReadDoubleArray(NamedPipeClientStream namedPipeClientStream, int count)
    {
        var inputBuffer = new byte[count * sizeof(double)];
        namedPipeClientStream.Read(inputBuffer, 0, inputBuffer.Length);
        double[] values = new double[inputBuffer.Length / sizeof(double)];
        Buffer.BlockCopy(inputBuffer, 0, values, 0, values.Length * sizeof(double));
        return values;
    }

    static float[] ReadFloatArray(NamedPipeClientStream namedPipeClientStream, int count)
    {
        var inputBuffer = new byte[count * sizeof(float)];
        namedPipeClientStream.Read(inputBuffer, 0, inputBuffer.Length);
        float[] values = new float[inputBuffer.Length / sizeof(float)];
        Buffer.BlockCopy(inputBuffer, 0, values, 0, values.Length * sizeof(float));
        return values;
    }

    static int[] ReadIntArray(NamedPipeClientStream namedPipeClientStream, int count)
    {
        var inputBuffer = new byte[count * sizeof(int)];
        namedPipeClientStream.Read(inputBuffer, 0, inputBuffer.Length);
        int[] values = new int[inputBuffer.Length / sizeof(int)];
        Buffer.BlockCopy(inputBuffer, 0, values, 0, values.Length * sizeof(int));
        return values;
    }

    int ReadMaxSigIndex(IBlackBox<double> phenome)
    {
        double maxSig = phenome.Outputs.Span[0];
        int maxSigIdx = 0;

        for (int i = 1; i < _outputCount; i++)
        {
            double v = phenome.Outputs.Span[i];
            if (v > maxSig)
            {
                maxSig = v;
                maxSigIdx = i;
            }
        }

        return maxSigIdx;
    }
}
