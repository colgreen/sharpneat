/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Threading;
using log4net;

namespace SharpNeat.EvolutionAlgorithm.Runner
{
    /// <summary>
    /// Wraps a background worker thread for running an <see cref="IEvolutionAlgorithm"/>, and provides methods for asynchronous
    /// control of the worker thread, for starting, stopping, pausing and resuming the evolution algorithm.
    /// </summary>
    public sealed class EvolutionAlgorithmRunner : IDisposable
    {
        static readonly ILog __log = LogManager.GetLogger(typeof(EvolutionAlgorithmRunner));

        #region Instance Fields

        readonly IEvolutionAlgorithm _ea;

        // Update event scheme.
        readonly UpdateScheme _updateScheme;
        readonly Func<bool> _isUpdateDueFn;

        // Event synchronisation.
        readonly AutoResetEvent _awaitPauseEvent = new(false);
        readonly AutoResetEvent _awaitRestartEvent = new(false);

        // Runner state.
        volatile RunState _runState = RunState.Ready;
        int _prevUpdateGeneration;
        long _prevUpdateTimeTick;

        // Misc working variables.
        Thread? _algorithmThread;
        volatile bool _pauseRequestFlag;
        volatile bool _terminateFlag = false;

        #endregion

        #region Events

        /// <summary>
        /// Notifies listeners of an update event.
        /// </summary>
        public event EventHandler? UpdateEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="ea">The <see cref="IEvolutionAlgorithm"/> to wrap.</param>
        /// <param name="updateScheme">Evolution algorithm update event scheme.</param>
        public EvolutionAlgorithmRunner(
            IEvolutionAlgorithm ea,
            UpdateScheme updateScheme)
        {
            _ea = ea ?? throw new ArgumentNullException(nameof(ea));

            // Init update scheme.
            _updateScheme = updateScheme ?? throw new ArgumentNullException(nameof(updateScheme));
            _isUpdateDueFn = CreateIsUpdateDueFunction(_updateScheme);
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
        public UpdateScheme UpdateScheme => _updateScheme;

        /// <summary>
        /// Gets the current run state of the runner.
        /// </summary>
        public RunState RunState => _runState;

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the algorithm running, or resumes a paused algorithm.
        /// </summary>
        /// <remarks>
        /// This method can be called when the runner is in either the Ready or Paused states.
        /// If the runner is in the Paused state, then the runner is resumed.
        /// If the runner is already in the Running state, then the method does nothing and returns (and logs a warning).
        /// For all other states an exception is thrown.
        /// </remarks>
        public void StartOrResume()
        {
            switch(_runState)
            {
                case RunState.Ready:
                {
                    // Create a background thread for running the algorithm.
                    _algorithmThread = new Thread(BackgroundThreadMethod)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.BelowNormal
                    };

                    // Update RunState, and start the thread running.
                    _runState = RunState.Running;
                    _algorithmThread.Start();
                    break;
                }
                case RunState.Paused:
                {
                    // The runner is paused; resume execution.
                    _runState = RunState.Running;
                    _awaitRestartEvent.Set();
                    break;
                }
                case RunState.Running:
                {
                    // Already running. Log a warning.
                    __log.Warn("StartContinue() called but algorithm is already running.");
                    break;
                }
                default:
                    throw new Exception($"StartContinue() call failed. Unexpected RunState [{_runState}]");
            }
        }

        /// <summary>
        /// Requests that the runner pauses, but doesn't wait it to do so.
        /// </summary>
        /// <remarks>
        /// The runner will pause once the worker thread completes execution of the current generation.
        /// However, this method returns immediately and does not wait for the worker thread to stop.
        /// </remarks>
        public void RequestPause()
        {
            if(_runState == RunState.Running)
            {   // Set a flag that tells the worker thread to enter the paused state,
                // but do not wait for the background thread to pause.
                _pauseRequestFlag = true;
            }
            else
            {
                __log.Warn("RequestPause() called, but the algorithm is not running.");
            }
        }

        /// <summary>
        /// Request that the runner pauses, and waits it to do so.
        /// </summary>
        /// <remarks>
        /// The runner will pause once the worker thread completes execution of the current generation.
        /// This method will block and wait for the worker thread to stop before returning.
        /// </remarks>
        public void RequestPauseAndWait()
        {
            if(_runState == RunState.Running)
            {
                // Set a flag that tells the worker thread to enter the paused state,
                // and wait for a signal that tells us the worker thread has paused.
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
        /// The runner will stop once the worker thread completes execution of the current generation.
        /// This method will block and wait for the worker thread to stop before returning.
        /// </remarks>
        public void RequestTerminateAndWait()
        {
            switch(_runState)
            {
                case RunState.Ready:
                {
                    _runState = RunState.Terminated;
                    break;
                }

                case RunState.Running:
                {
                    // Signal the worker thread to terminate, and wait for it to do so.
                    _terminateFlag = true;
                    _pauseRequestFlag = true;
                    _awaitPauseEvent.WaitOne();
                    break;
                }
                case RunState.Paused:
                {
                    // Signal the worker thread to terminate, and resume it.
                    _terminateFlag = true;
                    _runState = RunState.Terminated;
                    _awaitRestartEvent.Set();
                    break;
                }

                default:
                    throw new Exception($"StartContinue() call failed. Unexpected RunState [{_runState}]");
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
            catch(Exception ex)
            {
                __log.Error($"BackgroundThreadMethod() failed with exception [{ex.Message}]", ex);
            }
        }

        private void BackgroundThreadMethodInner()
        {
            // Notify listeners that the algorithm is starting.
            OnUpdateEvent();

            _prevUpdateGeneration = 0;
            _prevUpdateTimeTick = DateTime.UtcNow.Ticks;

            for(;;)
            {
                _ea.PerformOneGeneration();

                if(_isUpdateDueFn())
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
                    // Signal to any waiting thread that we are pausing.
                    _awaitPauseEvent.Set();

                    // Test for terminate signal.
                    if(_terminateFlag)
                    {
                        _runState = RunState.Terminated;
                        OnUpdateEvent();
                        return;
                    }

                    // Reset the pause flag, update RunState, and notify any listeners of the state change.
                    _pauseRequestFlag = false;
                    _runState = RunState.Paused;
                    OnUpdateEvent();

                    // Wait indefinitely for a signal to wake up and continue.
                    _awaitRestartEvent.WaitOne();

                    // Test for terminate signal.
                    if(_terminateFlag)
                    {
                        OnUpdateEvent();
                        return;
                    }

                    // Notify any listeners of the state change.
                    OnUpdateEvent();
                }
            }
        }

        #endregion

        #region Private Methods [Events]

        private void OnUpdateEvent()
        {
            this.UpdateEvent?.Invoke(this,EventArgs.Empty);
        }

        #endregion

        #region Private Methods [IsUpdateDue Functions / Function Factory]

        private Func<bool> CreateIsUpdateDueFunction(UpdateScheme updateScheme)
        {
            return updateScheme.UpdateMode switch
            {
                UpdateMode.None => () => false,
                UpdateMode.Generational => IsUpdateDue_Generational,
                UpdateMode.Timespan => IsUpdateDue_TimeSpan,
                _ => throw new ArgumentException("Unexpected UpdateMode."),
            };
        }

        private bool IsUpdateDue_Generational()
        {
            return (_ea.Stats.Generation - _prevUpdateGeneration) >= _updateScheme.Generations;
        }

        private bool IsUpdateDue_TimeSpan()
        {
            return (DateTime.UtcNow.Ticks - _prevUpdateTimeTick) >= _updateScheme.TimeSpan.Ticks;
        }

        #endregion
    }
}
