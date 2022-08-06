// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.EvolutionAlgorithm.Runner;

/// <summary>
/// Evolution algorithm update scheme.
/// </summary>
public sealed class UpdateScheme
{
    private UpdateScheme(
        UpdateMode updateMode,
        uint generations,
        TimeSpan timespan)
    {
        this.UpdateMode = updateMode;
        this.Generations = generations;
        this.TimeSpan = timespan;
    }

    /// <summary>
    /// Gets the update scheme's mode.
    /// </summary>
    public UpdateMode UpdateMode { get; }

    /// <summary>
    /// Gets the number of generations between updates Applies to the generational update scheme only.
    /// </summary>
    public uint Generations { get; }

    /// <summary>
    /// Gets the timespan between updates. Applies to the timespan update scheme only.
    /// </summary>
    public TimeSpan TimeSpan { get; }

    /// <summary>
    /// Create a 'no updates' update scheme.
    /// </summary>
    /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
    public static UpdateScheme CreateNoUpdateScheme()
    {
        return new UpdateScheme(UpdateMode.None, 0, default);
    }

    /// <summary>
    /// Create a generation based update scheme. I.e. the update event will trigger every N generations.
    /// </summary>
    /// <param name="generations">The number of generations between update events.</param>
    /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
    public static UpdateScheme CreateGenerationalUpdateScheme(uint generations)
    {
        return new UpdateScheme(UpdateMode.Generational, generations, default);
    }

    /// <summary>
    /// Create a clock time based update scheme. I.e. the update event will trigger periodically based on the specified clock time duration/timespan.
    /// </summary>
    /// <param name="timespan">The duration between update events.</param>
    /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
    public static UpdateScheme CreateTimeSpanUpdateScheme(TimeSpan timespan)
    {
        return new UpdateScheme(UpdateMode.Timespan, 0, timespan);
    }
}
