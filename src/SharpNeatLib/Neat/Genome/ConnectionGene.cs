using SharpNeat.Network;
using SharpNeat.Network2;

namespace SharpNeat.Neat.Genome
{
    // TODO: Consider if this could/should be a struct; or maybe wrap with a struct ConnectionGeneRef class, so we get to use whichever is most appropriate in each subroutine.
    /// <summary>
    /// A gene that represents a single connection between two neurons in NEAT.
    /// </summary>
    public class ConnectionGene : IDirectedConnection
    {
        #region Auto Properties

        public uint Id { get; }
        public int SourceId { get; }
        public int TargetId { get; }
        public double Weight { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ConnectionGene(ConnectionGene copyFrom)
        {
            this.Id = copyFrom.Id;
            this.SourceId = copyFrom.SourceId;
            this.TargetId = copyFrom.TargetId;
            this.Weight = copyFrom.Weight;
        }

        /// <summary>
        /// Construct a new ConnectionGene with the specified source and target neurons and connection weight.
        /// </summary>
        public ConnectionGene(uint id, int sourceId, int targetId, double weight)
        {
            this.Id = id;
            this.SourceId = sourceId;
            this.TargetId = targetId;
            this.Weight = weight;
        }

        ///// <summary>
        ///// Construct a new ConnectionGene with the specified source and target neurons and connection weight.
        ///// </summary>
        //public ConnectionGene(uint id, ConnectionEndpoints connection, double weight)
        //{
        //    this.Id = id;
        //    this.SourceId = connection.SourceId;
        //    this.TargetId = connection.TargetId;
        //    this.Weight = weight;
        //}

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
