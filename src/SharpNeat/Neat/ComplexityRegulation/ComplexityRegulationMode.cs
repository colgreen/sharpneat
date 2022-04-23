// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
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
