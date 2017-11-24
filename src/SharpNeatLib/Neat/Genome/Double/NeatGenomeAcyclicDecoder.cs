using System;
using SharpNeat.Core;
using SharpNeat.Neat.Network;
using SharpNeat.Network.Acyclic;
using SharpNeat.NeuralNets.Double;
using SharpNeat.Phenomes;

namespace SharpNeat.Neat.Genome.Double
{
    public class NeatGenomeAcyclicDecoder : IGenomeDecoder<NeatGenome<double>,IPhenome<double>>
    {
        bool _boundedOutput;

        #region Constructor

        public NeatGenomeAcyclicDecoder(bool boundedOutput)
        {
            _boundedOutput = boundedOutput;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode a genome into a working neural network.
        /// </summary>
        /// <param name="neatGenome">The genome to decode.</param>
        /// <param name="boundedOutput">Indicates whether the output nodes should be bounded to the interval [0,1]</param>
        public IPhenome<double> Decode(NeatGenome<double> neatGenome)
        {
            // Basic validation test.
            if(!neatGenome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode a cyclic neat genome into a acyclic neural network", "neatGenome");
            }

            // Create a WeightedDirectedGraph representation of the neural net.
            MetaNeatGenome<double> meta = neatGenome.MetaNeatGenome;
            WeightedAcyclicDirectedGraph<double> digraph = NeatAcyclicDirectedGraphFactory<double>.Create(
                                                        neatGenome.ConnectionGenes,
                                                        meta.InputNodeCount,
                                                        meta.OutputNodeCount);

            // Create a working neural net.
            var neuralNet = new AcyclicNeuralNet(digraph, meta.ActivationFn.Fn, _boundedOutput);
            return neuralNet;
        }

        #endregion
    }
}
