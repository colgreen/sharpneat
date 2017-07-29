using System;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.NeuralNets;

namespace SharpNeat.Neat.Genome
{
    public static class NeatGenomeCyclicDecoder
    {
        /// <summary>
        /// Decode a genome into a working neural network.
        /// </summary>
        /// <param name="neatGenome">The genome to decode.</param>
        /// <param name="activationCount">The number of activations of the cyclic network to perform per 
        /// invocation of the neural net as a whole.</param>
        /// <param name="boundedOutput">Indicates whether the output nodes should be bounded to the interval [0,1]</param>
        public static IPhenome<double> Decode(NeatGenome neatGenome, int activationCount, bool boundedOutput)
        {
            // Basic validation test.
            // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but that's almost certainly 
            // not what the caller wanted to do.
            if(neatGenome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode an acyclic neat genome into a cyclic neural network", "neatGenome");
            }

            // Create a WeightedDirectedGraph representation of the neural net.
            MetaNeatGenome meta = neatGenome.MetaNeatGenome;
            WeightedDirectedGraph<double> digraph = WeightedDirectedGraphFactory<double>.Create(
                                                        neatGenome.ConnectionGeneList,
                                                        meta.InputNodeCount,
                                                        meta.OutputNodeCount);

            // Create a working neural net.
            CyclicNeuralNet neuralNet = new CyclicNeuralNet(
                                        digraph,
                                        meta.ActivationFn.Fn,
                                        activationCount, boundedOutput);
            return neuralNet;
        }
    }
}
