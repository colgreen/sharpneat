using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpNeat.Core
{
    /// <summary>
    /// Represents a population of genomes.
    /// </summary>
    /// <typeparam name="TGenome"></typeparam>
    public class Population<TGenome>
    {
        IList<TGenome> _genomeList;
        IList<TGenome> _genomeListRO;
        int _populationSize;
        double _totalFitness;
        double _meanFitness;
        double _maxFitnessEver;
        double _meanComplexity;
        double _maxComplexityEver;

        uint _genAtLastImprovement;

        #region Constructor

        /// <summary>
        /// Construct with the provided genome list.
        /// </summary>
        /// <param name="genomeList"></param>
        public Population(IList<TGenome> genomeList)
        {
            _genomeList = genomeList;
            _genomeListRO = new ReadOnlyCollection<TGenome>(_genomeList);
            _populationSize = genomeList.Count;
        }

        /// <summary>
        /// Construct with a random genome population created by the provided factory.
        /// </summary>
        /// <param name="genomeFactory"></param>
        /// <param name="populationSize"></param>
        public Population(IGenomeFactory<TGenome> genomeFactory, int populationSize)
        {
            _genomeList = genomeFactory.CreateGenomeList(populationSize, 0);
            _genomeListRO = new ReadOnlyCollection<TGenome>(_genomeList);
            _populationSize = populationSize;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the full list of genomes in this population. This is a read-only wrapper around the true 
        /// genome list.
        /// </summary>
        public IList<TGenome> GenomeList
        {
            get { return _genomeListRO; }
        }

        /// <summary>
        /// Gets the size of the population.
        /// </summary>
        public int PopulationSize
        {
            get { return _populationSize; }
        }

        /// <summary>
        /// Gets the sum total fitness of all genomes in the population.
        /// </summary>
        public double TotalFitness
        {
            get { return _totalFitness; }
            set { _totalFitness=value; }
        }

        /// <summary>
        /// Gets the arithmetic mean fitness of genomes in the population.
        /// </summary>
        public double MeanFitness
        {
            get { return _meanFitness; }
            set { _meanFitness=value; }
        }

        /// <summary>
        /// Gets the maximum fitness that has occured in this population in its lifetime.
        /// </summary>
        public double MaxFitnessEver
        {
            get { return _maxFitnessEver; }
            set { _maxFitnessEver=value; }
        }

        /// <summary>
        /// Gets the mean complexity of genomes in the population.
        /// </summary>
        public double MeanComplexity
        {
            get { return _meanComplexity; }
        }

        /// <summary>
        /// Gets the maximum genome complexity that has occured in this population in its lifetime.
        /// </summary>
        public double MaxComplexityEver
        {
            get { return _maxComplexityEver; }
        }

        /// <summary>
        /// Gets the generation 
        /// </summary>
        public uint GenerationAtLastImprovement
        {
            get { return _genAtLastImprovement; }
            set { _genAtLastImprovement=value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a new genome to the population.
        /// </summary>
        /// <param name="genome"></param>
        public virtual void AddGenomeToPopulation(TGenome genome)
        {
            _genomeList.Add(genome);
        }

        #endregion
    }
}
