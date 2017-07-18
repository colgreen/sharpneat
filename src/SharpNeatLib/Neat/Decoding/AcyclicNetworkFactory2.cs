using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Network.Analysis;
using SharpNeat.Network2;
using SharpNeat.Phenomes.NeuralNets.Acyclic;

namespace SharpNeat.Neat.Decoding
{



    public class AcyclicNetworkFactory
    {


        #region Public Static Methods

        public static AcyclicNetwork CreateAcyclicNetwork(NeatGenome neatGenome, bool boundedOutput)
        {



        }


        #endregion



        #region Private Static Methods

        private static void InternalDecode(
            NeatGenome genome,
            out IActivationFunction<double>[] activationFnArr,
            out WeightedDirectedGraph<double> digraph,
            out LayerInfo[] layerInfoArr,
            out int[] outputNeuronIdxArr)
        {
            // Translate the neat genome connection graph into a WeightedDirectGraph.
            digraph = NeatGenomeDirectedGraphFactory.Create(genome);

            // Assert that the graph is acyclic.
            Debug.Assert(!CyclicGraphTest.IsCyclic(digraph), "Attempt to decode a cyclic network definition into an AcyclicNetwork.");

            // Determine the depth of each node in the network. 
            // Node depths are used to separate the nodes into depth based layers, these layers can then be
            // used to determine the order in which signals are propagated through the network.
            NetworkDepthInfo netDepthInfo = AcyclicGraphDepthAnalysis.CalculateNodeDepths(digraph, Enumerable.Range(0, genome.MetaNeatGenome.InputNodeCount));



        }



        #endregion


    }




}
