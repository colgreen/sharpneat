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
using System;
using System.Windows.Forms;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Utility;
using ZedGraph;

namespace SharpNeatGUI
{
    /// <summary>
    /// Form for displaying a graph plot of summary information (e.g. distribution curves).
    /// </summary>
    public partial class SummaryGraphForm : Form
    {
        AbstractGenerationalAlgorithm<NeatGenome> _ea;
        SummaryDataSource[] _dataSourceArray;
        PointPairList[] _pointPlotArray;
        GraphPane _graphPane;

        #region Constructor

        /// <summary>
        /// Construct the form with the provided details and data sources.
        /// </summary>
        public SummaryGraphForm(string title, string xAxisTitle, string y1AxisTitle, string y2AxisTitle,
                         SummaryDataSource[] dataSourceArray, AbstractGenerationalAlgorithm<NeatGenome> ea)
        {
            InitializeComponent();

            this.Text = string.Format("SharpNEAT Graph - {0}", title);
            _dataSourceArray = dataSourceArray;
            InitGraph(title, xAxisTitle, y1AxisTitle, y2AxisTitle, dataSourceArray);

            _ea = ea;
            if(null != ea) {
                _ea.UpdateEvent += new EventHandler(_ea_UpdateEvent);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when a new evolution algorithm is initialized. Clean up any existing event listeners and
        /// connect up to the new evolution algorithm.
        /// </summary>
        public void Reconnect(AbstractGenerationalAlgorithm<NeatGenome> ea)
        {
            // Clean up.
            if(null != _ea) {
                _ea.UpdateEvent -= new EventHandler(_ea_UpdateEvent);
            }

            foreach(PointPairList ppl in _pointPlotArray) {
                ppl.Clear();
            }

            // Reconnect.
            _ea = ea;
            _ea.UpdateEvent += new EventHandler(_ea_UpdateEvent);
        }

        #endregion

        #region Private Methods

        private void InitGraph(string title, string xAxisTitle, string y1AxisTitle, string y2AxisTitle, SummaryDataSource[] dataSourceArray)
        {
            _graphPane = zed.GraphPane;
            _graphPane.Title.Text = title;

			_graphPane.XAxis.Title.Text = xAxisTitle;
			_graphPane.XAxis.MajorGrid.IsVisible = true;

			_graphPane.YAxis.Title.Text = y1AxisTitle;
			_graphPane.YAxis.MajorGrid.IsVisible = true;

			_graphPane.Y2Axis.Title.Text = y2AxisTitle;
			_graphPane.Y2Axis.MajorGrid.IsVisible = false;

            // Create point-pair lists and bind them to the graph control.
            int sourceCount = dataSourceArray.Length;
            _pointPlotArray = new PointPairList[sourceCount];
            for(int i=0; i<sourceCount; i++)
            {
                SummaryDataSource ds = dataSourceArray[i];
                _pointPlotArray[i] =new PointPairList();
                LineItem lineItem = _graphPane.AddCurve(ds.Name,  _pointPlotArray[i], ds.Color, SymbolType.None);
                lineItem.IsY2Axis = (ds.YAxis == 1);
            }
        }

        private void EnsurePointPairListLength(PointPairList ppl, int length)
        {
            int delta = length-ppl.Count;
            
            if(delta > 0)
            {   // Add additional points.
                for(int i=0; i<delta; i++) {
                    ppl.Add(0.0, 0.0);
                }
            }
            else if(delta < 0)
            {   // Remove excess points.
                ppl.RemoveRange(length, -delta);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle update event from the evolution algorithm - update the view.
        /// </summary>
        public void _ea_UpdateEvent(object sender, EventArgs e)
        {
            // Switch execution to GUI thread if necessary.
            if(this.InvokeRequired)
            {
                // Must use Invoke(). BeginInvoke() will execute asynchronously and the evolution algorithm therefore 
                // may have moved on and will be in an intermediate and indeterminate (between generations) state.
                this.Invoke(new MethodInvoker(delegate() 
                {
                    if(this.IsDisposed) {
                        return;
                    }

                    // Update plot points for each series in turn.
                    int sourceCount = _dataSourceArray.Length;
                    for(int i=0; i<sourceCount; i++)
                    {
                        SummaryDataSource ds = _dataSourceArray[i];
                        Point2DDouble[] pointArr = ds.GetPointArray();
                        PointPairList ppl = _pointPlotArray[i];
                        EnsurePointPairListLength(ppl, pointArr.Length);

                        for(int j=0; j<pointArr.Length; j++)
                        {
                            ppl[j].X = pointArr[j].X;
                            ppl[j].Y = pointArr[j].Y;
                        }
                    }

                    // Trigger graph to redraw.
                    zed.AxisChange();
                    Refresh();
                }));
            }
        }

        private void GenomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(null != _ea) {
                _ea.UpdateEvent -= new EventHandler(_ea_UpdateEvent);
            }
        }

        #endregion
    }
}
