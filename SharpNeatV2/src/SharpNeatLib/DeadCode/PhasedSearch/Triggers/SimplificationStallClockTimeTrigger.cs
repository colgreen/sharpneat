using System;

namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change when simplification has stalled for some period of clock time.
    /// Stalling refers to complexity not falling below the minimum observed so far in the current phase.
    /// </summary>
    public class SimplificationStallClockTimeTrigger : IPhaseTransitionTrigger
    {
        readonly TimeSpan _clockTimeStallLimit;

        #region Constructor

        /// <summary>
        /// Construct with the provided clock time limit. 
        /// </summary>
        /// <param name="clockTimeStallLimit">If complexity has not fallen within this amount of time then the 
        /// simplification phase is deemed to have stalled.</param>
        public SimplificationStallClockTimeTrigger(TimeSpan clockTimeStallLimit)
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
            return (DateTime.Now - phasedSearchStats._currentPhaseComplexityMinClockTime) > _clockTimeStallLimit;
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
