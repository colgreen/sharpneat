// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Neat.Genome.Decoders;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.Windows.FunctionRegression;
using SharpNeat.Windows;
using SharpNeat.Windows.Neat;

namespace SharpNeat.Tasks.Windows.GenerativeFunctionRegression;

public sealed class GenerativeFnRegressionUi<TScalar> : NeatExperimentUi
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly INeatExperiment<TScalar> _neatExperiment;
    readonly Func<TScalar, TScalar> _fn;
    readonly ParamSamplingInfo<TScalar> _paramSamplingInfo;

    public GenerativeFnRegressionUi(
        INeatExperiment<TScalar> neatExperiment,
        Func<TScalar, TScalar> fn,
        ParamSamplingInfo<TScalar> paramSamplingInfo)
    {
        _neatExperiment = neatExperiment ?? throw new ArgumentNullException(nameof(neatExperiment));
        _fn = fn;
        _paramSamplingInfo = paramSamplingInfo;
    }

    /// <inheritdoc/>
    public override GenomeControl CreateTaskControl()
    {
        var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder<TScalar>(
            _neatExperiment.IsAcyclic,
            _neatExperiment.EnableHardwareAcceleratedNeuralNets);

        return new GenericFnRegressionControl<TScalar>(
            _fn,
            _paramSamplingInfo,
            true,
            genomeDecoder);
    }
}
