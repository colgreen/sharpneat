namespace SharpNeat.Neat.Reproduction.Asexual;

public static class NeatAsexualReproductionSettingsExtensions
{
    /// <summary>
    /// Creates a new settings object based on the current settings object, but modified to be suitable for use when
    /// the evolution algorithm is in simplifying mode.
    /// </summary>
    /// <returns>A new instance of <see cref="NeatAsexualReproductionSettings"/>.</returns>
    public static NeatAsexualReproductionSettings CreateSimplifyingSettings(
        this NeatAsexualReproductionSettings settings)
    {
        var simplifying = settings with
        {
            ConnectionWeightMutationProbability = 0.6,
            AddNodeMutationProbability = 0.0,
            AddConnectionMutationProbability = 0.0,
            DeleteConnectionMutationProbability = 0.4
        };

        return simplifying;
    }

    /// <summary>
    /// Validate the settings, and throw an exception if not valid.
    /// </summary>
    /// <remarks>
    /// As a 'simple' collection of properties there is no construction time check that can be performed, therefore this method is supplied to
    /// allow consumers of a settings object to validate it before using it.
    /// </remarks>
    public static void Validate(
        this NeatAsexualReproductionSettings settings)
    {
        if(!IsProbability(settings.ConnectionWeightMutationProbability)) throw new InvalidOperationException("ConnectionWeightMutationProbability must be in the interval [0,1].");
        if(!IsProbability(settings.AddNodeMutationProbability)) throw new InvalidOperationException("AddNodeMutationProbability must be in the interval [0,1].");
        if(!IsProbability(settings.AddConnectionMutationProbability)) throw new InvalidOperationException("AddConnectionMutationProbability must be in the interval [0,1].");
        if(!IsProbability(settings.DeleteConnectionMutationProbability)) throw new InvalidOperationException("DeleteConnectionMutationProbability must be in the interval [0,1].");
        if(Math.Abs((settings.ConnectionWeightMutationProbability + settings.AddNodeMutationProbability + settings.AddConnectionMutationProbability + settings.DeleteConnectionMutationProbability) - 1.0) > 1e-6)
            throw new InvalidOperationException("Mutation probabilities must sum to 1.0");

        static bool IsProbability(double p) => p >= 0 && p <= 1.0;
    }
}
