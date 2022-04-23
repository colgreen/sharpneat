// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;
using static SharpNeat.IO.JsonReadOptionalUtils;

namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// Static utility methods for reading <see cref="NeatEvolutionAlgorithmSettings"/> from json.
/// </summary>
public static class NeatEvolutionAlgorithmSettingsJsonReader
{
    /// <summary>
    /// Read json into a target instance of <see cref="NeatEvolutionAlgorithmSettings"/>.
    /// Settings that are present are read and set on the target settings object; all other settings
    /// remain unchanged on the target object.
    /// </summary>
    /// <param name="target">The target settings object to store the read values on.</param>
    /// <param name="jelem">The json element to read from.</param>
    public static void Read(
        NeatEvolutionAlgorithmSettings target,
        JsonElement jelem)
    {
        ReadIntOptional(jelem, "speciesCount", x => target.SpeciesCount = x);
        ReadDoubleOptional(jelem, "elitismProportion", x => target.ElitismProportion = x);
        ReadDoubleOptional(jelem, "selectionProportion", x => target.SelectionProportion = x);
        ReadDoubleOptional(jelem, "offspringAsexualProportion", x => target.OffspringAsexualProportion = x);
        ReadDoubleOptional(jelem, "offspringSexualProportion", x => target.OffspringSexualProportion = x);
        ReadDoubleOptional(jelem, "interspeciesMatingProportion", x => target.InterspeciesMatingProportion = x);
        ReadIntOptional(jelem, "statisticsMovingAverageHistoryLength", x => target.StatisticsMovingAverageHistoryLength = x);
        target.Validate();
    }
}
