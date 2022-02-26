/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// NEAT specific evolution algorithm statistics.
/// </summary>
public class NeatEvolutionAlgorithmStatistics : EvolutionAlgorithmStatistics
{
    /// <summary>
    /// The total number of offspring genomes created since the evolution algorithm started.
    /// </summary>
    public ulong TotalOffspringCount { get; set; }

    /// <summary>
    /// The total number of offspring genomes created through asexual reproduction since the evolution algorithm started.
    /// </summary>
    public int TotalOffspringAsexualCount { get; set; }

    /// <summary>
    /// The total number of offspring genomes created through sexual reproduction since the evolution algorithm started.
    /// </summary>
    public int TotalOffspringSexualCount { get; set; }

    /// <summary>
    /// The total number of offspring genomes created through inter-species sexual reproduction since the evolution algorithm started.
    /// This number is included in <see cref="TotalOffspringSexualCount"/>.
    /// </summary>
    public int TotalOffspringInterspeciesCount { get; set; }
}
