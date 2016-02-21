using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

using SharpNeat.View;
using SharpNeat.View.Neat;


namespace SharpNeatGUI
{
    public partial class GenericPlotForm : Form
    {
        public delegate PlotPoint[] PlotPointSource();

        PointPairList _ppl;
        PlotPointSource _plotPointSourceDelegate;

        public GenericPlotForm(PlotPointSource plotPointSourceDelegate)
        {
            InitializeComponent();
            _plotPointSourceDelegate = plotPointSourceDelegate;
            _ppl = new PointPairList();
            zed.GraphPane.AddCurve("", _ppl, Color.Black, SymbolType.None);
        }

        public void UpdatePlot()
        {
            PlotPoint[] dataPointArr = _plotPointSourceDelegate();
            int count = dataPointArr.Length;

            if(_ppl.Count != count)
            {
                _ppl.Capacity = count;
                _ppl.Clear();
                for(int i=0; i<count; i++) 
                {
                    _ppl.Add(0, 0);
                }
            }

            for(int i=0; i<count; i++) 
            {
                _ppl[i].X = dataPointArr[i]._x;
                _ppl[i].Y = dataPointArr[i]._y;
                _ppl[i].Tag = dataPointArr[i]._tag;
            }

            zed.AxisChange();
            Refresh();
        }
    }
}
