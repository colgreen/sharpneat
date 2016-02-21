
namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// An interface that encapsulates logic for triggering phase transitions in phased searching.
    /// </summary>
    public interface IPhaseTransitionTrigger
    {
        /// <summary>
        /// Returns true if the trigger criteria are met.
        /// </summary>
        bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats);
        /// <summary>
        /// Indicates if the trigger is appropriate for transition from complexification to simplification.
        /// </summary>
        bool ComplexificationToSimplification { get; } 
        /// <summary>
        /// Indicates if the trigger is appropriate for transition from simplification to complexification.
        /// </summary>
        bool SimplificationToComplexification { get; } 
    }
}
