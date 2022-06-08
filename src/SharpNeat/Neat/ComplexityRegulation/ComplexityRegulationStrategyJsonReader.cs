// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;
using static SharpNeat.IO.JsonReadMandatoryUtils;

namespace SharpNeat.Neat.ComplexityRegulation;

/// <summary>
/// Static utility methods for creating instances of <see cref="IComplexityRegulationStrategy"/> from json configuration.
/// </summary>
public static class ComplexityRegulationStrategyJsonReader
{
    #region Public Static Methods

    /// <summary>
    /// Create a new instance of <see cref="IComplexityRegulationStrategy"/> based on the provided json configuration.
    /// </summary>
    /// <param name="jelem">The json element to read from.</param>
    /// <returns>A new instance of <see cref="IComplexityRegulationStrategy"/>.</returns>
    public static IComplexityRegulationStrategy Read(JsonElement jelem)
    {
        string? strategyName = jelem.GetProperty("strategyName").GetString();
        return strategyName switch
        {
            // The 'null' strategy has been explicitly defined.
            "null" => new NullComplexityRegulationStrategy(),
            "absolute" => ReadAbsoluteComplexityRegulationStrategy(jelem),
            "relative" => ReadRelativeComplexityRegulationStrategy(jelem),
            null => throw new ConfigurationException($"strategyName json property is empty."),
            _ => throw new ConfigurationException($"Unsupported complexity regulation strategyName [{strategyName}]"),
        };
    }

    #endregion

    #region Private Static Methods

    private static AbsoluteComplexityRegulationStrategy ReadAbsoluteComplexityRegulationStrategy(JsonElement jelem)
    {
        int complexityCeiling = ReadIntMandatory(jelem, "complexityCeiling");
        int minSimplifcationGenerations = ReadIntMandatory(jelem, "minSimplifcationGenerations");
        return new AbsoluteComplexityRegulationStrategy(minSimplifcationGenerations, complexityCeiling);
    }

    private static RelativeComplexityRegulationStrategy ReadRelativeComplexityRegulationStrategy(JsonElement jelem)
    {
        int relativeComplexityCeiling = ReadIntMandatory(jelem, "relativeComplexityCeiling");
        int minSimplifcationGenerations = ReadIntMandatory(jelem, "minSimplifcationGenerations");
        return new RelativeComplexityRegulationStrategy(minSimplifcationGenerations, relativeComplexityCeiling);
    }

    #endregion
}
