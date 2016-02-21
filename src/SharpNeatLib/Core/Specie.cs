/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;

namespace SharpNeat.Core
{
    /// <summary>
    /// Represents a single specie within a speciated population.
    /// </summary>
    public class Specie<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        readonly uint _id;
        int _idx;
        readonly List<TGenome> _genomeList;
        CoordinateVector _centroid;
        
        #region Constructors

        /// <summary>
        /// Construct a specie with the specified ID and index in its parent list/array.
        /// </summary>
        public Specie(uint id, int idx)
        {
            _id = id;
            _idx = idx;
            _genomeList = new List<TGenome>();
        }

        /// <summary>
        /// Construct a specie with the specified ID, index; and creates an empty genome list with a specified capacity.
        /// </summary>
        public Specie(uint id, int idx, int capacity)
        {
            _id = id;
            _idx = idx;
            _genomeList = new List<TGenome>(capacity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the specie's ID. Specie IDs are unique within a population.
        /// </summary>
        public uint Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets or sets the index of the specie in its containing list. This is a working variable
        /// that typically will be the same as the specie ID but is not guaranteed to be e.g. in a distributed NEAT 
        /// environment where specie IDs may become non-contiguous.
        /// </summary>
        public int Idx
        {
            get { return _idx; }
            set { _idx = value; }
        }

        /// <summary>
        /// Gets the list of all genomes in the specie.
        /// </summary>
        public List<TGenome> GenomeList
        {
            get { return _genomeList; }
        }
        
        /// <summary>
        /// Gets or sets the centroid position for all genomes within the specie. Note that this may be out of 
        /// date, it is the responsibility of code external to this class to recalculate and set a new centroid
        /// if the set of genomes in the specie has changed and therefore the specieList centroid has also changed.
        /// </summary>
        public CoordinateVector Centroid
        {
            get { return _centroid; }
            set { _centroid = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the total fitness of all genomes within the specie.
        /// Implemented as a method rather than a property as an indication that this method does significant
        /// work to calculate the value.
        /// </summary>
        public double CalcTotalFitness()
        {
            double total = 0.0;
            foreach(TGenome genome in _genomeList) {
                total += genome.EvaluationInfo.Fitness;
            }
            return total;
        }

        /// <summary>
        /// Calculates the mean fitness of genomes within the specie.
        /// Implemented as a method rather than a property as an indication that this method does significant
        /// work to calculate the value.        
        /// </summary>
        public double CalcMeanFitness()
        {
            return CalcTotalFitness() / (double)_genomeList.Count;
        }        

        /// <summary>
        /// Calculates the total complexity of all genomes within the specie.
        /// Implemented as a method rather than a property as an indication that this method does significant
        /// work to calculate the value.
        /// </summary>
        public double CalcTotalComplexity()
        {
            double total = 0.0;
            foreach(TGenome genome in _genomeList) {
                total += genome.Complexity;
            }
            return total;
        }

        /// <summary>
        /// Calculates the mean complexity of genomes within the specie.
        /// Implemented as a method rather than a property as an indication that this method does significant
        /// work to calculate the value.        
        /// </summary>
        public double CalcMeanComplexity()
        {
            return CalcTotalComplexity() / (double)_genomeList.Count;
        }  

        #endregion
    }
}
