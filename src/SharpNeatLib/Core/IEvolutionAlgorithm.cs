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
using System;
using System.Collections.Generic;

namespace SharpNeat.Core
{
    /// <summary>
    /// A generic interface for evolution algorithm classes.
    /// </summary>
    public interface IEvolutionAlgorithm<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        /// <summary>
        /// Notifies listeners that some state change has occured.
        /// </summary>
        event EventHandler UpdateEvent;

        /// <summary>
        /// Gets or sets the algorithm's update scheme.
        /// </summary>
        UpdateScheme UpdateScheme { get; set; }

        /// <summary>
        /// Gets the current execution/run state of the IEvolutionAlgorithm.
        /// </summary>
        RunState RunState { get; }

        /// <summary>
        /// Gets the population's current champion genome.
        /// </summary>
        TGenome CurrentChampGenome { get; }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that the algorithm has therefore stopped.
        /// </summary>
        bool StopConditionSatisfied { get; }

        /// <summary>
        /// Initializes the evolution algorithm with the provided IGenomeListEvaluator, IGenomeFactory
        /// and an initial population of genomes.
        /// </summary>
        void Initialize(IGenomeListEvaluator<TGenome> genomeListEvaluator,
                        IGenomeFactory<TGenome> genomeFactory,
                        List<TGenome> genomeList);

        /// <summary>
        /// Initializes the evolution algorithm with the provided IGenomeListEvaluator
        /// and an IGenomeFactory that can be used to create an initial population of genomes.
        /// </summary>
        void Initialize(IGenomeListEvaluator<TGenome> genomeListEvaluator,
                        IGenomeFactory<TGenome> genomeFactory,
                        int populationSize);

        /// <summary>
        /// Starts the algorithm running. The algorithm will switch to the Running state from either
        /// the Ready or Paused states.
        /// </summary>
        void StartContinue();

        /// <summary>
        /// Requests that the algorithm pauses but doesn't wait for the algorithm thread to stop.
        /// The algorithm thread will pause when it is next convenient to do so, and notifies
        /// listeners via an UpdateEvent.
        /// </summary>
        void RequestPause();

        /// <summary>
        /// Request that the algorithm pause and waits for the algorithm to do so. The algorithm
        /// thread will pause when it is next convenient to do so and notifies any UpdateEvent 
        /// listeners prior to returning control to the caller. Therefore it's generally a bad idea 
        /// to call this method from a GUI thread that also has code that may be called by the
        /// UpdateEvent - doing so will result in deadlocked threads.
        /// </summary>
        void RequestPauseAndWait();
    }
}
