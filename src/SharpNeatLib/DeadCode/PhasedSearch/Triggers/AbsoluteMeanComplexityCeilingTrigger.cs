
namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change when a hard/absolute complexity ceiling is reached.
    /// Use this if you want to constrain a search within a fixed range of complexity.
    /// </summary>
    public class AbsoluteMeanComplexityCeilingTrigger : IPhaseTransitionTrigger
    {
        readonly double _ceiling;

        #region Constructor

        /// <summary>
        /// Constructs with the provided absolute complexity ceiling value.
        /// </summary>
        public AbsoluteMeanComplexityCeilingTrigger(double ceiling)
        {
            _ceiling = ceiling;
        }

        #endregion

        #region IPhaseTransitionTrigger

        /// <summary>
        /// Returns true of the trigger criteria are met.
        /// </summary>
        public bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats)
        {
            return neatStats._meanComplexity > _ceiling;
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
