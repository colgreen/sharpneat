using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// A gene that represents a single neuron in NEAT.
    /// </summary>
    public class NeuronGene
    {
        #region Auto Properties

        /// <summary>
        /// Although this ID is allocated from the global innovation ID pool, neurons do not participate 
        /// in compatibility measurements and so it is not actually used as an innovation ID. 
        /// </summary>
        public uint InnovationId { get; private set; }
        public NodeType NodeType { get; private set; }
        public int ActivationFnId { get; private set; }
        public HashSet<uint> SourceNeurons { get; private set; }
        public HashSet<uint> TargetNeurons { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copyFrom">NeuronGene to copy from.</param>
        /// <param name="copyConnectivityData">Indicates whether or not tp copy connectivity data for the neuron.</param>
        public NeuronGene(NeuronGene copyFrom, bool copyConnectivityData)
        {
            this.InnovationId = copyFrom.InnovationId;
            this.NodeType = copyFrom.NodeType;
            this.ActivationFnId = copyFrom.ActivationFnId;

            if(copyConnectivityData) {
                this.SourceNeurons = new HashSet<uint>(copyFrom.SourceNeurons);
                this.TargetNeurons = new HashSet<uint>(copyFrom.TargetNeurons);
            } else {
                this.SourceNeurons = new HashSet<uint>();
                this.TargetNeurons = new HashSet<uint>();
            }
        }

        /// <summary>
        /// Construct new NeuronGene with the specified innovationId, neuron type 
        /// and activation function ID.
        /// </summary>
        public NeuronGene(uint innovationId, NodeType neuronType, int activationFnId)
        {
            this.InnovationId = innovationId;
            this.NodeType = neuronType;
            this.ActivationFnId = activationFnId;
            this.SourceNeurons = new HashSet<uint>();
            this.TargetNeurons = new HashSet<uint>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overridden by sub-types.
        /// </summary>
        public virtual NeuronGene CreateCopy()
        {
            return new NeuronGene(this, true);
        }

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overridden by sub-types.
        /// </summary>
        /// <param name="copyConnectivityData">Indicates whether or not top copy connectivity data for the neuron.</param>
        public virtual NeuronGene CreateCopy(bool copyConnectivityData)
        {
            return new NeuronGene(this, copyConnectivityData);
        }

        #endregion
    }
}
