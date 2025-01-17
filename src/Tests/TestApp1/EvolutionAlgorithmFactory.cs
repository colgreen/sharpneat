﻿using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Tasks.BinaryElevenMultiplexer;
using SharpNeat.Tasks.BinarySixMultiplexer;
using SharpNeat.Tasks.CartPole.DoublePole;
using SharpNeat.Tasks.CartPole.SinglePole;
using SharpNeat.Tasks.GenerativeFunctionRegression;
using SharpNeat.Tasks.PreyCapture;
using SharpNeat.Tasks.Xor;

namespace TestApp1;

public static class EvolutionAlgorithmFactory
{
    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_Xor()
    {
        var experimentFactory = new XorExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_Binary6()
    {
        var experimentFactory = new BinarySixMultiplexerExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_Binary11()
    {
        var experimentFactory = new BinaryElevenMultiplexerExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_GenerativeSinewave()
    {
        var experimentFactory = new GenerativeFnRegressionExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_CartSinglePole()
    {
        var experimentFactory = new CartSinglePoleExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_CartDoublePole()
    {
        var experimentFactory = new CartDoublePoleExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    public static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm_PreyCapture()
    {
        var experimentFactory = new PreyCaptureExperimentFactory();
        return CreateNeatEvolutionAlgorithm(experimentFactory);
    }

    private static NeatEvolutionAlgorithm<float> CreateNeatEvolutionAlgorithm(
        INeatExperimentFactory experimentFactory)
    {
        string jsonConfigFilename = $"experiments-config/{experimentFactory.Id}.config.json";

        // Create an instance of INeatExperiment, configured using the supplied json config.
        INeatExperiment<float> neatExperiment = experimentFactory.CreateExperiment<float>(jsonConfigFilename);

        // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
        var ea = NeatEvolutionAlgorithmFactory.CreateEvolutionAlgorithm(neatExperiment);
        return ea;
    }
}
