using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Genomes.Neat.Settings.Mutation;

namespace SharpNeat.Genomes.Neat.Settings
{
    /// <summary>
    /// Represents parameters specific to NEAT genomes. E.g. parameters that describe probabilities
    /// for the different types of mutation and the proportion of possible connections to instantiate 
    /// between input and output neurons within the initial population.
    /// </summary>
    public class NeatGenomeSettings
    {
        #region Auto Properties [High Level Settings]

        /// <summary>
        /// Indicates if NEAT should produce only feed-forward networks (no recurrent/cyclic connection paths).
        /// </summary>
        public bool FeedforwardOnly { get; set; }

        /// <summary>
        /// The neuron activation function to use in evolved networks. NEAT uses the same activation
        /// function at each node.
        /// </summary>
        public Func<double,double> ActivationFn { get; set; }

        /// <summary>
        /// Connection weight range to use in NEAT genomes. E.g. a value of 5 defines a weight range of -5 to 5.
        /// The weight range is strictly enforced - e.g. when creating new connections and mutating existing ones.
        /// </summary>
        public double ConnectionWeightRange { get; set; } = 5.0;

        /// <summary>
        /// A proportion that specifies the number of interconnections to make between input and output neurons in 
        /// an initial random population. This is a proportion of the total number of possible interconnections.
        /// </summary>
        public double InitialInterconnectionsProportion { get; set; } = 0.05;

        /// <summary>
        /// Probability that all excess and disjoint genes are copied into an offspring genome during sexual reproduction. 
        /// Currently the excess/disjoint genes are copied in an 'all or nothing' strategy.
        /// </summary>
        public double DisjointExcessGenesRecombinedProbability { get; set; } = 0.1;

        #endregion

        #region Auto Properties [Genome Mutation Settings]

        /// <summary>
        /// Probability that a genome mutation is a connection weights mutation.
        /// </summary>
        public double ConnectionWeightMutationProbability { get; set; } = 0.94;

        /// <summary>
        /// Probability that a genome mutation is an 'add node' mutation.
        /// </summary>
        public double AddNodeMutationProbability { get; set; } = 0.01;

        /// <summary>
        /// Probability that a genome mutation is an 'add connection' mutation.
        /// </summary>
        public double AddConnectionMutationProbability { get; set; } = 0.025;

        /// <summary>
        /// Probability that a genome mutation is a 'delete connection' mutation.
        /// </summary>
        public double DeleteConnectionMutationProbability { get; set; } = 0.025;

        /// <summary>
        /// Connection weight mutation scheme.
        /// </summary>
        public ConnectionMutationScheme ConnectionMutationScheme { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NeatGenomeSettings()
        {
            this.ConnectionMutationScheme =  ConnectionMutationScheme.CreateDefault();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatGenomeSettings(NeatGenomeSettings copyFrom)
        {
            this.FeedforwardOnly = copyFrom.FeedforwardOnly;
            this.ActivationFn = copyFrom.ActivationFn;
            this.ConnectionWeightRange = copyFrom.ConnectionWeightRange;
            this.InitialInterconnectionsProportion = copyFrom.InitialInterconnectionsProportion;
            this.DisjointExcessGenesRecombinedProbability = copyFrom.DisjointExcessGenesRecombinedProbability;
            this.ConnectionWeightMutationProbability = copyFrom.ConnectionWeightMutationProbability;
            this.AddNodeMutationProbability = copyFrom.AddNodeMutationProbability;
            this.AddConnectionMutationProbability = copyFrom.AddConnectionMutationProbability;
            this.DeleteConnectionMutationProbability = copyFrom.DeleteConnectionMutationProbability;
            this.ConnectionMutationScheme = copyFrom.ConnectionMutationScheme;
        }

        #endregion
    }
}
