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
            Init(Functions.LogisticApproximantSteep);
            //WriteToCsv(Functions.LogisticApproximantSteep);
        }

        private void Init(Func<double,double> fn)
        {
            const double xmin = -2.0;
            const double xmax = 2.0;
            const int resolution = 2000;

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

        //private static void WriteToCsv(Func<double, double> fn)
        //{
        //    const double xmin = -2.0;
        //    const double xmax = 2.0;
        //    const int resolution = 2000;

        //    double[] xarr = new double[resolution];
        //    double[] yarr = new double[resolution];

        //    double incr = (xmax - xmin) / resolution;
        //    double x = xmin;

        //    for (int i = 0; i < resolution; i++, x += incr)
        //    {
        //        xarr[i] = x;
        //        yarr[i] = fn(x);
        //    }

        //    using (var sw = new StreamWriter("fn.csv"))
        //    using (var csvWriter = new CsvWriter(sw))
        //    {
        //        // Header row.
        //        csvWriter.WriteRecord("x", "y");

        //        for(int i=0; i< xarr.Length; i++) {
        //            csvWriter.WriteRecord(xarr[i].ToString(), yarr[i].ToString());
        //        }
        //    }
        //}
    }
}
