/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
namespace SharpNeat.Core
{
    /// <summary>
    /// Generic interface for genome classes.
    /// 
    /// Concrete IGenome classes are expected to be given a reference to their concrete IGenomeFactory class even 
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
        /// Gets or sets a specie index. An implementation of this is required only when using 
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
        /// an abstract coordinate data type rather than being coded against specific IGenome types.
        /// </summary>
        CoordinateVector Position { get; }

        /// <summary>
        /// Gets or sets a cached phenome obtained from decoding the genome.
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
