
namespace SharpNeat.EvolutionAlgorithm.Runner
{
    /// <summary>
    /// An enumeration of update schemes, e.g. Fire an update event the per some time duration or some number of generations.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Raise an update event at regular time intervals.
        /// </summary>
        Timespan,
        /// <summary>
        /// Raise an update event at regular generation intervals. (Every N generations).
        /// </summary>
        Generational
    }
}
