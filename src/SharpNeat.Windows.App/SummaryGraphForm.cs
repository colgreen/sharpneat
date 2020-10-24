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
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// Form for displaying a graph plot of summary information (e.g. distribution curves).
    /// </summary>
    public partial class SummaryGraphForm : Form
    {
        readonly SummaryDataSource[] _dataSourceArray;
        PointPairList[] _pointPlotArray;
        GraphPane _graphPane;
        readonly Color[] _plotColorArr = new Color[] { Color.LightSlateGray, Color.LightBlue, Color.LightGreen };

        #region Constructor

        /// <summary>
        /// Construct the form with the provided details and data sources.
        /// </summary>
        public SummaryGraphForm(
            string title, string xAxisTitle,
            string y1AxisTitle, string y2AxisTitle,
            SummaryDataSource[] dataSourceArray)
        {
            InitializeComponent();

            this.Text = $"SharpNEAT - {title}";
            _dataSourceArray = dataSourceArray;
            InitGraph(title, xAxisTitle, y1AxisTitle, y2AxisTitle, dataSourceArray);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh view.
        /// </summary>
        public void RefreshView()
        {
            if(this.InvokeRequired)
            {
                // Note. Must use Invoke(). BeginInvoke() will execute asynchronously and the evolution algorithm therefore 
                // may have moved on and will be in an intermediate and indeterminate (between generations) state.
                this.Invoke(new MethodInvoker(delegate()
                {
                    RefreshView();
                }));
                return;
            }

            if(this.IsDisposed) {
                return;
            }
                
            // Update plot points for each series in turn.
            int sourceCount = _dataSourceArray.Length;
            for(int i=0; i < sourceCount; i++)
            {
                SummaryDataSource ds = _dataSourceArray[i];
                Point2DDouble[] pointArr = ds.GetPointArray();
                PointPairList ppl = _pointPlotArray[i];
                EnsurePointPairListLength(ppl, pointArr.Length);

                for(int j=0; j < pointArr.Length; j++)
                {
                    ppl[j].X = pointArr[j].X;
                    ppl[j].Y = pointArr[j].Y;
                }
            }

            // Trigger graph to redraw.
            zed.AxisChange();
            Refresh();
        }

        #endregion

        #region Private Methods

        private void InitGraph(
            string title, string xAxisTitle,
            string y1AxisTitle, string y2AxisTitle,
            SummaryDataSource[] dataSourceArray)
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

            for(int i=0; i < sourceCount; i++)
            {
                SummaryDataSource ds = dataSourceArray[i];
                _pointPlotArray[i] =new PointPairList();

                Color color = _plotColorArr[i % 3];
                BarItem barItem = _graphPane.AddBar(ds.Name, _pointPlotArray[i], color);
                barItem.Bar.Fill = new Fill(color);
                _graphPane.BarSettings.MinClusterGap = 0;

                barItem.IsY2Axis = (ds.YAxis == 1);
            }
        }

        private static void EnsurePointPairListLength(PointPairList ppl, int length)
        {
            int delta = length - ppl.Count;
            
            if(delta > 0)
            {   
                // Add additional points.
                for(int i=0; i < delta; i++) {
                    ppl.Add(0.0, 0.0);
                }
            }
            else if(delta < 0)
            {   
                // Remove excess points.
                ppl.RemoveRange(length, -delta);
            }
        }

        #endregion
    }
}
