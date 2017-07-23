using System;
using SharpNeat.Network2.Acyclic;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets.Acyclic;

namespace SharpNeat.Neat.Genome
{
    public static class NeatGenomeAcyclicDecoder
    {
        /// <summary>
        /// Decode a genome into a working neural network.
        /// </summary>
        /// <param name="neatGenome">The genome to decode.</param>
        /// <param name="boundedOutput">Indicates whether the output nodes should be bounded to the interval [0,1]</param>
        public static IBlackBox<double> Decode(NeatGenome neatGenome, bool boundedOutput)
        {
            // Basic validation test.
            if(!neatGenome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode a cyclic neat genome into a acyclic neural network", "neatGenome");
            }

            // Create a WeightedDirectedGraph representation of the neural net.
            MetaNeatGenome meta = neatGenome.MetaNeatGenome;
            WeightedAcyclicDirectedGraph<double> digraph = WeightedAcyclicDirectedGraphFactory<double>.Create(
                                                        neatGenome.ConnectionGeneList,
                                                        meta.InputNodeCount,
                                                        meta.OutputNodeCount);

            // Create a working neural net.
            AcyclicNeuralNet neuralNet = new AcyclicNeuralNet(digraph, meta.ActivationFn.Fn, boundedOutput);
            return neuralNet;
        }
    }
}
