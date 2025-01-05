using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Tasks.PreyCapture;
using static System.Net.Mime.MediaTypeNames;

namespace SharpNeat.Tasks.Windows.PreyCapture;

// Note. The Visual Studio UI designer/editor doesn't work with generic types, hence all generic code has been 'lifted' into this generic subclass.
public class GenericPreyCaptureControl<TScalar> : PreyCaptureControl
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    // View painting consts & objects.
    const int GridTop = 2;
    const int GridLeft = 2;

    readonly Pen _penGrey = new(Color.LightGray, 1F);
    readonly Brush _brushBackground = new SolidBrush(Color.Lavender);
    readonly Brush _brushBackgroundSensor = new SolidBrush(Color.LightBlue);
    readonly Brush _brushBackgroundSensorCaptured = new SolidBrush(Color.Orange);
    readonly Brush _brushAgent = new SolidBrush(Color.Red);
    readonly Brush _brushPrey = new SolidBrush(Color.Green);

    readonly IGenomeDecoder<NeatGenome<TScalar>,IBlackBox<TScalar>> _genomeDecoder;
    readonly PreyCaptureWorld<TScalar> _world;

    // Thread for running simulation.
    readonly Thread _simThread;

    // Event that signals simulation thread to start a simulation.
    readonly AutoResetEvent _simStartEvent = new(false);
    readonly ManualResetEvent _simNotRunningEvent = new(false);

    // Indicates whether a simulation is running. Access is thread synchronised using Interlocked.
    volatile bool _terminateSimThread;

    // The agent used by the simulation thread.
    volatile IBlackBox<TScalar> _agent;

    public GenericPreyCaptureControl(
        IGenomeDecoder<NeatGenome<TScalar>, IBlackBox<TScalar>> genomeDecoder,
        PreyCaptureWorld<TScalar> world)
        : base()
    {
        _genomeDecoder = genomeDecoder ?? throw new ArgumentNullException(nameof(genomeDecoder));
        _world = world ?? throw new ArgumentNullException(nameof(world));

        try
        {
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

    /// <summary>
    /// Simulate prey capture until thread is terminated.
    /// </summary>
    private void SimulationThread()
    {
        // Wait to be signalled to start the next trial run.
        _simStartEvent.WaitOne();

        while(true)
        {
            // Check if we have been signalled to terminate before starting a simulation run.
            if(_terminateSimThread)
                break;

            RunTrial();

            // Test if the thread should be terminated.
            if(_terminateSimThread)
                break;
        }

        // Signal any thread waiting for this simulation thread to terminate.
        _simNotRunningEvent.Set();
    }

    /// <summary>
    /// Run a single prey capture trial.
    /// </summary>
    private void RunTrial()
    {
        // Get local copy of agent so that the same agent is used throughout each individual simulation trial/run
        // (_agent is being continually updated by the evolution algorithm update events). This is probably an atomic
        // operation and thus thread safe.
        IBlackBox<TScalar> agent = _agent;

        // Init world state.
        _world.InitPositions();

        // Clear any prior agent state.
        agent.Reset();

        // Prey gets a head start (simulate world as normal but agent is frozen).
        int t = 0;
        for(; t < _world.PreyInitMoves && !_terminateSimThread; t++)
        {
            _world.SetAgentInputsAndActivate(agent);
            _world.MovePrey();

            // Repaint view on GUI thread.
            BeginInvoke(new MethodInvoker(PaintView));

            Thread.Sleep(80);
        }

        // Let the chase begin!
        bool exit = false;
        for(; t < _world.MaxTimesteps && !_terminateSimThread; t++)
        {
            _world.SetAgentInputsAndActivate(agent);
            _world.MoveAgent(agent);
            if(_world.IsPreyCaptured())
            {
                exit = true;
            }

            _world.MovePrey();
            if(_world.IsPreyCaptured())
            {   // The prey walked directly into the agent.
                exit = true;
            }

            // Repaint view on GUI thread.
            BeginInvoke(new MethodInvoker(PaintView));

            // Sleep. Even if the simulation is about to exit - that way we see the end result for a moment.
            Thread.Sleep(80);
            if(exit)
            {
                break;
            }
        }
    }

    protected override void PaintView()
    {
        if(_initializing || _world is null)
            return;

        Graphics g = Graphics.FromImage(Image);
        g.FillRectangle(_brushBackground, 0, 0, Image.Width, Image.Height);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Get control width and height.
        int width = Width;
        int height = Height;

        // Determine smallest dimension. Use that as the edge length of the square grid.
        height = Math.Min(width, height);

        // Pixel size is calculated using integer division to produce cleaner lines when drawing.
        // The inherent rounding down may produce a grid 1 pixel smaller then the determined edge length.
        // Also make room for a button above the grid (next test case button).
        int visualFieldPixelSize = (height - GridTop) / _world.GridSize;
        width = height = visualFieldPixelSize * _world.GridSize;

        // Paint pixel outline grid.
        // Vertical lines.
        int xg = GridLeft;
        for(int i = 0; i <= _world.GridSize; i++, xg += visualFieldPixelSize)
        {
            g.DrawLine(_penGrey, xg, GridTop, xg, GridTop+height);
        }

        // Horizontal lines.
        int yg = GridTop;
        for(int i = 0; i <= _world.GridSize; i++, yg += visualFieldPixelSize)
        {
            g.DrawLine(_penGrey, GridLeft, yg, GridLeft+width, yg);
        }

        // Paint grid squares. Background color.
        Brush sensorBrush = _world.IsPreyCaptured() ? _brushBackgroundSensorCaptured : _brushBackgroundSensor;

        yg = GridTop;
        for(int y = 0; y < _world.GridSize; y++, yg += visualFieldPixelSize)
        {
            xg = GridLeft;
            for(int x = 0; x<_world.GridSize; x++, xg += visualFieldPixelSize)
            {
                // Calc distance of square from agent.
                if(Int32Point.CalculateDistance(_world.AgentPosition, x, y) <= _world.SensorRange)
                {
                    g.FillRectangle(sensorBrush, xg+1, yg+1, visualFieldPixelSize-2, visualFieldPixelSize-2);
                }
            }
        }

        // Paint agent and prey squares.
        Int32Point a = _world.AgentPosition;
        Int32Point p = _world.PreyPosition;

        g.FillRectangle(
            _brushAgent,
            GridLeft+(a.X * visualFieldPixelSize)+1,
            GridTop+(a.Y * visualFieldPixelSize)+1,
            visualFieldPixelSize-2,
            visualFieldPixelSize-2);

        g.FillRectangle(
            _brushPrey,
            GridLeft+(p.X * visualFieldPixelSize)+1,
            GridTop+(p.Y * visualFieldPixelSize)+1,
            visualFieldPixelSize-2,
            visualFieldPixelSize-2);

        Refresh();
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
        NeatGenome<TScalar> neatGenome = genome as NeatGenome<TScalar>;
        _agent = _genomeDecoder.Decode(neatGenome);

        // Signal simulation thread to start running a one simulation.
        _simStartEvent.Set();
    }

    protected override void Dispose(bool disposing)
    {
        // Signal the simulation thread to terminate, and wait for it to terminate.
        _terminateSimThread = true;
        _simStartEvent.Set();
        _simNotRunningEvent.WaitOne();

        base.Dispose(disposing);

        _penGrey.Dispose();
        _brushBackground.Dispose();
        _brushBackgroundSensor.Dispose();
        _brushBackgroundSensorCaptured.Dispose();
        _brushAgent.Dispose();
        _brushPrey.Dispose();
        _agent.Dispose();
        _simStartEvent.Dispose();
        _simNotRunningEvent.Dispose();
    }
}
