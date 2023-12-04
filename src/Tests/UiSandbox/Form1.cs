using Redzen.Collections;
using SharpNeat.Drawing;
using SharpNeat.Drawing.Graph;
using SharpNeat.Graphs;

#pragma warning disable IDE1006 // Naming Styles

namespace UiSandbox;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender,EventArgs e)
    {
        var graphViewportPainter = new GraphViewportPainter();
        viewportControl1.ViewportPainter = graphViewportPainter;

        DirectedGraphViewModel graphViewModel = CreateGraphViewModel();

        graphViewportPainter.GraphViewModel = graphViewModel;

        viewportControl1.RepaintViewport();
        viewportControl1.Refresh();
    }


    #region Private Static Methods

    private static DirectedGraphViewModel CreateGraphViewModel()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<WeightedDirectedConnection<float>>
        {
            new(0, 4, 1f),
            new(4, 5, 2f),
            new(5, 2, 3f),
            new(1, 2, 4f),
            new(2, 2, 5f),
            new(2, 3, 5f),
            new(2, 4, 5f),
            new(2, 5, 5f)
        };
        var connSpan = connList.AsSpan();

        connSpan.Sort(WeightedDirectedConnectionComparer<float>.Default);

        // Create graph.
        var digraph = WeightedDirectedGraphBuilder<float>.Create(connSpan, 2, 2);

        // Create graph view model, and return.
        INodeIdMap nodeIdByIdx = CreateNodeIdByIdx(digraph.TotalNodeCount);
        DirectedGraphViewModel graphViewModel = new(digraph, digraph.WeightArray, nodeIdByIdx);
        return graphViewModel;
    }

    private static INodeIdMap CreateNodeIdByIdx(int length)
    {
        var arr = new int[length];
        for(int i=0; i < length; i++)
            arr[i] = i;

        return new ArrayNodeIdMap(arr);
    }

    #endregion
}
