using System;
using System.Diagnostics;
using SharpNeat.Evaluation;
using SharpNeat.Neat.Network;
using SharpNeat.Network;
using SharpNeat.NeuralNets.Double;
using SharpNeat.Phenomes;

namespace SharpNeat.Neat.Genome.Double
{
    public class NeatGenomeDecoder : IGenomeDecoder<NeatGenome<double>,IPhenome<double>>
    {
        bool _boundedOutput;
        int _activationCount;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boundedOutput"></param>
        public NeatGenomeDecoder(
            bool boundedOutput,
            int activationCount)
        {
            _boundedOutput = boundedOutput;
            _activationCount = activationCount;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode a genome into a working neural network.
        /// </summary>
        /// <param name="genome">The genome to decode.</param>
        /// <param name="activationCount">The number of activations of the cyclic network to perform per 
        /// invocation of the neural net as a whole.</param>
        /// <param name="boundedOutput">Indicates whether the output nodes should be bounded to the interval [0,1]</param>
        public IPhenome<double> Decode(
            NeatGenome<double> genome)
        {
            // Assert that the connections are sorted.
            Debug.Assert(DirectedConnectionUtils.IsSorted(genome.ConnectionGenes._connArr));

            // Basic validation test.
            // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but that's almost certainly 
            // not what the caller wanted to do.
            if(genome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode an acyclic neat genome into a cyclic neural network", "neatGenome");
            }

            // Create a working neural net.
            var neuralNet = new CyclicNeuralNet(
                genome.DirectedGraph,
                genome.ConnectionGenes._weightArr,
                genome.MetaNeatGenome.ActivationFn.Fn,
                _activationCount, _boundedOutput);

            return neuralNet;
        }

        #endregion
    }
}
