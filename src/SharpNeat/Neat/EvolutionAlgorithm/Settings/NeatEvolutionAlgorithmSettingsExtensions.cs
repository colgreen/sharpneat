namespace SharpNeat.Neat.EvolutionAlgorithm.Settings;

public static class NeatEvolutionAlgorithmSettingsExtensions
{
    /// <summary>
    /// Creates a new settings object based on the current settings object, but modified to be suitable for use when
    /// the evolution algorithm is in simplifying mode.
    /// </summary>
    /// <returns>A new instance of <see cref="NeatEvolutionAlgorithmSettings"/>.</returns>
    public static NeatEvolutionAlgorithmSettings CreateSimplifyingSettings(this NeatEvolutionAlgorithmSettings config)
    {
        NeatEvolutionAlgorithmSettings simplifying = config with
        {
            OffspringAsexualProportion = 1.0,
            OffspringRecombinationProportion = 0.0
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
    public static void Validate(this NeatEvolutionAlgorithmSettings config)
    {
        if (config.SpeciesCount < 1) throw new InvalidOperationException("SpeciesCount must be >= 1.");
        if (!IsProportion(config.ElitismProportion)) throw new InvalidOperationException("ElitismProportion must be in the interval [0,1].");
        if (!IsProportion(config.SelectionProportion)) throw new InvalidOperationException("SelectionProportion must be in the interval [0,1].");
        if (!IsProportion(config.OffspringAsexualProportion)) throw new InvalidOperationException("OffspringAsexualProportion must be in the interval [0,1].");
        if (!IsProportion(config.OffspringRecombinationProportion)) throw new InvalidOperationException("OffspringRecombinationProportion must be in the interval [0,1].");
        if (!IsProportion(config.InterspeciesMatingProportion)) throw new InvalidOperationException("InterspeciesMatingProportion must be in the interval [0,1].");
        if (config.StatisticsMovingAverageHistoryLength < 1) throw new InvalidOperationException("StatisticsMovingAverageHistoryLength must be >= 1.");
        if (Math.Abs(config.OffspringAsexualProportion + config.OffspringRecombinationProportion - 1.0) > 1e-6) throw new InvalidOperationException("OffspringAsexualProportion and OffspringRecombinationProportion must sum to 1.0");

        static bool IsProportion(double p) => p >= 0 && p <= 1.0;
    }
}
