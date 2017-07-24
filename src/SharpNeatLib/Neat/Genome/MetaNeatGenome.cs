using SharpNeat.NeuralNets;

namespace SharpNeat.Neat.Genome
{
    public class MetaNeatGenome
    {
        #region Auto Properties

        /// <summary>
        /// Input node count.
        /// </summary>
        public int InputNodeCount { get; set; }

        /// <summary>
        /// Output node count.
        /// </summary>
        public int OutputNodeCount { get; set; }

        /// <summary>
        /// Maximum connection weight magnitude. 
        /// E.g. a value of 5 defines a weight range of -5 to 5.
        /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
        /// </summary>
        public double ConnectionWeightRange { get; set; } = 5.0;

        /// <summary>
        /// Indicates if the genomes that are evolved are acyclic, i.e. they should have no recurrent/cyclic connection paths.
        /// </summary>
        public bool IsAcyclic { get; set; }

        /// <summary>
        /// The neuron activation function to use in evolved networks. NEAT uses the same activation
        /// function at each node.
        /// </summary>
        public IActivationFunction<double> ActivationFn { get; set; }

        #endregion
    }
}
