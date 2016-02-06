
namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change after a fixed number of generations.
    /// Suitable for transitioning in both directions.
    /// </summary>
    public class ElapsedGenerationsTrigger : IPhaseTransitionTrigger
    {
        readonly uint _generations;

        #region Constructor

        /// <summary>
        /// Constructs with the provided phase duration in generations.
        /// </summary>
        public ElapsedGenerationsTrigger(uint generations)
        {
            _generations = generations;
        }

        #endregion

        #region IPhaseTransitionTrigger

        /// <summary>
        /// Returns true of the trigger criteria are met.
        /// </summary>
        public bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats)
        {
            return (neatStats._generation - phasedSearchStats._lastTransitionGeneration) > _generations;
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
            get { return true; }
        }

        #endregion
    }
}
