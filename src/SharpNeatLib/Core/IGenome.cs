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
namespace SharpNeat.Core
{
    /// <summary>
    /// Generic interface for genome classes.
    /// 
    /// Concrete IGenome classes are expected to to be given a reference to their concrete IGenomeFactory class even 
    /// if they are spawned from another IGenome. This allows all genomes to use the same set of parameters
    /// for spawning - which may change during evolution, e.g. in the case of phased searching in NEAT.
    /// </summary>
    public interface IGenome<TGenome>
    {
        /// <summary>
        /// Gets the genome's unique ID. IDs are unique across all genomes created from a single 
        /// IGenomeFactory.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets or sets a specie index. An implemention of this is required only when using 
        /// evolution algorithms that speciate genomes.
        /// </summary>
        int SpecieIdx { get; set; }

        /// <summary>
        /// Gets the generation that this genome was born/created in. Used to track genome age.
        /// </summary>
        uint BirthGeneration { get; }

        /// <summary>
        /// Gets the evaluation information for the genome, including its fitness.
        /// </summary>
        EvaluationInfo EvaluationInfo { get; }

        /// <summary>
        /// Gets a value that indicates the magnitude of a genome's complexity.
        /// </summary>
        double Complexity { get; }

        /// <summary>
        /// Gets a coordinate that represents the genome's position in the search space (also known
        /// as the genetic encoding space). This allows speciation/clustering algorithms to operate on
        /// an abstract cordinate data type rather than being coded against specific IGenome types.
        /// </summary>
        CoordinateVector Position { get; }

        /// <summary>
        /// Gets or sets a cached phenome obtained from decodign the genome.
        /// Genomes are typically decoded to Phenomes for evaluation. This property allows decoders to 
        /// cache the phenome in order to avoid decoding on each re-evaluation; However, this is optional.
        /// The phenome in un-typed to prevent the class framework from becoming overly complex.
        /// </summary>
        object CachedPhenome { get; set; }

        /// <summary>
        /// Asexual reproduction.
        /// </summary>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genome at its birth generation.</param>
        TGenome CreateOffspring(uint birthGeneration);

        /// <summary>
        /// Sexual reproduction.
        /// </summary>
        /// <param name="parent">The other parent genome.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genome at its birth generation.</param>
        TGenome CreateOffspring(TGenome parent, uint birthGeneration);
    }
}
