/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Neat.ComplexityRegulation;

/// <summary>
/// Complexity regulation modes.
///
/// Represents two variations in the overall search strategy - complexifying and simplifying.
/// That is, allowing genomes to complexify, and reducing their complexity to trim away excess
/// and/or redundant structure in the population to reinvigorate a search.
///
/// For more information see:
/// Phased Searching with NEAT: Alternating Between Complexification And Simplification, Colin Green, 2004
/// (http://sharpneat.sourceforge.net/phasedsearch.html).
/// </summary>
public enum ComplexityRegulationMode
{
    /// <summary>
    /// Search by allowing genomes to complexify (add new structure).
    /// </summary>
    Complexifying = 0,
    /// <summary>
    /// Search by simplifying genomes (removing structure).
    /// </summary>
    Simplifying = 1
}
