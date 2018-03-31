using System;
using SharpNeat.Neat.Network;
using SharpNeat.Network;
using SharpNeat.NeuralNets.Double;
using SharpNeat.Phenomes;

namespace SharpNeat.Neat.Genome.Double
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
        public static IPhenome<double> Decode(NeatGenome<double> neatGenome, int activationCount, bool boundedOutput)
        {
            // Basic validation test.
            // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but that's almost certainly 
            // not what the caller wanted to do.
            if(neatGenome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode an acyclic neat genome into a cyclic neural network", "neatGenome");
            }

            // Create a WeightedDirectedGraph representation of the neural net.
            WeightedDirectedGraph<double> digraph = NeatDirectedGraphFactory<double>.Create(neatGenome);

            // Create a working neural net.
            var neuralNet = new CyclicNeuralNet(
                digraph,
                neatGenome.MetaNeatGenome.ActivationFn.Fn,
                activationCount, boundedOutput);

            return neuralNet;
        }
    }
}
