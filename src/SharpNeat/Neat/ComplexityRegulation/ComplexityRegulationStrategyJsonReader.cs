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
            null => throw new Exception($"strategyName json property is empty."),
            _ => throw new Exception($"Unsupported complexity regulation strategyName [{strategyName}]"),
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
