using System.Collections.Generic;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Speciation
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
        // Species centroid.
        ConnectionGenes<T> _centroid;

        // The genomes that are within the species.
        readonly List<NeatGenome<T>> _genomeList;
        // Working dictionary of genomes keyed by genome ID.
        readonly Dictionary<int,NeatGenome<T>> _genomeById;
        // Working list of genomes to be added to _genomeById at the end of a k-means iteration.
        readonly List<NeatGenome<T>> _pendingGenomeList;
        // Working list of genome IDs to remove from _genomeById at the end of a k-means iteration.
        public List<int> _removeIdList;

        #endregion

        #region Constructor

        public Species(int id, ConnectionGenes<T> centroid, int capacity = 0)
        {
            _id = id;
            _centroid = centroid;
            _genomeList = new List<NeatGenome<T>>(capacity);
            _genomeById = new Dictionary<int,NeatGenome<T>>(capacity);
            _pendingGenomeList = new List<NeatGenome<T>>(capacity);
            _removeIdList = new List<int>(capacity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Species ID.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// Species centroid.
        /// </summary>
        public ConnectionGenes<T> Centroid { get =>_centroid; set => _centroid = value; }

        /// <summary>
        /// The genomes that are within the species.
        /// </summary>
        public List<NeatGenome<T>> GenomeList => _genomeList;

        /// <summary>
        /// A working dictionary of genomes keyed by ID.
        /// </summary>
        public Dictionary<int,NeatGenome<T>> GenomeById => _genomeById;

        /// <summary>
        /// Working list of genomes to be added to GenomeById at the end of a k-means iteration.
        /// </summary>
        public List<NeatGenome<T>> PendingAddsList => _pendingGenomeList;

        /// <summary>
        /// Working list of genome IDs to remove from GenomeById at the end of a k-means iteration.
        /// </summary>
        public List<int> PendingRemovesList => _removeIdList;

        #endregion

        #region Public Methods

        /// <summary>
        /// Transfer genomes from GenomeList into GenomeById.
        /// </summary>
        public void LoadWorkingDictionary()
        {
            _genomeById.Clear();
            foreach(var genome in _genomeList) {
                _genomeById.Add(genome.Id, genome);
            }
            _genomeList.Clear();
        }

        /// <summary>
        /// Transfer genomes from GenomeById into GenomeList.
        /// </summary>
        public void FlushWorkingDictionary()
        {
            _genomeList.Clear();
            _genomeList.AddRange(_genomeById.Values);
            _genomeById.Clear();
        }

        /// <summary>
        /// Complete all pending genome moves for this species.
        /// </summary>
        public void CompletePendingMoves()
        {
            // Remove genomes that are marked for removal.
            foreach(int id in _removeIdList) {
                _genomeById.Remove(id);
            }

            // Process pending additions.
            foreach(var genome in _pendingGenomeList) {
                _genomeById.Add(genome.Id, genome);
            }

            _removeIdList.Clear();
            _pendingGenomeList.Clear();
        }

        #endregion
    }
}
