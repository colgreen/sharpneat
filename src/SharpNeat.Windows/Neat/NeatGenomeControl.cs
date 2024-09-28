﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Drawing.Graph;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Windows.Neat;

#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

/// <summary>
/// A Windows.Form <see cref="UserControl"/> for displaying NEAT genomes.
/// </summary>
public class NeatGenomeControl : GenomeControl
{
    private ViewportControl viewportControl1;
    private readonly GraphViewportPainter _graphViewportPainter;

    /// <summary>
    /// Constructs a new instance of <see cref="NeatGenomeControl"/>.
    /// </summary>
    public NeatGenomeControl()
    {
        InitializeComponent();
        _graphViewportPainter = new GraphViewportPainter();
        viewportControl1.ViewportPainter = _graphViewportPainter;
    }

    /// <summary>
    /// Genome updated handler.
    /// </summary>
    public override void OnGenomeUpdated()
    {
        base.OnGenomeUpdated();

        // Get a local reference to avoid possible race conditions on the class field.
        IGenome genome = _genome;

        if(genome is null)
        {
            _graphViewportPainter.GraphViewModel = null;
        }
        else
        {
            DirectedGraphViewModel graphViewModel = CreateGraphViewModel(genome);
            _graphViewportPainter.GraphViewModel = graphViewModel;
        }

        // Repaint the viewport.
        viewportControl1.RepaintViewport();

        // Update the control/window to show the updated/repainted viewport.
        viewportControl1.Refresh();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            viewportControl1?.Dispose();
            _graphViewportPainter?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Private Methods [Windows.Forms Designer Code]

    private void InitializeComponent()
    {
#pragma warning disable SA1120 // Comments should contain text

        this.viewportControl1 = new SharpNeat.Windows.ViewportControl();
        this.SuspendLayout();
        //
        // viewportControl1
        //
        this.viewportControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.viewportControl1.Location = new System.Drawing.Point(0,0);
        this.viewportControl1.Name = "viewportControl1";
        this.viewportControl1.Size = new System.Drawing.Size(150,150);
        this.viewportControl1.TabIndex = 0;
        this.viewportControl1.ViewportPainter = null;
        this.viewportControl1.ZoomFactor = 1F;
        //
        // NeatGenomeControl
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F,15F);
        this.Controls.Add(this.viewportControl1);
        this.Name = "NeatGenomeControl";
        this.ResumeLayout(false);
    }

    #endregion

    #region Private Static Methods

    private static DirectedGraphViewModel CreateGraphViewModel(IGenome genome)
    {
        DirectedGraph digraph;
        float[] weightArr;
        INodeIdMap nodeIdByIdxMap;

        if(genome is NeatGenome<double> neatGenomeDouble)
        {
            digraph = neatGenomeDouble.DirectedGraph;
            weightArr = ToFloatArray(neatGenomeDouble.GetDigraphWeightArray());
            nodeIdByIdxMap = neatGenomeDouble.NodeIndexByIdMap.CreateInverseMap();
        }
        else if(genome is NeatGenome<float> neatGenomeFloat)
        {
            digraph = neatGenomeFloat.DirectedGraph;
            weightArr = neatGenomeFloat.GetDigraphWeightArray();
            nodeIdByIdxMap = neatGenomeFloat.NodeIndexByIdMap.CreateInverseMap();
        }
        else
        {
            throw new InvalidOperationException("The genome object is not a NeatGenome.");
        }

        DirectedGraphViewModel graphViewModel = new(digraph, weightArr, nodeIdByIdxMap);
        return graphViewModel;
    }

    private static float[] ToFloatArray(double[] src)
    {
        var arr = new float[src.Length];
        for(int i=0; i < src.Length; i++)
            arr[i] = (float)src[i];

        return arr;
    }

    #endregion
}
