using System.Diagnostics;
using System.Numerics;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;
using SharpNeat.Network.Acyclic;
using SharpNeat.NeuralNet.Double;

namespace SharpNeat.Neat.Genome.Double
{
    public class NeatGenomeAcyclicDecoder : IGenomeDecoder<NeatGenome<double>,IBlackBox<double>>
    {
        readonly bool _boundedOutput;

        #region Constructor

        public NeatGenomeAcyclicDecoder(
            bool boundedOutput)
        {
            _boundedOutput = boundedOutput;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode a genome into a working neural network.
        /// </summary>
        /// <param name="genome">The genome to decode.</param>
        /// <param name="boundedOutput">Indicates whether the output nodes should be bounded to the interval [0,1]</param>
        public IBlackBox<double> Decode(
            NeatGenome<double> genome)
        {
            Debug.Assert(genome?.MetaNeatGenome?.IsAcyclic == true);
            Debug.Assert(null != genome?.ConnectionGenes);
            Debug.Assert(genome.ConnectionGenes.Length == genome?.ConnectionIndexMap?.Length);
            Debug.Assert(genome.DirectedGraph is AcyclicDirectedGraph);

            // Create neural net weight array.
            // Note. We cannot use the genome's weight array directly here (as is done in NeatGenomeDecoder,
            // i.e. for cyclic graphs) because the genome connections and digraph connections have a 
            // different order.
            double[] neuralNetWeightArr = CreateNeuralNetWeightArray(genome);

            // Create a working neural net.
            IBlackBox<double> neuralNet;
            if (Vector.IsHardwareAccelerated)
            {
                neuralNet = new NeuralNet.Double.Vectorized.AcyclicNeuralNet(
                    (AcyclicDirectedGraph)genome.DirectedGraph,
                    neuralNetWeightArr,
                    genome.MetaNeatGenome.ActivationFn.Fn,
                    _boundedOutput);

            }
            else
            {
                neuralNet = new AcyclicNeuralNet(
                    (AcyclicDirectedGraph)genome.DirectedGraph,
                    neuralNetWeightArr,
                    genome.MetaNeatGenome.ActivationFn.Fn,
                    _boundedOutput);
            }
            return neuralNet;
        }

        #endregion

        #region Private Static Methods

        private static double[] CreateNeuralNetWeightArray(
            NeatGenome<double> genome)
        {
            // Create a new weight array, and copy in the weights from the genome into their correct positions.
            double[] genomeWeightArr = genome.ConnectionGenes._weightArr;
            double[] neuralNetWeightArr = new double[genomeWeightArr.Length];
            int[] connIdxMap = genome.ConnectionIndexMap;

            for(int i=0; i < connIdxMap.Length; i++) {
                neuralNetWeightArr[i] = genomeWeightArr[connIdxMap[i]];
            }
            return neuralNetWeightArr;
        }

        #endregion
    }
}
