/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
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
using System.Drawing;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using ZedGraph;

namespace SharpNeat.Domains.FunctionRegression
{
    /// <summary>
    /// Domain View for function regression with one input and one output.
    /// Plots function on 2D graph.
    /// </summary>
    public partial class FunctionRegressionView2D : AbstractDomainView
    {
        IFunction _func;
        double _xMin;
        double _xIncr;
        int _sampleCount;
        IGenomeDecoder<NeatGenome,IBlackBox> _genomeDecoder;
        PointPairList _plotPointListTarget;
        PointPairList _plotPointListResponse;

        #region Constructor

        /// <summary>
        /// Constructs with the details of teh function regression problem to be visualized. 
        /// </summary>
        /// <param name="func">The function being regressed.</param>
        /// <param name="xMin">The minimum value of the input range being sampled.</param>
        /// <param name="xIncr">The increment between input sample values.</param>
        /// <param name="sampleCount">The number of samples over the input range.</param>
        /// <param name="genomeDecoder">Genome decoder.</param>
        public FunctionRegressionView2D(IFunction func, double xMin, double xIncr, int sampleCount, IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
        {
            InitializeComponent();
            InitGraph(string.Empty, string.Empty, string.Empty);

            _func = func;
            _xMin = xMin;
            _xIncr = xIncr;
            _sampleCount = sampleCount;
            _genomeDecoder = genomeDecoder;

            // Prebuild plot point objects.
            _plotPointListTarget = new PointPairList();
            _plotPointListResponse = new PointPairList();
            
            double[] args = new double[]{xMin};
            for(int i=0; i<sampleCount; i++, args[0] += xIncr)
            {
                _plotPointListTarget.Add(args[0], _func.GetValue(args));
                _plotPointListResponse.Add(args[0], 0.0);
            }

            // Bind plot points to graph.
            zed.GraphPane.AddCurve("Target", _plotPointListTarget, Color.Black, SymbolType.None);
            zed.GraphPane.AddCurve("Network Response", _plotPointListResponse, Color.Red, SymbolType.None);
        }

        #endregion

        #region Private Methods

        private void InitGraph(string title, string xAxisTitle, string yAxisTitle)
        {
            GraphPane graphPane = zed.GraphPane;
            graphPane.Title.Text = title;

			graphPane.XAxis.Title.Text = xAxisTitle;
			graphPane.XAxis.MajorGrid.IsVisible = true;

			graphPane.YAxis.Title.Text = yAxisTitle;
			graphPane.YAxis.MajorGrid.IsVisible = true;
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

            // Decode genome.
            IBlackBox box = _genomeDecoder.Decode(neatGenome);
            
            // Update plot points.
            double x = _xMin;
            for(int i=0; i<_sampleCount;i++, x += _xIncr) 
            {
                box.ResetState();
                box.InputSignalArray[0] = x;
                box.Activate();
                _plotPointListResponse[i].Y = box.OutputSignalArray[0];
            }

            // Trigger graph to redraw.
            zed.AxisChange();
            Refresh();
        }

        #endregion
    }
}
