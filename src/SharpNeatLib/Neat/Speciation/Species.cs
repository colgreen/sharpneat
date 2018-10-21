using System.Collections.Generic;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Speciation
{
    /// <summary>
    /// Represents a NEAT species.
    /// In NEAT all genomes are within a species.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class Species<T> where T : struct
    {
        #region Auto Properties

        /// <summary>
        /// Species ID.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Species centroid.
        /// </summary>
        public ConnectionGenes<T> Centroid { get; set; }

        /// <summary>
        /// The genomes that are within the species.
        /// </summary>
        public List<NeatGenome<T>> GenomeList { get; }

        /// <summary>
        /// A working dictionary of genomes keyed by ID.
        /// </summary>
        public Dictionary<int,NeatGenome<T>> GenomeById { get; }

        /// <summary>
        /// Working list of genomes to be added to GenomeById at the end of a k-means iteration.
        /// </summary>
        public List<NeatGenome<T>> PendingAddsList { get; }

        /// <summary>
        /// Working list of genome IDs to remove from GenomeById at the end of a k-means iteration.
        /// </summary>
        public List<int> PendingRemovesList { get; }

        /// <summary>
        /// Species statistics.
        /// </summary>
        public SpeciesStats Stats { get; }

        #endregion

        #region Constructor

        public Species(int id, ConnectionGenes<T> centroid, int capacity = 0)
        {
            this.Id = id;
            this.Centroid = centroid;
            this.GenomeList = new List<NeatGenome<T>>(capacity);
            this.GenomeById = new Dictionary<int,NeatGenome<T>>(capacity);
            this.PendingAddsList = new List<NeatGenome<T>>(capacity);
            this.PendingRemovesList = new List<int>(capacity);
            this.Stats = new SpeciesStats();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transfer genomes from GenomeList into GenomeById.
        /// </summary>
        public void LoadWorkingDictionary()
        {
            GenomeById.Clear();
            foreach(var genome in GenomeList) {
                GenomeById.Add(genome.Id, genome);
            }
            GenomeList.Clear();
        }

        /// <summary>
        /// Transfer genomes from GenomeById into GenomeList.
        /// </summary>
        public void FlushWorkingDictionary()
        {
            GenomeList.Clear();
            GenomeList.AddRange(GenomeById.Values);
            GenomeById.Clear();
        }

        /// <summary>
        /// Complete all pending genome moves for this species.
        /// </summary>
        public void CompletePendingMoves()
        {
            // Remove genomes that are marked for removal.
            foreach(int id in PendingRemovesList) {
                GenomeById.Remove(id);
            }

            // Process pending additions.
            foreach(var genome in PendingAddsList) {
                GenomeById.Add(genome.Id, genome);
            }

            PendingRemovesList.Clear();
            PendingAddsList.Clear();
        }

        #endregion
    }
}
