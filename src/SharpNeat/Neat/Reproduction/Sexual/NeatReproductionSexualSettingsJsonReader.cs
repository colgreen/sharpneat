// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;
using static SharpNeat.IO.JsonReadOptionalUtils;

namespace SharpNeat.Neat.Reproduction.Sexual;

/// <summary>
/// Static utility methods for reading <see cref="NeatReproductionSexualSettings"/> from json.
/// </summary>
public static class NeatReproductionSexualSettingsJsonReader
{
    /// <summary>
    /// Read json into a target instance of <see cref="NeatReproductionSexualSettings"/>.
    /// Settings that are present are read and set on the target settings object; all other settings
    /// remain unchanged on the target object.
    /// </summary>
    /// <param name="target">The target settings object to store the read values on.</param>
    /// <param name="jelem">The json element to read from.</param>
    public static void Read(
        NeatReproductionSexualSettings target,
        JsonElement jelem)
    {
        ReadDoubleOptional(jelem, "secondaryParentGeneProbability", x => target.SecondaryParentGeneProbability = x);
        target.Validate();
    }
}
