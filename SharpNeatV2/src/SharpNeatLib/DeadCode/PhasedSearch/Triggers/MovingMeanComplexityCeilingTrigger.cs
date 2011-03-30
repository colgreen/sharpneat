
namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change when a moving complexity ceiling is reached.
    /// The ceiling is relative to the baseline complexity at the end of the last simplification phase, thus
    /// we allow complexity to rise by a fixed amount during each complexification phase before transitioning
    /// to simplification.
    /// </summary>
    public class MovingMeanComplexityCeilingTrigger : IPhaseTransitionTrigger
    {
        readonly double _relativeCeiling;

        #region Constructor

        /// <summary>
        /// Constructs with the provided relative complexity ceiling value.
        /// </summary>
        public MovingMeanComplexityCeilingTrigger(double relativeCeiling)
        {
            _relativeCeiling = relativeCeiling;
        }

        #endregion

        #region IPhaseTransitionTrigger

        /// <summary>
        /// Returns true of the trigger criteria are met.
        /// </summary>
        public bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats)
        {
            return neatStats._meanComplexity > (phasedSearchStats._lastComplexityBase + _relativeCeiling);
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
