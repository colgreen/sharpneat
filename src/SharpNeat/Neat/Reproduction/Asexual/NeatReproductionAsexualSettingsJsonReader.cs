// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;
using static SharpNeat.IO.JsonReadOptionalUtils;

namespace SharpNeat.Neat.Reproduction.Asexual;

/// <summary>
/// Static utility methods for reading <see cref="NeatReproductionAsexualSettings"/> from json.
/// </summary>
public static class NeatReproductionAsexualSettingsJsonReader
{
    /// <summary>
    /// Read json into a target instance of <see cref="NeatReproductionAsexualSettings"/>.
    /// Settings that are present are read and set on the target settings object; all other settings
    /// remain unchanged on the target object.
    /// </summary>
    /// <param name="target">The target settings object to store the read values on.</param>
    /// <param name="jelem">The json element to read from.</param>
    public static void Read(
        NeatReproductionAsexualSettings target,
        JsonElement jelem)
    {
        ReadDoubleOptional(jelem, "connectionWeightMutationProbability", x => target.ConnectionWeightMutationProbability = x);
        ReadDoubleOptional(jelem, "addNodeMutationProbability", x => target.AddNodeMutationProbability = x);
        ReadDoubleOptional(jelem, "addConnectionMutationProbability", x => target.AddConnectionMutationProbability = x);
        ReadDoubleOptional(jelem, "deleteConnectionMutationProbability", x => target.DeleteConnectionMutationProbability = x);
        target.Validate();
    }
}
