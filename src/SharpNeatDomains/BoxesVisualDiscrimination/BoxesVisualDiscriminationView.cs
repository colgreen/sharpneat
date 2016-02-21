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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SharpNeat.Core;
using SharpNeat.Domains.BoxesVisualDiscrimination;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains
{
    partial class BoxesVisualDiscriminationView : AbstractDomainView
    {
        const int GridTop = 41;
        const int GridLeft = 2;
        const PixelFormat ViewportPixelFormat = PixelFormat.Format16bppRgb565;
        static readonly Pen __penGrey = new Pen(Color.LightGray, 1F);
        static readonly Pen __penBoxOutline = new Pen(Color.Orange, 2F);
        static readonly Pen __penSelectedPixel = new Pen(Color.Red, 2F);
        readonly Brush _brushBackground = new SolidBrush(Color.Lavender);

        IGenomeDecoder<NeatGenome,IBlackBox> _genomeDecoder;
        TestCaseField _testCaseField;
        int _largeBoxTestCase;
        Image _image;
        NeatGenome _neatGenome;
        IBlackBox _box;
        bool _initializing = true;

        /// <summary>
        /// The sensor and output pixel array resolution.
        /// </summary>
        int _visualFieldResolution;
        /// <summary>
        /// The experiment class that containsconfig settings parsed from the experiment config XML.
        /// </summary>
        BoxesVisualDiscriminationExperiment _experiment;
        /// <summary>
        /// The width and height of a pixel in the real coordinate system.
        /// </summary>
        double _visualPixelSize;
        /// <summary>
        /// The X and Y position of the origin pixel in the real coordinate system (the center position of the origin pixel).
        /// </summary>
        double _visualOriginPixelXY;

        #region Constructor

        /// <summary>
        /// Construct with an INeatExperiment. This provides config data parsed from the experiment config XML and a method for
        /// creating genome decoders for different visual resolutions.
        /// </summary>
        public BoxesVisualDiscriminationView(BoxesVisualDiscriminationExperiment experiment)
        {
            try
            {
                InitializeComponent();
                cbxResolution.SelectedIndex = 0;

                _experiment = experiment;
                SetResolution(_experiment.VisualFieldResolution);
                _testCaseField = new TestCaseField();  
                
                // Create a bitmap for the picturebox.
                int width = Width;
                int height = Height;
                _image = new Bitmap(width, height, ViewportPixelFormat);           
                pbx.Image = _image;
            }
            finally
            {
                _initializing = false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh/update the view with the provided genome.
        /// </summary>
        public override void RefreshView(object genome)
        {
            _box = null;
            _neatGenome = genome as NeatGenome;
            if(null == _neatGenome) {
                return;
            }

            // Decode genome.
            _box = _genomeDecoder.Decode(_neatGenome);

            // Generate new test case.
            _testCaseField.InitTestCase(_largeBoxTestCase++ % 3);

            // Paint test case and network response.
            PaintView(_box);
        }

        #endregion

        #region Private Methods

        private void SetResolution(int visualFieldResolution)
        {
            _visualFieldResolution = visualFieldResolution;
            _visualPixelSize = BoxesVisualDiscriminationEvaluator.VisualFieldEdgeLength / _visualFieldResolution;
            _visualOriginPixelXY = -1 + (_visualPixelSize/2.0);
            _genomeDecoder = _experiment.CreateGenomeDecoder(visualFieldResolution, _experiment.LengthCppnInput);
            _box = null;

            // If we have a cached genome then re-decode it using the new decoder (with the updated resolution).
            if(null != _neatGenome) {
                _box = _genomeDecoder.Decode(_neatGenome);
            }
        }

        private void PaintView(IBlackBox box)
        {
            if(_initializing) {
                return;
            }

            Graphics g = Graphics.FromImage(_image);
            g.FillRectangle(_brushBackground, 0, 0, _image.Width, _image.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get control width and height.
            int width = Width;
            int height = Height;

            // Determine smallest dimension. Use that as the edge length of the square grid.
            width = height = Math.Min(width, height);

            // Pixel size is calculated using integer division to produce cleaner lines when drawing.
            // The inherent rounding down may produce a grid 1 pixel smaller then the determined edge length.
            // Also make room for a combo box above the grid. This will be used allow the user to change the grid resolution.
            int visualFieldPixelSize = (height-GridTop) / _visualFieldResolution;
            width = height = visualFieldPixelSize * _visualFieldResolution;

            // Paint pixel outline grid.
            // Vertical lines.
            int x = GridLeft;
            for(int i=0; i<=_visualFieldResolution; i++, x+=visualFieldPixelSize) {
                g.DrawLine(__penGrey, x, GridTop, x, GridTop+height);
            }

            // Horizontal lines.
            int y = GridTop;
            for(int i=0; i<=_visualFieldResolution; i++, y+=visualFieldPixelSize) {
                g.DrawLine(__penGrey, GridLeft, y, GridLeft+width, y);
            }

            // Apply test case to black box's visual field sensor inputs.
            BoxesVisualDiscriminationEvaluator.ApplyVisualFieldToBlackBox(_testCaseField, box, _visualFieldResolution, _visualOriginPixelXY, _visualPixelSize);

            // Activate the black box.
            box.ResetState();
            box.Activate();

            // Read output responses and paint them to the pixel grid.
            // Also, we determine the range of values so that we can normalize the range of pixel shades used.
            double low, high;
            low = high = box.OutputSignalArray[0];
            for(int i=1; i<box.OutputSignalArray.Length; i++)
            {
                double val = box.OutputSignalArray[i];
                if(val > high) {
                    high = val;
                } else if(val <low) {
                    low = val;
                }
            }

            double colorScaleFactor;
            if(0.0 == (high-low)) {
                colorScaleFactor = 1.0;
            } else {
                colorScaleFactor = 1.0 / (high-low);
            }

            IntPoint maxActivationPoint = new IntPoint(0,0);
            double  maxActivation = -1.0;
            int outputIdx = 0;
            y = GridTop;
            for(int i=0; i<_visualFieldResolution; i++, y+=visualFieldPixelSize)
            {
                x = GridLeft;
                for(int j=0; j<_visualFieldResolution; j++, x+=visualFieldPixelSize, outputIdx++)
                {
                    double output = box.OutputSignalArray[outputIdx];
                    if(output > maxActivation)
                    {
                        maxActivation = output;
                        maxActivationPoint._x = j;
                        maxActivationPoint._y = i;
                    }

                    Color color = GetResponseColor((output-low)*colorScaleFactor);
                    Brush brush = new SolidBrush(color);
                    g.FillRectangle(brush, x+1, y+1, visualFieldPixelSize-2, visualFieldPixelSize-2);
                }
            }

            // Paint lines around the small and large test case boxes to highlight them.
            // Small box.
            int testFieldPixelSize = (_visualFieldResolution / TestCaseField.TestFieldResolution) * visualFieldPixelSize;
            g.DrawRectangle(__penBoxOutline,
                            GridLeft + (_testCaseField.SmallBoxTopLeft._x * testFieldPixelSize),
                            GridTop + (_testCaseField.SmallBoxTopLeft._y * testFieldPixelSize),
                            testFieldPixelSize, testFieldPixelSize);

            // Large box.
            g.DrawRectangle(__penBoxOutline,
                            GridLeft + (_testCaseField.LargeBoxTopLeft._x * testFieldPixelSize),
                            GridTop + (_testCaseField.LargeBoxTopLeft._y * testFieldPixelSize),
                            testFieldPixelSize*3, testFieldPixelSize*3);
            
            // Draw red line around pixel with highest activation.
            g.DrawRectangle(__penSelectedPixel,
                            GridLeft + (maxActivationPoint._x * visualFieldPixelSize),
                            GridTop + (maxActivationPoint._y * visualFieldPixelSize),
                            visualFieldPixelSize, visualFieldPixelSize);

            Refresh();
        }

        private Color GetResponseColor(double response)
        {
            if(response > 1.0) {
                response = 1.0;
            } 
            else if(response < 0.0) {
                response = 0.0;
            }
            
            int component = (int)((1.0 - response) * 255.0);
            return Color.FromArgb(component, component, component);
        }

        #endregion

        #region Event Handlers

        private void pbx_SizeChanged(object sender, System.EventArgs e)
        {
            const float ImageSizeChangeDelta = 100f;

            if(_initializing) {
                return;
            }

            // Track viewport area.
            int width = Width;
            int height = Height;

            // If the viewport has grown beyond the size of the image then create a new image. 
            // Note. If the viewport shrinks we just paint on the existing (larger) image, this prevents unnecessary 
            // and expensive construction/destrucion of Image objects.
            if(width > _image.Width || height > _image.Height) 
            {   // Reset the image's size. We round up the the nearest __imageSizeChangeDelta. This prevents unnecessary 
                // and expensive construction/destrucion of Image objects as the viewport is resized multiple times.
                int imageWidth = (int)(Math.Ceiling((float)width / ImageSizeChangeDelta) * ImageSizeChangeDelta);
                int imageHeight = (int)(Math.Ceiling((float)height / ImageSizeChangeDelta) * ImageSizeChangeDelta);
                _image = new Bitmap(imageWidth, imageHeight, ViewportPixelFormat);
                pbx.Image = _image;
            }
            
            // Repaint control.
            if(null != _box) {
                PaintView(_box);
            }
        }

        private void cbxResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_initializing) {
                return;
            }

            int resolution;
            if(int.TryParse(cbxResolution.Text, out resolution)) {
                SetResolution(resolution);        
            }

            // Repaint control.
            if(null != _box)  {
                PaintView(_box);
            }
        }

        private void btnNextTestCase_Click(object sender, EventArgs e)
        {
            // Generate new test case and repaint control.
            if(null != _box) 
            {
                _testCaseField.InitTestCase(_largeBoxTestCase++ % 3);
                PaintView(_box);
            }
        }

        private void pbx_MouseMove(object sender, MouseEventArgs e)
        {
            if(_initializing || null == _box || (!e.Button.HasFlag(MouseButtons.Left) && !e.Button.HasFlag(MouseButtons.Right))) {
                return;
            }

            // Determine visual field pixel coordinate that the mouse is over.
            // Determine grid edge length.
            int width = Width;
            int height = Height;
            height = Math.Min(width, height);
            int testFieldPixelSize = (height-GridTop) / TestCaseField.TestFieldResolution;

            int testFieldPixelX = (e.X - GridLeft) / testFieldPixelSize;
            int testFieldPixelY = (e.Y - GridTop) / testFieldPixelSize;

            if(e.Button.HasFlag(MouseButtons.Left))
            {
                _testCaseField.LargeBoxTopLeft = new IntPoint(testFieldPixelX-1, testFieldPixelY-1);
            }

            if(e.Button.HasFlag(MouseButtons.Right))
            {
                _testCaseField.SmallBoxTopLeft = new IntPoint(testFieldPixelX, testFieldPixelY);
            }

            PaintView(_box);
        }

        #endregion
    }
}
