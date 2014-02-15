using System;

namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change when fitness has not improved for some period of elapsed clock time.
    /// </summary>
    public class FitnessStallClockTimeTrigger : IPhaseTransitionTrigger
    {
        readonly TimeSpan _clockTimeStallLimit;

        #region Constructor

        /// <summary>
        /// Construct with the provided clock time limit. 
        /// </summary>
        /// <param name="clockTimeStallLimit">If fitness has not risen within this amount of time then the 
        /// fitness is deemed to have stalled.</param>
        public FitnessStallClockTimeTrigger(TimeSpan clockTimeStallLimit)
        {
            _clockTimeStallLimit = clockTimeStallLimit;
        }

        #endregion

        #region IPhaseTransitionTrigger

        /// <summary>
        /// Returns true of the trigger criteria are met.
        /// </summary>
        public bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats)
        {
            return (DateTime.Now - phasedSearchStats._currentPhaseFitnessMaxClockTime) > _clockTimeStallLimit;
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
