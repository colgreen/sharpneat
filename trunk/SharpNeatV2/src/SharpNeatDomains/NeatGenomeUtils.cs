using System.Collections.Generic;
using System.Xml;
using log4net;
using SharpNeat.Genomes.Neat;

namespace SharpNeat.Domains
{
    /// <summary>
    /// Static helper methods for NEAT genomes.
    /// </summary>
    public static class NeatGenomeUtils
    {
        private static readonly ILog __log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
        /// The genome factory for the genomes can be obtained from any one of the genomes.
        /// </summary>
        /// <param name="xr">The XmlReader to read from.</param>
        /// <param name="nodeFnIds">Flaf that indicates if each node's activation function attribute should be read. NEAT genome's have a fixed activation function and therefore don't use this attribute.
        /// Use true for HyperNEAT genomes (CPPNS).</param>
        /// <param name="inputCount">The number of input nodes we are expecting on loaded genomes.</param>
        /// <param name="outputCount">The number of output nodes we are expecting on loaded genomes.</param>
        public static List<NeatGenome> LoadPopulation(XmlReader xr, bool nodeFnIds, int inputCount, int outputCount)
        {
            // Load genomes. Reading node IDs is not necessary for NEAT.
            List<NeatGenome> genomeList = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, nodeFnIds);

            // Check the number of inputs/outputs matches up between genomes and the experiment.
            int count = genomeList.Count;
            for(int i=count-1; i>-1; i--)
            {
                NeatGenome genome = genomeList[i];
                if(genome.InputNodeCount != inputCount) {
                    __log.ErrorFormat("Loaded genome has wrong number of inputs for currently selected experiment. Has [{0}], expected [{1}].", genome.InputNodeCount, inputCount);
                    genomeList.RemoveAt(i);
                    continue;
                }
                if(genome.OutputNodeCount != outputCount) {
                    __log.ErrorFormat("Loaded genome has wrong number of outputs for currently selected experiment. Has [{0}], expected [{1}].", genome.OutputNodeCount, outputCount);
                    genomeList.RemoveAt(i);
                    continue;
                }
            }

            // Return the genome list.
            return genomeList;
        }
    }
}
