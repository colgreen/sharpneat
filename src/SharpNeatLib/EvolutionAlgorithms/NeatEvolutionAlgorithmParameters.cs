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

namespace SharpNeat.EvolutionAlgorithms
{
    /// <summary>
    /// Parameters specific to the NEAT evolution algorithm.
    /// </summary>
    public class NeatEvolutionAlgorithmParameters
    {
        #region Constants

        const int DefaultSpecieCount = 10;
        const double DefaultElitismProportion = 0.2;
        const double DefaultSelectionProportion = 0.2;

        const double DefaultOffspringAsexualProportion = 0.5;
        const double DefaultOffspringSexualProportion = 0.5;
        const double DefaultInterspeciesMatingProportion = 0.01;

        const int DefaultDestFitnessMovingAverageHistoryLength = 100;
        const int DefgaultMeanSpecieChampFitnessMovingAverageHistoryLength = 100;
        const int DefaultComplexityMovingAverageHistoryLength = 100;

        #endregion

        #region Instance Fields

        int _specieCount;
        double _elitismProportion;
        double _selectionProportion;
        
        double _offspringAsexualProportion;
        double _offspringSexualProportion;
        double _interspeciesMatingProportion;

        int _bestFitnessMovingAverageHistoryLength;
        int _meanSpecieChampFitnessMovingAverageHistoryLength;
        int _complexityMovingAverageHistoryLength;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs with the default parameters.
        /// </summary>
        public NeatEvolutionAlgorithmParameters()
        {
            _specieCount = DefaultSpecieCount;
            _elitismProportion = DefaultElitismProportion;
            _selectionProportion = DefaultSelectionProportion;
            
            _offspringAsexualProportion = DefaultOffspringAsexualProportion;
            _offspringSexualProportion = DefaultOffspringSexualProportion;
            _interspeciesMatingProportion = DefaultInterspeciesMatingProportion;

            _bestFitnessMovingAverageHistoryLength = DefaultDestFitnessMovingAverageHistoryLength;
            _meanSpecieChampFitnessMovingAverageHistoryLength = DefgaultMeanSpecieChampFitnessMovingAverageHistoryLength;
            _complexityMovingAverageHistoryLength = DefaultComplexityMovingAverageHistoryLength;

            NormalizeProportions();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatEvolutionAlgorithmParameters(NeatEvolutionAlgorithmParameters copyFrom)
        {
            _specieCount = copyFrom._specieCount;
            _elitismProportion = copyFrom._elitismProportion;
            _selectionProportion = copyFrom._selectionProportion;

            _offspringAsexualProportion = copyFrom._offspringAsexualProportion;
            _offspringSexualProportion = copyFrom._offspringSexualProportion;
            _interspeciesMatingProportion = copyFrom._interspeciesMatingProportion;

            _bestFitnessMovingAverageHistoryLength = copyFrom._bestFitnessMovingAverageHistoryLength;
            _meanSpecieChampFitnessMovingAverageHistoryLength = copyFrom._meanSpecieChampFitnessMovingAverageHistoryLength;
            _complexityMovingAverageHistoryLength = copyFrom._complexityMovingAverageHistoryLength;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the specie count.
        /// </summary>
        public int SpecieCount
        {
            get { return _specieCount; }
            set { _specieCount = value; }
        }

        /// <summary>
        /// Gets or sets the elitism proportion. 
        /// We sort specie genomes by fitness and keep the top N%, the other genomes are
        /// removed to make way for the offspring.
        /// </summary>
        public double ElitismProportion
        {
            get { return _elitismProportion; }
            set { _elitismProportion = value; }
        }

        /// <summary>
        /// Gets or sets the selection proportion.
        /// We sort specie genomes by fitness and select parent genomes for producing offspring from 
        /// the top N%. Selection is performed prior to elitism being applied, therefore selecting from more
        /// genomes than will be made elite is possible.
        /// </summary>
        public double SelectionProportion
        {
            get { return _selectionProportion; }
            set { _selectionProportion = value; }
        }
    
        /// <summary>
        /// Gets or sets the proportion of offspring to be produced from asexual reproduction (mutation).
        /// </summary>
        public double OffspringAsexualProportion
        {
            get { return _offspringAsexualProportion; }
            set { _offspringAsexualProportion = value; } 
        }

        /// <summary>
        /// Gets or sets the proportion of offspring to be produced from sexual reproduction.
        /// </summary>
        public double OffspringSexualProportion
        {
            get { return _offspringSexualProportion; }
            set { _offspringSexualProportion = value; } 
        }

        /// <summary>
        /// Gets or sets the proportion of sexual reproductions that will use genomes from different species.
        /// </summary>
        public double InterspeciesMatingProportion
        {
            get { return _interspeciesMatingProportion; }
            set { _interspeciesMatingProportion = value; } 
        }

        /// <summary>
        /// Gets or sets the history buffer length used for calculating the best fitness moving average.
        /// </summary>
        public int BestFitnessMovingAverageHistoryLength
        {
            get { return _bestFitnessMovingAverageHistoryLength; }
            set { _bestFitnessMovingAverageHistoryLength = value; } 
        }

        /// <summary>
        /// Gets or sets the history buffer length used for calculating the mean specie champ fitness
        /// moving average.
        /// </summary>
        public int MeanSpecieChampFitnessMovingAverageHistoryLength
        {
            get { return _meanSpecieChampFitnessMovingAverageHistoryLength; }
            set { _meanSpecieChampFitnessMovingAverageHistoryLength = value; } 
        }

        /// <summary>
        /// Gets or sets the history buffer length used for calculating the mean genome complexity moving 
        /// average.
        /// </summary>
        public int ComplexityMovingAverageHistoryLength
        {
            get { return _complexityMovingAverageHistoryLength; }
            set { _complexityMovingAverageHistoryLength = value; } 
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Normalize the sexual and asexual proportions such that their sum equals 1.
        /// </summary>
        private void NormalizeProportions()
        {
            double total = _offspringAsexualProportion + _offspringSexualProportion;
            _offspringAsexualProportion = _offspringAsexualProportion / total;
            _offspringSexualProportion = _offspringSexualProportion / total;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a set of parameters based on the current set and that are suitable for the simplifying 
        /// phase of the evolution algorithm when running with complexity regulation enabled.
        /// </summary>
        public NeatEvolutionAlgorithmParameters CreateSimplifyingParameters()
        {
            // Make a copy of the current 'complexifying' parameters (as required by complexity regulation)
            // and modify the copy to be suitable for simplifcation. Basically we disable sexual reproduction
            // whle in simplifying mode to prevent proliferation of structure through sexual reproduction.
            NeatEvolutionAlgorithmParameters eaParams = new NeatEvolutionAlgorithmParameters(this);
            eaParams._offspringAsexualProportion = 1.0;
            eaParams._offspringSexualProportion = 0.0;
            return eaParams;
        }

        #endregion
    }
}
