using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Tasks.Gymnasium;
using SharpNeat.Windows;

namespace SharpNeat.Tasks.Windows.Gymnasium;

public class GymnasiumControl : GenomeControl
{
    readonly IGenomeDecoder<NeatGenome<double>,IBlackBox<double>> _genomeDecoder;

    // The agent used by the simulation thread.
    volatile IBlackBox<double> _agent;
    readonly bool _initializing = true;

    // Thread for running simulation.
    readonly Thread _simThread;

    // Indicates whether a simulation is running. Access is thread synchronised using Interlocked.
    volatile bool _terminateSimThread;

    // Event that signals simulation thread to start a simulation.
    readonly AutoResetEvent _simStartEvent = new(false);
    readonly ManualResetEvent _simNotRunningEvent = new(false);

    /// <summary>
    /// Constructs a new instance of <see cref="GymnasiumControl"/>.
    /// </summary>
    /// <param name="genomeDecoder">Genome decoder.</param>
    public GymnasiumControl(
        IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> genomeDecoder)
    {
        _genomeDecoder = genomeDecoder ?? throw new ArgumentNullException(nameof(genomeDecoder));

        try
        {
            InitializeComponent();

            // Create background thread for running simulation alongside NEAT algorithm.
            _simThread = new(new ThreadStart(SimulationThread))
            {
                IsBackground = true
            };
            _simThread.Start();
        }
        finally
        {
            _initializing = false;
        }
    }

    public override void OnGenomeUpdated()
    {
        base.OnGenomeUpdated();

        // Get a local reference to avoid possible race conditions on the class field.
        IGenome genome = _genome;

        if(genome is null || _terminateSimThread || _initializing)
            return;

        // Dispose any existing agent.
        var existingAgent = _agent;
        Thread.MemoryBarrier();
        existingAgent?.Dispose();

        // Decode the genome, and store the resulting IBlackBox agent in an instance field.
        NeatGenome<double> neatGenome = genome as NeatGenome<double>;
        _agent = _genomeDecoder.Decode(neatGenome);

        // Signal simulation thread to start running a one simulation.
        _simStartEvent.Set();
    }

    #region Private Methods [Windows.Forms Designer Code]

    private void InitializeComponent()
    {
    }

    #endregion

    /// <summary>
    /// Simulate prey capture until thread is terminated.
    /// </summary>
    private void SimulationThread()
    {
        // Wait to be signalled to start the next trial run.
        _simStartEvent.WaitOne();

        IBlackBox<double> agent = _agent;

        // Clear any prior agent state.
        agent.Reset();

        while (true)
        {
            // Check if we have been signalled to terminate before starting a simulation run.
            if(_terminateSimThread)
                break;

            var episode = new GymnasiumEpisode(24, 4, true, true);
            episode.Evaluate(agent);

            // Test if the thread should be terminated.
            if (_terminateSimThread)
                break;
        }

        // Signal any thread waiting for this simulation thread to terminate.
        _simNotRunningEvent.Set();
    }

    protected override void Dispose(bool disposing)
    {
        if( disposing )
        {
            // Signal the simulation thread to terminate, and wait for it to terminate.
            _terminateSimThread = true;
            _simStartEvent.Set();
            _simNotRunningEvent.WaitOne();

            base.Dispose(disposing);

            _agent.Dispose();
            _simStartEvent.Dispose();
            _simNotRunningEvent.Dispose();
        }
    }
}
