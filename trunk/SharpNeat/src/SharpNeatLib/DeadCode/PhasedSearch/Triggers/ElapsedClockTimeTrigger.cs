using System;

namespace SharpNeat.EvolutionAlgorithms.PhasedSearch.Triggers
{
    /// <summary>
    /// Triggers a phase change after a fixed period of clock time has elapsed.
    /// Suitable for transitioning in both directions.
    /// </summary>
    public class ElapsedClockTimeTrigger : IPhaseTransitionTrigger
    {
        readonly TimeSpan _timespan;

        #region Constructor

        /// <summary>
        /// Constructs with the provided phase duration timespan.
        /// </summary>
        public ElapsedClockTimeTrigger(TimeSpan timespan)
        {
            _timespan = timespan;
        }

        #endregion

        #region IPhaseTransitionTrigger

        /// <summary>
        /// Returns true of the trigger criteria are met.
        /// </summary>
        public bool TestTrigger(NeatAlgorithmStats neatStats, PhasedSearchStats phasedSearchStats)
        {
            return (DateTime.Now - phasedSearchStats._lastTransitionClockTime) > _timespan;
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
