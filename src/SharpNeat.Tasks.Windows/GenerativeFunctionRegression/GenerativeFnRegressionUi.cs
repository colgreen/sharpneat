// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.Neat.Genome.Decoders;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.Windows.FunctionRegression;
using SharpNeat.Windows;
using SharpNeat.Windows.Neat;

namespace SharpNeat.Tasks.Windows.GenerativeFunctionRegression;

public sealed class GenerativeFnRegressionUi : NeatExperimentUi
{
    readonly INeatExperiment<float> _neatExperiment;
    readonly Func<float, float> _fn;
    readonly ParamSamplingInfo<float> _paramSamplingInfo;

    public GenerativeFnRegressionUi(
        INeatExperiment<float> neatExperiment,
        Func<float, float> fn,
        ParamSamplingInfo<float> paramSamplingInfo)
    {
        _neatExperiment = neatExperiment ?? throw new ArgumentNullException(nameof(neatExperiment));
        _fn = fn;
        _paramSamplingInfo = paramSamplingInfo;
    }

    /// <inheritdoc/>
    public override GenomeControl CreateTaskControl()
    {
        var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder<float>(
            _neatExperiment.IsAcyclic,
            _neatExperiment.EnableHardwareAcceleratedNeuralNets);

        return new FnRegressionControl(
            _fn,
            _paramSamplingInfo,
            true,
            genomeDecoder);
    }
}
