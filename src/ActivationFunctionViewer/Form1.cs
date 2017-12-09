using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ActivationFnBenchmarks;
using KBCsv;
using ZedGraph;

namespace ActivationFunctionViewer
{
    public partial class Form1 : Form
    {
        #region Constructor

        public Form1()
        {
            InitializeComponent();

            PlotAllFunctions();

            // LogisticFunctionSteep
            //WriteToCsv(Functions.LogisticApproximantSteep);
        }

        #endregion

        #region Private Methods [Graph Plots]

        private void PlotAllFunctions()
        {
            // First, clear out any old GraphPane's from the MasterPane collection
            MasterPane master = zed.MasterPane;
            master.PaneList.Clear();

            // Display the MasterPane Title, and set the outer margin to 10 points
            master.Title.IsVisible = true;
            master.Margin.All = 10;

            // Plot multiple functions arranged on a master pane.
            PlotOnMasterPane(Functions.LogisticApproximantSteep, "Logistic Steep (Approximant)");
            PlotOnMasterPane(Functions.LogisticFunctionSteep, "Logistic Steep (Function)");
            PlotOnMasterPane(Functions.SoftSign, "Soft Sign");
            PlotOnMasterPane(Functions.PolynomialApproximant, "Polynomial Approximant");
            PlotOnMasterPane(Functions.QuadraticSigmoid, "Quadratic Sigmoid");
            PlotOnMasterPane(Functions.ReLU, "ReLU");
            PlotOnMasterPane(Functions.LeakyReLU, "Leaky ReLU");
            PlotOnMasterPane(Functions.LeakyReLUShifted, "Leaky ReLU (Shifted)");
            PlotOnMasterPane(Functions.SReLU, "S-Shaped ReLU");
            PlotOnMasterPane(Functions.SReLUShifted, "S-Shaped ReLU (Shifted)");
            PlotOnMasterPane(Functions.ArcTan, "ArcTan");
            PlotOnMasterPane(Functions.TanH, "TanH");
            PlotOnMasterPane(Functions.ArcSinH, "ArcSinH");
            PlotOnMasterPane(Functions.ScaledELU, "Scaled Exponential Linear Unit");

            // Refigure the axis ranges for the GraphPanes.
            zed.AxisChange();

            // Layout the GraphPanes using a default Pane Layout.
            using (Graphics g = this.CreateGraphics()) {
                master.SetLayout(g, PaneLayout.SquareColPreferred);
            }
        }

        private void PlotOnMasterPane(Func<double, double> fn, string fnName)
        {
            GraphPane pane = new GraphPane();
            Plot(fn, fnName, Color.Black, pane);
            zed.MasterPane.Add(pane);
        }

        private void Plot(Func<double, double> fn, string fnName, Color graphColor, GraphPane gpane = null)
        {
            const double xmin = -2.0;
            const double xmax = 2.0;
            const int resolution = 2000;

            zed.IsShowPointValues = true;
            zed.PointValueFormat = "e";

            GraphPane pane;

            if (gpane == null)
                pane = zed.GraphPane;
            else
                pane = gpane;

            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;

            pane.Title.Text = fnName;
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
            LineItem li = pane.AddCurve(string.Empty, list1, graphColor, SymbolType.None);
            li.Symbol.Fill = new Fill(Color.White);
            pane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0F);
        }

        #endregion

        #region Private Static Methods [Misc]

        private static void WriteToCsv(Func<double, double> fn)
        {
            const double xmin = -2.0;
            const double xmax = 2.0;
            const int resolution = 2000;

            double[] xarr = new double[resolution];
            double[] yarr = new double[resolution];

            double incr = (xmax - xmin) / resolution;
            double x = xmin;

            for (int i = 0; i < resolution; i++, x += incr)
            {
                xarr[i] = x;
                yarr[i] = fn(x);
            }

            using (var sw = new StreamWriter("fn.csv"))
            using (var csvWriter = new CsvWriter(sw))
            {
                // Header row.
                csvWriter.WriteRecord("x", "y");

                for(int i=0; i< xarr.Length; i++) {
                    csvWriter.WriteRecord(xarr[i].ToString(), yarr[i].ToString());
                }
            }
        }

        #endregion
    }
}
