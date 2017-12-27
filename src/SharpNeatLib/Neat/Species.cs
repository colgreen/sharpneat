using System.Collections.Generic;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat
{
    /// <summary>
    /// Represents a NEAT species.
    /// In NEAT all genome are within a species.
    /// </summary>
    /// <typeparam name="T">Genome weight type.</typeparam>
    public class Species<T> where T : struct
    {
        #region Instance Fields

        // Species ID.
        readonly int _id;
        // The genomes that are within the species.
        readonly List<NeatGenome<T>> _genomeList;
        // Species centroid.
        ConnectionGenes<T> _centroid;

        #endregion

        #region Constructor

        public Species(int id)
        {
            _id = id;
            _genomeList = new List<NeatGenome<T>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Species ID.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// The genomes that are within the species.
        /// </summary>
        public List<NeatGenome<T>> GenomeList => _genomeList;

        /// <summary>
        /// Species centroid.
        /// </summary>
        public ConnectionGenes<T> Centroid { get =>_centroid; set => _centroid = value; }

        #endregion
    }
}
