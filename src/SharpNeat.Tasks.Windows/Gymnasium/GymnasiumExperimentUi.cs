using SharpNeat.Experiments;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Windows;
using SharpNeat.Windows.Neat;

namespace SharpNeat.Tasks.Windows.Gymnasium;

public sealed class GymnasiumExperimentUi : NeatExperimentUi
{
    readonly INeatExperiment<double> _neatExperiment;

    public GymnasiumExperimentUi(
        INeatExperiment<double> neatExperiment)
    {
        _neatExperiment = neatExperiment ?? throw new ArgumentNullException(nameof(neatExperiment));
    }

    /// <inheritdoc/>
    public override GenomeControl CreateTaskControl()
    {
        var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder(
            _neatExperiment.IsAcyclic,
            _neatExperiment.EnableHardwareAcceleratedNeuralNets);

        return new GymnasiumControl(genomeDecoder);
    }
}
