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
    /// Generic interface for evaluating a list of genomes. By operating on a list we allow concrete 
    /// implementations of this interface to choose between evaluating each genome independently of the others,
    /// perhaps across several execution threads, or in some collective evaluation scheme such as an artificial
    /// life/world scenario.
    /// </summary>
    public interface IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        ulong EvaluationCount { get; }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        bool StopConditionSatisfied { get; }

        /// <summary>
        /// Evaluates a list of genomes.
        /// </summary>
        void Evaluate(IList<TGenome> genomeList);  

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        void Reset();
    }
}
