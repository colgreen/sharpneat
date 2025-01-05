// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Neat.Genome.Decoders;
using SharpNeat.Tasks.PreyCapture;
using SharpNeat.Tasks.PreyCapture.ConfigModels;
using SharpNeat.Windows;
using SharpNeat.Windows.Neat;

namespace SharpNeat.Tasks.Windows.PreyCapture;

/// <summary>
/// Implementation of <see cref="IExperimentUi"/> for the Prey Capture task.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public sealed class PreyCaptureExperimentUi<TScalar> : NeatExperimentUi
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly INeatExperiment<TScalar> _neatExperiment;
    readonly PreyCaptureCustomConfig _customConfig;

    public PreyCaptureExperimentUi(
        INeatExperiment<TScalar> neatExperiment,
        PreyCaptureCustomConfig customConfig)
    {
        _neatExperiment = neatExperiment ?? throw new ArgumentNullException(nameof(neatExperiment));
        _customConfig = customConfig ?? throw new ArgumentNullException(nameof(customConfig));
    }

    /// <inheritdoc/>
    public override GenomeControl CreateTaskControl()
    {
        PreyCaptureWorld<TScalar> world = new(
            _customConfig.PreyInitMoves,
            _customConfig.PreySpeed,
            _customConfig.SensorRange,
            _customConfig.MaxTimesteps);

        var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder<TScalar>(
            _neatExperiment.IsAcyclic,
            _neatExperiment.EnableHardwareAcceleratedNeuralNets);

        return new GenericPreyCaptureControl<TScalar>(genomeDecoder, world);
    }
}
