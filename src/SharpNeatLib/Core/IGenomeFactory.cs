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
using SharpNeat.Utility;

namespace SharpNeat.Core
{
    /// <summary>
    /// Generic interface genome factory classes. Genome factories are intended to be used for creating initial
    /// populations, either random populations or from one or more seed genomes. However, genome reproduction
    /// is performed via the overloaded IGenome.CreateOffspring() methods, placing these methods on
    /// IGenome gives reproduction methods direct and convenient access to internal genome data.
    /// 
    /// Concrete IGenomeFactory classes are expected to contain all parameters used for creating and breeding
    /// genomes such as (in NEAT) the number of input/output neurons, connection weight range and mutation 
    /// rate. Concrete IGenomes are therefore expected to be given a reference to their concrete factory class
    /// upon construction, this gives them access to parameters for use in IGenome.CreateOffspring() - which may 
    /// change during evolution, e.g. in the case of phased searching in NEAT.
    /// </summary>
    /// <typeparam name="TGenome">The genome type to act as a factory for.</typeparam>
    public interface IGenomeFactory<TGenome>
    {
        /// <summary>
        /// Gets the genome ID generator for the factory. This is used internally when creating new
        /// genomes and is exposed as a public property so that genomes that are spawning offspring can 
        /// generate IDs in the same ID space.
        /// </summary>
        UInt32IdGenerator GenomeIdGenerator { get; }

        /// <summary>
        /// Gets or sets a mode value. This is intended as a means for an evolution algorithm to convey changes
        /// in search mode to genomes, and because the set of modes is specific to each concrete implementation
        /// of an IEvolutionAlgorithm the mode is defined as an integer (rather than an enum[eration]).
        /// E.g. SharpNEAT's implementation of NEAT uses an evolutionary algorithm that alternates between
        /// a complexifying and simplifying mode, in order to do this the algorithm class needs to notify the genomes
        /// of the current mode so that the CreateOffspring() methods are able to generate offspring appropriately - 
        /// e.g. we avoid adding new nodes and connections and increase the rate of deletion mutations when in
        /// simplifying mode.
        /// </summary>
        int SearchMode { get; set; }

        /// <summary>
        /// Creates a list of randomly initialised genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genomes as their birth generation.</param>
        List<TGenome> CreateGenomeList(int length, uint birthGeneration);

        /// <summary>
        /// Creates a list of genomes spawned from a seed genome. Spawning uses asexual reproduction.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation.
        /// Assigned to the new genomes as their birth generation.</param>
        /// <param name="seedGenome">The seed genome.</param>
        List<TGenome> CreateGenomeList(int length, uint birthGeneration, TGenome seedGenome);

        /// <summary>
        /// Creates a list of genomes spawned from a list of seed genomes. Spawning uses asexual reproduction and
        /// typically we repeatedly loop over (and spawn from) the seed genomes until we have the required number
        /// of spawned genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genomes as their birth generation.</param>
        /// <param name="seedGenomeList">A list of seed genomes.</param>
        List<TGenome> CreateGenomeList(int length, uint birthGeneration, List<TGenome> seedGenomeList);

        /// <summary>
        /// Creates a single randomly initialised genome.
        /// </summary>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genome as its birth generation.</param>
        TGenome CreateGenome(uint birthGeneration);

        /// <summary>
        /// Supports debug/integrity checks. Checks that a given genome object's type is consistent with the genome factory. 
        /// Typically the wrong type of object may occur where factoriess are subtyped and not all of the relevant virtual
		/// methods are overriden. Returns true if type is correct.
        /// </summary>
        bool CheckGenomeType(TGenome genome);
    }
}
