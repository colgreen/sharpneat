using System.Globalization;
using Serilog;
using SharpNeat.Experiments;
using SharpNeat.Tasks.BinaryElevenMultiplexer;
using SharpNeat.Tasks.BinaryTwentyMultiplexer;
using SharpNeat.Tasks.GenerativeFunctionRegression;

namespace EfficacySampler;

sealed class Program
{
    static StreamWriter? __streamWriter;

    #region Main Entry Point

    static void Main(string[] args)
    {
        // Intercept termination of the console app, to flush and close the output file stream
        // (apparently the 'finally' block below is not executed if the app is terminated with Ctrl-C).
        Console.CancelKeyPress += delegate
        {
            __streamWriter?.Close();
        };

        // Read command line arguments.
        StopCondition? stopCond = ArgUtils.ReadArgs(args, out string? experimentId, out string? filename);
        if(stopCond is null || experimentId is null || filename is null)
            return;

        // Initialise Serilog logging (
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();

        // Create and configure a NEAT experiment instance.
        INeatExperiment<double>? experiment = InitExperiment(experimentId);
        if(experiment is null)
            return;

        // Create an evolution algorithm host.
        IEvolutionAlgorithmHost eaHost = CreateEvolutionAlgorithmHost(experiment, stopCond);

        // Open and initialise the output file.
        __streamWriter = InitOutputFile(filename);
        try
        {
            // Run the main efficacy sampling loop until the process is terminated.
            for(;;)
            {
                Sample s = eaHost.Sample();
                __streamWriter.WriteLine($"{s.ElapsedTimeSecs},{s.GenerationCount},{s.BestFitness:0.#####},{s.MeanFitness:0.#####},{s.MaxComplexity:0.#####},{s.MeanComplexity:0.#####},{s.EvaluationCount}");
                __streamWriter.Flush();
            }
        }
        finally
        {
            __streamWriter?.Close();
            Log.CloseAndFlush();
        }
    }

    #endregion

    #region Private Static Methods [Initialisation]

    private static IEvolutionAlgorithmHost CreateEvolutionAlgorithmHost(
        INeatExperiment<double> experiment,
        StopCondition stopCond)
    {
        return stopCond.StopConditionType switch
        {
            StopConditionType.ElapsedClockTime => new EvolutionAlgorithmHostClockTime(experiment, stopCond.Value),
            StopConditionType.GenerationCount => new EvolutionAlgorithmHostGenerational(experiment, stopCond.Value),
            _ => throw new ArgumentException("Unknown StopConditionType.", nameof(stopCond)),
        };
    }

    private static INeatExperiment<double>? InitExperiment(string experimentId)
    {
        switch(experimentId)
        {
            case "binary11":
                return InitExperiment_BinaryElevenMultiplexer();
            case "binary20":
                return InitExperiment_BinaryTwentyMultiplexer();
            case "sinewave":
                return InitExperiment_Sinewave();
            case "beatsinewave":
                return InitExperiment_BeatSinewave();
        }

        Console.WriteLine($"Unrecognised experiment [{experimentId}]");
        return null;
    }

    private static INeatExperiment<double> InitExperiment_BinaryElevenMultiplexer()
    {
        // Create an instance of INeatExperiment for the binary 11-multiplexer task, configured using the supplied json config.
        var experimentFactory = new BinaryElevenMultiplexerExperimentFactory();
        INeatExperiment<double> neatExperiment = 
            experimentFactory.CreateExperiment("config/binary-eleven-multiplexer.config.json");

        return neatExperiment;
    }

    private static INeatExperiment<double> InitExperiment_BinaryTwentyMultiplexer()
    {
        // Create an instance of INeatExperiment for the binary 20-multiplexer task, configured using the supplied json config.
        var experimentFactory = new BinaryTwentyMultiplexerExperimentFactory();
        INeatExperiment<double> neatExperiment =
            experimentFactory.CreateExperiment("config/binary-twenty-multiplexer.config.json");

        return neatExperiment;
    }

    private static INeatExperiment<double> InitExperiment_Sinewave()
    {
        // Create an instance of INeatExperiment for the generative sinewave task, configured using the supplied json config.
        var experimentFactory = new GenerativeFnRegressionExperimentFactory();
        INeatExperiment<double> neatExperiment =
            experimentFactory.CreateExperiment("config/generative-sinewave.config.json");

        return neatExperiment;
    }

    private static INeatExperiment<double> InitExperiment_BeatSinewave()
    {
        // Create an instance of INeatExperiment for the generative beat-sinewave task, configured using the supplied json config.
        var experimentFactory = new GenerativeFnRegressionExperimentFactory();
        INeatExperiment<double> neatExperiment =
            experimentFactory.CreateExperiment("config/generative-beat-sinewave.config.json");

        return neatExperiment;
    }

    #endregion

    #region Private Static Methods

    private static StreamWriter InitOutputFile(string filename)
    {
        FileInfo fileInfo = new(filename);
        if(fileInfo.Exists)
        {
            // Append to existing file.
            return new StreamWriter(filename, true);
        }

        // Create new file and write a CSV header row.
        StreamWriter sw = new(filename);
        sw.WriteLine("secs,gens,bestfitness,meanfitness,maxcomplexity,meancomplexity,evalcount");
        return sw;
    }

    #endregion
}
