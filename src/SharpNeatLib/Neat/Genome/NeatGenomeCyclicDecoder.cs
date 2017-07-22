using System;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Network2;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets.Cyclic;

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
        /// <returns></returns>
        public static IBlackBox<double> Decode(NeatGenome neatGenome, int activationCount, bool boundedOutput)
        {
            // Basic validation test.
            // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but that's almost certainly 
            // not what the caller wanted to do.
            if(neatGenome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode an acyclic neat genome into a cyclic neural network", "neatGenome");
            }

            // Define an IEnumerable for the fixed node IDs, i.e. all of the input and output nodes.
            MetaNeatGenome meta = neatGenome.MetaNeatGenome;
            int inputOutputCount = meta.InputNodeCount + meta.OutputNodeCount;
            IEnumerable<int> fixedNodeIds =  Enumerable.Range(0, inputOutputCount);

            // Create a WeightedDirectedGraph representation of the neural net.
            WeightedDirectedGraph<double> digraph = WeightedDirectedGraphFactory<double>.Create(
                                                        neatGenome.ConnectionGeneList,
                                                        fixedNodeIds);

            // Create a working neural net.
            CyclicNeuralNet neuralNet = new CyclicNeuralNet(
                                        digraph,
                                        meta.InputNodeCount, meta.OutputNodeCount,
                                        meta.ActivationFn.Fn,
                                        activationCount, boundedOutput);
            return neuralNet;
        }
    }
}
