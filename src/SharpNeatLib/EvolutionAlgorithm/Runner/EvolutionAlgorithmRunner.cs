using System;
using System.Threading;
using log4net;

namespace SharpNeat.EvolutionAlgorithm.Runner
{
    /// <summary>
    /// Wraps a background thread for running an <see cref="IEvolutionAlgorithm"/>, and methods for asynchronous
    /// control of the background thread, i.e. for starting, stopping, pausing and restarting the evolution algorithm.
    /// </summary>
    public class EvolutionAlgorithmRunner : IDisposable
    {
        static readonly ILog __log = LogManager.GetLogger(typeof(EvolutionAlgorithmRunner));

        #region Instance Fields

        readonly IEvolutionAlgorithm _ea;

        // Runner state.
        volatile RunState _runState = RunState.NotReady;

        // Update event scheme / data.
        UpdateScheme _updateScheme;
        uint _prevUpdateGeneration;
        long _prevUpdateTimeTick;

        // Misc working variables.
        Thread _algorithmThread;
        volatile bool _pauseRequestFlag;
        volatile bool _terminateFlag = false;
        readonly AutoResetEvent _awaitPauseEvent = new AutoResetEvent(false);
        readonly AutoResetEvent _awaitRestartEvent = new AutoResetEvent(false);

        #endregion

        #region Events

        /// <summary>
        /// Notifies listeners of an update event.
        /// </summary>
        public event EventHandler UpdateEvent;

        /// <summary>
        /// Notifies listeners that the runner has paused.
        /// </summary>
        public event EventHandler PausedEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="ea">The <see cref="IEvolutionAlgorithm"/> to wrap.</param>
        public EvolutionAlgorithmRunner(IEvolutionAlgorithm ea)
        {
            _ea = ea ?? throw new ArgumentNullException(nameof(ea));

            // Set to ready state.
            _runState = RunState.Ready;

            // Set a default update scheme of once per second.
            _updateScheme = UpdateScheme.CreateTimeSpanUpdateScheme(TimeSpan.FromSeconds(1));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The underlying <see cref="IEvolutionAlgorithm"/>.
        /// </summary>
        public IEvolutionAlgorithm EA => _ea; 

        /// <summary>
        /// Gets or sets the runner's update scheme.
        /// </summary>
        public UpdateScheme UpdateScheme 
        {
            get { return _updateScheme; }
            set { _updateScheme = value; }
        }

        /// <summary>
        /// Gets the current run state of the runner.
        /// </summary>
        public RunState RunState
        {
            get { return _runState; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the algorithm running. 
        /// </summary>
        /// <remarks>
        /// This method can be called when the runner is in either the Ready or Paused states.
        /// If the runner is already in the Running state then the method does nothing and returns (and logs a warning).
        /// For all other states an exception is thrown.
        /// </remarks>
        public void StartContinue()
        {
            switch(_runState)
            {
                case RunState.Ready:
                    // Create a new thread and start it running.
                    _algorithmThread = new Thread(BackgroundThreadMethod)
                    { 
                        IsBackground = true,
                        Priority = ThreadPriority.BelowNormal
                    };
                    _runState = RunState.Running;
                    OnUpdateEvent();
                    _algorithmThread.Start();
                    return;
                
                case RunState.Paused:
                    // The runner is paused. Resume execution.
                    _runState = RunState.Running;
                    OnUpdateEvent();
                    _awaitRestartEvent.Set();
                    return;

                case RunState.Running:
                    // Already running. Log a warning.
                    __log.Warn("StartContinue() called but algorithm is already running.");
                    return;

                default:
                    throw new Exception($"StartContinue() call failed. Unexpected RunState [{_runState}]");
            }
        }

        /// <summary>
        /// Requests that the runner pauses, but doesn't wait it to do so.
        /// </summary>
        /// <remarks>
        /// The runner will pause once the background thread completes execution of the current generation. 
        /// However, this method returns immediately and does not wait for the background thread to stop.
        /// </remarks>
        public void RequestPause()
        {
            if(RunState.Running == _runState) {
                _pauseRequestFlag = true;
            } else {
                __log.Warn("RequestPause() called, but the runner is not running.");
            }
        }

        /// <summary>
        /// Request that the runner pauses, and waits it to do so.
        /// </summary>
        /// <remarks>
        /// The runner will pause once the background thread completes execution of the current generation.
        /// This method will block and wait for the background thread to stop before returning.
        /// </remarks>
        public void RequestPauseAndWait()
        {
            if(RunState.Running == _runState) 
            {
                // Set a flag that tells the background thread to enter the paused state, 
                // and wait for a signal that tells us the thread has paused.
                _pauseRequestFlag = true;
                _awaitPauseEvent.WaitOne();
            }
            else 
            {
                __log.Warn("RequestPauseAndWait() called but algorithm is not running.");
            }
        }

        /// <summary>
        /// Request that the runner terminates, and waits it to do so.
        /// </summary>
        /// <remarks>
        /// The runner will stop once the background thread completes execution of the current generation.
        /// This method will block and wait for the background thread to stop before returning.
        /// </remarks>
        public void RequestTerminateAndWait()
        {
            if(RunState.Running == _runState) 
            {   
                // Signal worker thread to terminate.
                _terminateFlag = true;
                _pauseRequestFlag = true;
                _awaitPauseEvent.WaitOne();
            }
        }

        /// <summary>
        /// Releases both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RequestTerminateAndWait();
        }

        #endregion

        #region Private Methods [Background Thread]

        private void BackgroundThreadMethod()
        {
            try
            {
                BackgroundThreadMethodInner();
            }
            catch(ThreadAbortException)
            {   // Quietly exit thread.
            }
        }

        private void BackgroundThreadMethodInner()
        {
            _prevUpdateGeneration = 0;
            _prevUpdateTimeTick = DateTime.UtcNow.Ticks;

            for(;;)
            {
                _ea.PerformOneGeneration();

                if(IsUpdateDue())
                {
                    _prevUpdateGeneration = _ea.Stats.Generation;
                    _prevUpdateTimeTick = DateTime.UtcNow.Ticks;
                    OnUpdateEvent();
                }
                
                // Check if a pause has been requested. 
                // Note. Access to the flag is not thread synchronized, but it doesn't really matter if
                // we miss it being set and perform one other generation before pausing.
                if(_pauseRequestFlag || _ea.Stats.StopConditionSatisfied)
                {
                    // Signal to any waiting thread that we are pausing
                    _awaitPauseEvent.Set();

                    // Test for terminate signal.
                    if(_terminateFlag) {
                        return;
                    }

                    // Reset the flag. Update RunState and notify any listeners of the state change.
                    _pauseRequestFlag = false;
                    _runState = RunState.Paused;
                    OnUpdateEvent();
                    OnPausedEvent();

                    // Wait indefinitely for a signal to wake up and continue.
                    _awaitRestartEvent.WaitOne();
                }
            }
        }

        #endregion

        #region Private Methods [Events]

        /// <summary>
        /// Returns true if an update event is due.
        /// </summary>
        private bool IsUpdateDue()
        {
            return ((UpdateMode.Generational == _updateScheme.UpdateMode) && ((_ea.Stats.Generation - _prevUpdateGeneration) >= _updateScheme.Generations))
                || ((UpdateMode.Timespan == _updateScheme.UpdateMode) && ((DateTime.UtcNow.Ticks - _prevUpdateTimeTick) >= _updateScheme.TimeSpan.Ticks));
        }

        private void OnUpdateEvent()
        {
            this?.UpdateEvent(this, EventArgs.Empty);
        }

        private void OnPausedEvent()
        {
            this?.PausedEvent(this, EventArgs.Empty);
        }

        #endregion
    }
}
