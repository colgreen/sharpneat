using ActivationFnBenchmarks;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace ActivationFunctionViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init(Functions.ArcTan);
        }

        private void Init(Func<double,double> fn)
        {
            const double xmin = -100.0;
            const double xmax = 100;
            const int resolution = 80000;

            zed.IsShowPointValues = true;
            zed.PointValueFormat = "e";
            GraphPane pane = zed.GraphPane;

            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;

            pane.Title.Text = "Activation Function";
            pane.YAxis.Title.Text = "";
            pane.XAxis.Title.Text = "";

            double[] xarr = new double[resolution];
            double[] yarr = new double[resolution];

            double incr = (xmax - xmin) / resolution;
            double x = xmin;

            for(int i=0; i < resolution; i++, x+=incr)
            {
                xarr[i] = x;
                yarr[i] = fn(x);
            }

            PointPairList list1 = new PointPairList(xarr, yarr);
            LineItem li = pane.AddCurve("Actual", list1, Color.Red, SymbolType.None);

            zed.AxisChange();
        }
    }
}
