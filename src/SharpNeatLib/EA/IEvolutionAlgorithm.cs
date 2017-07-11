
namespace SharpNeat.EA
{
    public interface IEvolutionAlgorithm
    {
        /// <summary>
        /// Gets evolutionary algorithm statistics.
        /// </summary>
        EAStatistics EAStats { get; }

        /// <summary>
        /// Gets or sets the complexity regulation mode.
        /// </summary>
        ComplexityRegulationMode ComplexityRegulationMode { get; set; }

        /// <summary>
        /// Perform one generation of the evolutionary algorithm.
        /// </summary>
        void PerformOneGeneration();
    }
}
