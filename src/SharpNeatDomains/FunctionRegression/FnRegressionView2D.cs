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
    public partial class FnRegressionView2D : AbstractDomainView
    {
        IFunction _fn;
        ParamSamplingInfo _paramSamplingInfo;
        bool _generativeMode;
        IBlackBoxProbe _blackBoxProbe;
        readonly double[] _yArrTarget;
        IGenomeDecoder<NeatGenome,IBlackBox> _genomeDecoder;
        PointPairList _plotPointListTarget;
        PointPairList _plotPointListResponse;

        #region Constructor

        /// <summary>
        /// Constructs with the details of the function regression problem to be visualized. 
        /// </summary>
        /// <param name="fn">The function being regressed.</param>
        /// <param name="generativeMode">Indicates that blacbox has no inputs; it will generate a waveform as a function of time.</param>
        /// <param name="paramSamplingInfo">Parameter sampling info.</param>
        /// <param name="genomeDecoder">Genome decoder.</param>
        public FnRegressionView2D(IFunction fn, ParamSamplingInfo paramSamplingInfo, bool generativeMode, IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
        {
            InitializeComponent();
            InitGraph(string.Empty, string.Empty, string.Empty);

            _fn = fn;
            _paramSamplingInfo = paramSamplingInfo;
            _generativeMode = generativeMode;
            _genomeDecoder = genomeDecoder;

            // Determine the mid output value of the function (over the specified sample points) and a scaling factor
            // to apply the to neural netwkrk response for it to be able to recreate the function (because the neural net
            // output range is [0,1] when using the logistic function as the neurn activation function).
            double scale, mid;
            FnRegressionUtils.CalcFunctionMidAndScale(fn, paramSamplingInfo, out mid, out scale);
            if(generativeMode) {
                _blackBoxProbe = new GenerativeBlackBoxProbe(paramSamplingInfo, mid, scale);
            } else {
                _blackBoxProbe = new BlackBoxProbe(paramSamplingInfo, mid, scale);
            }
            
            _yArrTarget = new double[paramSamplingInfo._sampleCount];

            // Pre-build plot point objects.
            _plotPointListTarget = new PointPairList();
            _plotPointListResponse = new PointPairList();
            
            double[] xArr = paramSamplingInfo._xArr;
            for(int i=0; i<xArr.Length; i++)
            {
                double x = xArr[i];
                _plotPointListTarget.Add(x, _fn.GetValue(x));
                _plotPointListResponse.Add(x, 0.0);
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

            // Probe the black box.
            _blackBoxProbe.Probe(box, _yArrTarget);

            // Update plot points.
            double[] xArr = _paramSamplingInfo._xArr;
            for(int i=0; i < xArr.Length; i++) 
            {
                if(!_generativeMode) {
                    box.ResetState();
                    box.InputSignalArray[0] = xArr[i];
                }
                box.Activate();
                _plotPointListResponse[i].Y = _yArrTarget[i];
            }

            // Trigger graph to redraw.
            zed.AxisChange();
            Refresh();
        }

        #endregion
    }
}
