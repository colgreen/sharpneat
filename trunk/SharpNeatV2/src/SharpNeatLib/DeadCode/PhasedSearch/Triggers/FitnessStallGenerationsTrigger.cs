
namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change when fitness has not improved for some number of generations.
    /// </summary>
    public class FitnessStallGenerationsTrigger : IPhaseTransitionTrigger
    {
        readonly uint _generationStallLimit;

        #region Constructor

        /// <summary>
        /// Construct with the provided generations limit. 
        /// </summary>
        /// <param name="generationStallLimit">If fitness has not risen within this number of generations then the 
        /// fitness is deemed to have stalled.</param>
        public FitnessStallGenerationsTrigger(uint generationStallLimit)
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
            return (neatStats._generation - phasedSearchStats._currentPhaseFitnessMaxGeneration) > _generationStallLimit;
        }

        /// <summary>
        /// Indicates if the trigger is appropriate for transition from complexification to simplification.
        /// </summary>
        public bool ComplexificationToSimplification
        {
            get { return true; }
        }

        /// <summary>
        /// Indicates if the trigger is appropriate for transition from simplification to complexification.
        /// </summary>
        public bool SimplificationToComplexification
        {
            get { return false; }
        }

        #endregion
    }
}
