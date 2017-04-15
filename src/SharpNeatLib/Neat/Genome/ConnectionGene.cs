
namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// A gene that represents a single connection between two neurons in NEAT.
    /// </summary>
    public class ConnectionGene
    {
        #region Auto Properties

        public uint InnovationId { get; }
        public uint SourceNodeId { get; }
        public uint TargetNodeId { get; }
        public double Weight { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ConnectionGene(ConnectionGene copyFrom)
        {
            this.InnovationId = copyFrom.InnovationId;
            this.SourceNodeId = copyFrom.SourceNodeId;
            this.TargetNodeId = copyFrom.TargetNodeId;
            this.Weight = copyFrom.Weight;
        }

        /// <summary>
        /// Construct a new ConnectionGene with the specified source and target neurons and connection weight.
        /// </summary>
        public ConnectionGene(uint innovationId, uint sourceNodeId, uint targetNodeId, double weight)
        {
            this.InnovationId = innovationId;
            this.SourceNodeId = sourceNodeId;
            this.TargetNodeId = targetNodeId;
            this.Weight = weight;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overridden by sub-types.
        /// </summary>
        public virtual ConnectionGene CreateCopy()
        {
            return new ConnectionGene(this);
        }

        #endregion
    }
}
