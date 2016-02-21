
namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change when simplification has stalled for some number of generations.
    /// Stalling refers to complexity not falling below the minimum observed so far in the current phase.
    /// </summary>
    public class SimplificationStallGenerationsTrigger : IPhaseTransitionTrigger
    {
        readonly uint _generationStallLimit;

        #region Constructor

        /// <summary>
        /// Construct with the provided generations limit. 
        /// </summary>
        /// <param name="generationStallLimit">If complexity has not fallen within this number of generations then the 
        /// simplification phase is deemed to have stalled.</param>
        public SimplificationStallGenerationsTrigger(uint generationStallLimit)
        {
            _generationStallLimit = generationStallLimit;
        }

        #endregion

        #region IPhaseTransitionTrigger

        /// <summary>
        /// Returns true of the trigger criteria are met.
        /// </summary>
        public bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats)
        {
            return (phasedSearchStats._currentPhaseComplexityMinGeneration - neatStats._generation) > _generationStallLimit;
        }

        /// <summary>
        /// Indicates if the trigger is appropriate for transition from complexification to simplification.
        /// </summary>
        public bool ComplexificationToSimplification
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates if the trigger is appropriate for transition from simplification to complexification.
        /// </summary>
        public bool SimplificationToComplexification
        {
            get { return true; }
        }

        #endregion
    }
}
