/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using SharpNeat.View.Graph;
using SharpNeat.Genomes.Neat;

namespace SharpNeat.Domains
{
    /// <summary>
    /// General purpose form for hosting genome view controls.
    /// </summary>
    public partial class NeatGenomeView : AbstractGenomeView
    {
        NetworkGraphFactory _graphFactory = new NetworkGraphFactory();
        IOGraphViewportPainter _viewportPainter;

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NeatGenomeView()
        {
            InitializeComponent();
            graphControl1.ViewportPainter = _viewportPainter = new IOGraphViewportPainter(new IOGraphPainter());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh/update the view with the provided genome.
        /// </summary>
        public override void RefreshView(object genome)
        {
            NeatGenome neatGenome = genome as NeatGenome;
            if(null == neatGenome) {
                return;
            }

            IOGraph graph = _graphFactory.CreateGraph(neatGenome);
            _viewportPainter.IOGraph = graph;
            graphControl1.RefreshImage();
        }

        #endregion
    }
}
