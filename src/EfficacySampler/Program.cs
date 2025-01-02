using System.Globalization;
using System.Numerics;
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
        var experiment = InitExperiment<float>(experimentId);
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
                __streamWriter.WriteLine($"{s.ElapsedTimeSecs:0.00},{s.GenerationCount},{s.BestFitness:0.#####},{s.MeanFitness:0.#####},{s.MaxComplexity:0.#####},{s.MeanComplexity:0.#####},{s.EvaluationCount}");
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

    private static IEvolutionAlgorithmHost CreateEvolutionAlgorithmHost<TScalar>(
        INeatExperiment<TScalar> experiment,
        StopCondition stopCond)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        return stopCond.StopConditionType switch
        {
            StopConditionType.ElapsedClockTime => new EvolutionAlgorithmHostClockTime<TScalar>(experiment, stopCond.Value),
            StopConditionType.GenerationCount => new EvolutionAlgorithmHostGenerational<TScalar>(experiment, stopCond.Value),
            _ => throw new ArgumentException("Unknown StopConditionType.", nameof(stopCond)),
        };
    }

    private static INeatExperiment<TScalar>? InitExperiment<TScalar>(string experimentId)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        switch(experimentId)
        {
            case "binary11":
                return InitExperiment_BinaryElevenMultiplexer<TScalar>();
            case "binary20":
                return InitExperiment_BinaryTwentyMultiplexer<TScalar>();
            case "sinewave":
                return InitExperiment_Sinewave<TScalar>();
            case "beatsinewave":
                return InitExperiment_BeatSinewave<TScalar>();
        }

        Console.WriteLine($"Unrecognised experiment [{experimentId}]");
        return null;
    }

    private static INeatExperiment<TScalar> InitExperiment_BinaryElevenMultiplexer<TScalar>()
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create an instance of INeatExperiment for the binary 11-multiplexer task, configured using the supplied json config.
        var experimentFactory = new BinaryElevenMultiplexerExperimentFactory();
        INeatExperiment<TScalar> neatExperiment = 
            experimentFactory.CreateExperiment<TScalar>("config/binary-eleven-multiplexer.config.json");

        return neatExperiment;
    }

    private static INeatExperiment<TScalar> InitExperiment_BinaryTwentyMultiplexer<TScalar>()
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create an instance of INeatExperiment for the binary 20-multiplexer task, configured using the supplied json config.
        var experimentFactory = new BinaryTwentyMultiplexerExperimentFactory();
        INeatExperiment<TScalar> neatExperiment =
            experimentFactory.CreateExperiment<TScalar>("config/binary-twenty-multiplexer.config.json");

        return neatExperiment;
    }

    private static INeatExperiment<TScalar> InitExperiment_Sinewave<TScalar>()
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create an instance of INeatExperiment for the generative sinewave task, configured using the supplied json config.
        var experimentFactory = new GenerativeFnRegressionExperimentFactory();
        INeatExperiment<TScalar> neatExperiment =
            experimentFactory.CreateExperiment<TScalar>("config/generative-sinewave.config.json");

        return neatExperiment;
    }

    private static INeatExperiment<TScalar> InitExperiment_BeatSinewave<TScalar>()
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create an instance of INeatExperiment for the generative beat-sinewave task, configured using the supplied json config.
        var experimentFactory = new GenerativeFnRegressionExperimentFactory();
        INeatExperiment<TScalar> neatExperiment =
            experimentFactory.CreateExperiment<TScalar>("config/generative-beat-sinewave.config.json");

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
