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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SharpNeat.View
{
    /// <summary>
    /// A user control that provides a re-sizable area that can be painted to by an 
    /// IViewportPainter.
    /// </summary>
    public class Viewport : UserControl
    {
        const PixelFormat ViewportPixelFormat = PixelFormat.Format16bppRgb565;
        readonly Brush _brushBackground = new SolidBrush(Color.Lavender);

        IViewportPainter _viewportPainter;
        Rectangle _viewportArea;
        float _zoomFactor = 1f;
        Image _image;
        
        #region Component designer variables

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
        private System.Windows.Forms.PictureBox pictureBox;

        #endregion

        #region Constructor / Disposal

        /// <summary>
        /// Default constructor. Required for user controls.
        /// </summary>
        public Viewport()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            int width = Width;
            int height = Height;
            _viewportArea = new Rectangle(0, 0, width, height);

            // Create a bitmap for the picturebox.
            _image = new Bitmap(width, height, ViewportPixelFormat);           
            pictureBox.Image = _image;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing) {
                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Component Designer Generated Code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(216, 232);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.SizeChanged += new System.EventHandler(this.PictureBox_SizeChanged);
            this.pictureBox.DoubleClick += new System.EventHandler(this.PictureBox_DoubleClick);
            // 
            // Viewport
            // 
            this.Controls.Add(this.pictureBox);
            this.Name = "Viewport";
            this.Size = new System.Drawing.Size(216, 232);
            this.ResumeLayout(false);

        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the viewport's IViewportPainter.
        /// </summary>
        public IViewportPainter ViewportPainter
        {
            get { return _viewportPainter; }
            set 
            {
                _viewportPainter = value;
            }
        }

        /// <summary>
        /// Gets or sets the viewport's zoom factor.
        /// </summary>
        public float ZoomFactor
        {
            get { return _zoomFactor; }
            set
            {   
                _zoomFactor = value;
                RefreshImage();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh/repaint the image being displayed by the control.
        /// </summary>
        public void RefreshImage()
        {
            Graphics g = Graphics.FromImage(_image);
            g.FillRectangle(_brushBackground, 0, 0, _image.Width, _image.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // If a painter has been assigned then paint the graph.
            if(null != _viewportPainter) {
                _viewportPainter.Paint(g, _viewportArea, _zoomFactor);
                Refresh();
            }
        }

        #endregion

        #region Event Handlers

        private void PictureBox_SizeChanged(object sender, System.EventArgs e)
        {
            const float ImageSizeChangeDelta = 100f;

            // Track viewport area.
            int width = Width;
            int height = Height;
            _viewportArea.Size = new Size(width, height);

            // Handle calls during control initialization (image may not be created yet).
            if(null == _image) {
                return;
            }

            // If the viewport has grown beyond the size of the image then create a new image. 
            // Note. If the viewport shrinks we just paint on the existing (larger) image, this prevents unnecessary 
            // and expensive construction/destruction of Image objects.
            if(width > _image.Width || height > _image.Height) 
            {   // Reset the image's size. We round up the nearest __imageSizeChangeDelta. This prevents unnecessary 
                // and expensive construction/destruction of Image objects as the viewport is resized multiple times.
                int imageWidth = (int)(Math.Ceiling((float)width / ImageSizeChangeDelta) * ImageSizeChangeDelta);
                int imageHeight = (int)(Math.Ceiling((float)height / ImageSizeChangeDelta) * ImageSizeChangeDelta);
                _image = new Bitmap(imageWidth, imageHeight, ViewportPixelFormat);
                pictureBox.Image = _image;
            }

            RefreshImage();
        }

        private void PictureBox_DoubleClick(object sender, System.EventArgs e)
        {
            OnDoubleClick(e);
        }

        #endregion
    }
}
