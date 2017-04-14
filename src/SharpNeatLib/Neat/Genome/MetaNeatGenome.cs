using System;

namespace SharpNeat.Neat.Genome
{
    public class MetaNeatGenome
    {
        #region Auto Properties

        public int InputNodeCount { get; set; }
        public int OutputNodeCount { get; set; }

        /// <summary>
        /// Connection weight range to use in NEAT genomes. E.g. a value of 5 defines a weight range of -5 to 5.
        /// The weight range is strictly enforced - e.g. when creating new connections and mutating existing ones.
        /// </summary>
        public double ConnectionWeightRange { get; set; } = 5.0;

        /// <summary>
        /// Indicates if NEAT should produce only feed-forward networks (no recurrent/cyclic connection paths).
        /// </summary>
        public bool FeedforwardOnly { get; set; }

        /// <summary>
        /// The neuron activation function to use in evolved networks. NEAT uses the same activation
        /// function at each node.
        /// </summary>
        public Func<double,double> ActivationFn { get; set; }

        #endregion
    }
}
