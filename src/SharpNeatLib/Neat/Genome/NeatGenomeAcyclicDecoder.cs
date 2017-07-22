using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Network2.Acyclic;
using SharpNeat.Phenomes;

namespace SharpNeat.Neat.Genome
{





    public static class NeatGenomeAcyclicDecoder
    {


        public static IBlackBox<double> Decode(NeatGenome neatGenome, bool boundedOutput)
        {
            // Basic validation test.
            if(!neatGenome.MetaNeatGenome.IsAcyclic) {
                throw new ArgumentException("Attempt to decode a cyclic neat genome into a acyclic neural network", "neatGenome");
            }



            // Define an IEnumerable for the fixed node IDs, i.e. all of the input and output nodes.
            MetaNeatGenome meta = neatGenome.MetaNeatGenome;
            int inputOutputCount = meta.InputNodeCount + meta.OutputNodeCount;
            IEnumerable<int> fixedNodeIds =  Enumerable.Range(0, inputOutputCount);


            // TODO:

            //// Create a WeightedAcyclicDirectedGraph representation of the neural net.
            //WeightedAcyclicDirectedGraph<double> digraph = WeightedAcyclicDirectedGraphFactory<double>.Create(
            //                                            neatGenome.ConnectionGeneList,
            //                                            fixedNodeIds);


            return null;
        }
    
    }





}
