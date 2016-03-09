/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using SharpNeat.View.Graph;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;

namespace SharpNeat.Domains
{
    /// <summary>
    /// GUI control for visualizing CPPN genomes.
    /// </summary>
    public partial class CppnGenomeView : AbstractGenomeView
    {
        NetworkGraphFactory _graphFactory = new NetworkGraphFactory();
        IOGraphViewportPainter _viewportPainter;

        #region Constructor

        /// <summary>
        /// Construct with the provided CPPN activation function library to draw CPPNs with (genome nodes contain an index into this library).
        /// </summary>
        /// <param name="actFnLib"></param>
        public CppnGenomeView(IActivationFunctionLibrary actFnLib)
        {
            InitializeComponent();
            graphControl1.ViewportPainter = _viewportPainter = new IOGraphViewportPainter(new CppnGraphPainter(actFnLib));
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
