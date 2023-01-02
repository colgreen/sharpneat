// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Domains.FunctionRegression;
using SharpNeat.Experiments;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Windows;
using SharpNeat.Windows.Neat;

namespace SharpNeat.Tasks.Windows.GenerativeFunctionRegression;

public sealed class GenerativeFnRegressionUi : NeatExperimentUi
{
    readonly INeatExperiment<double> _neatExperiment;
    readonly Func<double, double> _fn;
    readonly ParamSamplingInfo _paramSamplingInfo;

    public GenerativeFnRegressionUi(
        INeatExperiment<double> neatExperiment,
        Func<double, double> fn,
        ParamSamplingInfo paramSamplingInfo)
    {
        _neatExperiment = neatExperiment ?? throw new ArgumentNullException(nameof(neatExperiment));
        _fn = fn;
        _paramSamplingInfo = paramSamplingInfo;
    }

    /// <inheritdoc/>
    public override GenomeControl CreateTaskControl()
    {
        var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder(
            _neatExperiment.IsAcyclic,
            _neatExperiment.EnableHardwareAcceleratedNeuralNets);

        return new FnRegressionControl(
            _fn,
            _paramSamplingInfo,
            true,
            genomeDecoder);
    }
}
