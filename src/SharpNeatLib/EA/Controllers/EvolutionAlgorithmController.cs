using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace SharpNeat.EA.Controllers
{
    public class EvolutionAlgorithmController : IDisposable
    {
        static readonly ILog __log = LogManager.GetLogger(typeof(EvolutionAlgorithmController));

        #region Instance Fields

        readonly IEvolutionAlgorithm _ea;

        // Algorithm state data.
        RunState _runState = RunState.NotReady;

        // Update event scheme / data.
        UpdateScheme _updateScheme;
        uint _prevUpdateGeneration;
        long _prevUpdateTimeTick;

        // Misc working variables.
        Thread _algorithmThread;
        bool _pauseRequestFlag;
        bool _terminateFlag = false;
        readonly AutoResetEvent _awaitPauseEvent = new AutoResetEvent(false);
        readonly AutoResetEvent _awaitRestartEvent = new AutoResetEvent(false);

        // Current status.
        uint _currentGeneration;

        #endregion

        #region Events

        /// <summary>
        /// Notifies listeners that some state change has occurred.
        /// </summary>
        public event EventHandler UpdateEvent;
        /// <summary>
        /// Notifies listeners that the algorithm has paused.
        /// </summary>
        public event EventHandler PausedEvent;

        #endregion

        #region Constructor

        public EvolutionAlgorithmController(IEvolutionAlgorithm ea)
        {
            _ea = ea;
            _currentGeneration = 0;
            _runState = RunState.Ready;
            _updateScheme = new UpdateScheme(new TimeSpan(0, 0, 1));
        }

        #endregion

        #region Properties

        public IEvolutionAlgorithm EA => _ea; 

        /// <summary>
        /// Gets or sets the algorithm's update scheme.
        /// </summary>
        public UpdateScheme UpdateScheme 
        {
            get { return _updateScheme; }
            set { _updateScheme = value; }
        }

        /// <summary>
        /// Gets the current execution/run state of the IEvolutionAlgorithm.
        /// </summary>
        public RunState RunState
        {
            get { return _runState; }
        }

        /// <summary>
        /// Gets the current generation.
        /// </summary>
        public uint CurrentGeneration => _currentGeneration;
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the algorithm running. The algorithm will switch to the Running state from either
        /// the Ready or Paused states.
        /// </summary>
        public void StartContinue()
        {
            // RunState must be Ready or Paused.
            if(RunState.Ready == _runState)
            {   // Create a new thread and start it running.
                _algorithmThread = new Thread(AlgorithmThreadMethod);
                _algorithmThread.IsBackground = true;
                _algorithmThread.Priority = ThreadPriority.BelowNormal;
                _runState = RunState.Running;
                OnUpdateEvent();
                _algorithmThread.Start();
            }
            else if(RunState.Paused == _runState)
            {   // Thread is paused. Resume execution.
                _runState = RunState.Running;
                OnUpdateEvent();
                _awaitRestartEvent.Set();
            }
            else if(RunState.Running == _runState)
            {   // Already running. Log a warning.
                __log.Warn("StartContinue() called but algorithm is already running.");
            }
            else {
                throw new Exception($"StartContinue() call failed. Unexpected RunState [{_runState}]");
            }
        }

        /// <summary>
        /// Alias for RequestPause().
        /// </summary>
        public void Stop()
        {
            RequestPause();
        }

        /// <summary>
        /// Requests that the algorithm pauses but doesn't wait for the algorithm thread to stop.
        /// The algorithm thread will pause when it is next convenient to do so, and will notify
        /// listeners via an UpdateEvent.
        /// </summary>
        public void RequestPause()
        {
            if(RunState.Running == _runState) {
                _pauseRequestFlag = true;
            }
            else {
                __log.Warn("RequestPause() called but algorithm is not running.");
            }
        }

        /// <summary>
        /// Request that the algorithm pause and waits for the algorithm to do so. The algorithm
        /// thread will pause when it is next convenient to do so and notifies any UpdateEvent 
        /// listeners prior to returning control to the caller. Therefore it's generally a bad idea 
        /// to call this method from a GUI thread that also has code that may be called by the
        /// UpdateEvent - doing so will result in deadlocked threads.
        /// </summary>
        public void RequestPauseAndWait()
        {
            if(RunState.Running == _runState) 
            {   // Set a flag that tells the algorithm thread to enter the paused state and wait 
                // for a signal that tells us the thread has paused.
                _pauseRequestFlag = true;
                _awaitPauseEvent.WaitOne();
            }
            else 
            {
                __log.Warn("RequestPauseAndWait() called but algorithm is not running.");
            }
        }

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

        public void Dispose()
        {
            RequestTerminateAndWait();
        }

        #endregion

        #region Private Methods

        private void AlgorithmThreadMethod()
        {
            try
            {
                _prevUpdateGeneration = 0;
                _prevUpdateTimeTick = DateTime.Now.Ticks;

                for(;;)
                {
                    _currentGeneration++;
                    _ea.PerformOneGeneration();

                    if(UpdateTest())
                    {
                        _prevUpdateGeneration = _currentGeneration;
                        _prevUpdateTimeTick = DateTime.Now.Ticks;
                         OnUpdateEvent();
                    }
                
                    // Check if a pause has been requested. 
                    // Access to the flag is not thread synchronized, but it doesn't really matter if
                    // we miss it being set and perform one other generation before pausing.
                    if(_pauseRequestFlag || _ea.EAStats.StopConditionSatisfied)
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
            catch(ThreadAbortException)
            {   // Quietly exit thread.
            }
        }

        /// <summary>
        /// Returns true if it is time to raise an update event.
        /// </summary>
        private bool UpdateTest()
        {
            if(UpdateMode.Generational == _updateScheme.UpdateMode) {
                return (_currentGeneration - _prevUpdateGeneration) >= _updateScheme.Generations;
            }
            
            return (DateTime.Now.Ticks - _prevUpdateTimeTick) >= _updateScheme.TimeSpan.Ticks;
        }

        #endregion

        #region Private Methods [OnEvent*]

        private void OnUpdateEvent()
        {
            if(null != UpdateEvent)
            {
                // Catch exceptions thrown by event listeners. This prevents listener exceptions from terminating the algorithm thread.
                try {
                    UpdateEvent(this, EventArgs.Empty);
                }
                catch(Exception ex) {
                    __log.Error("UpdateEvent listener threw exception", ex);
                }
            }
        }

        private void OnPausedEvent()
        {
            if(null != PausedEvent)
            {
                // Catch exceptions thrown by even listeners. This prevents listener exceptions from terminating the algorithm thread.
                try {
                    PausedEvent(this, EventArgs.Empty);
                }
                catch(Exception ex) {
                    __log.Error("PausedEvent listener threw exception", ex);
                }
            }
        }

        #endregion
    }
}
