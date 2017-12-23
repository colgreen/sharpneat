using SharpNeat.NeuralNets;

namespace SharpNeat.Neat.Genome
{
    public class MetaNeatGenome<T> where T : struct
    {
        #region Auto Properties

        /// <summary>
        /// Input node count.
        /// </summary>
        public int InputNodeCount { get; }

        /// <summary>
        /// Output node count.
        /// </summary>
        public int OutputNodeCount { get; }

        /// <summary>
        /// Indicates if the genomes that are evolved are acyclic, i.e. they should have no recurrent/cyclic connection paths.
        /// </summary>
        public bool IsAcyclic { get; }

        /// <summary>
        /// The neuron activation function to use in evolved networks. NEAT uses the same activation
        /// function at each node.
        /// </summary>
        public IActivationFunction<T> ActivationFn { get; }

        /// <summary>
        /// Maximum connection weight magnitude. 
        /// E.g. a value of 5 defines a weight range of -5 to 5.
        /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
        /// </summary>
        public double ConnectionWeightRange { get; } = 5.0;

        /// <summary>
        /// Sexual reproduction.
        /// The probability that a gene that exists only on the secondary parent is copied into the child genome.
        /// </summary>
        public double SecondaryParentGeneProbability = 0.02;

        #endregion

        #region Properties

        /// <summary>
        /// The total number of input and output nodes.
        /// </summary>
        public int InputOutputNodeCount {
            get { return this.InputNodeCount + this.OutputNodeCount; }
        }

        #endregion

        #region Constructor

        public MetaNeatGenome(int inputNodeCount, int outputNodeCount, bool isAcyclic, IActivationFunction<T> activationFn)
        {
            this.InputNodeCount = inputNodeCount;
            this.OutputNodeCount = outputNodeCount;
            this.IsAcyclic = isAcyclic;
            this.ActivationFn = activationFn;
        }

        #endregion
    }
}
