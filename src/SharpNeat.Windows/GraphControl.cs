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
using SharpNeat.Drawing;

namespace SharpNeat.Windows
{
    /// <summary>
    /// A user control for visualization of node graphs.
    /// </summary>
    public class GraphControl : UserControl
    {
        readonly Viewport _viewport;

        #region Component Designer variables

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        #endregion

        #region Constructor / Disposal

        /// <summary>
        /// Default constructor. Required for user controls.
        /// </summary>
        public GraphControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Create and attach the viewport.
            _viewport = new Viewport
            {
                // By default we match the size of the containing control.
                Size = new Size(Width,Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            _viewport.DoubleClick += Viewport_DoubleClick;
            this.Controls.Add(_viewport);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GraphControl
            // 
            this.Name = "GraphControl";
            this.Size = new Size(352, 336);
            this.Resize += new System.EventHandler(this.GraphControl_Resize);
            this.ResumeLayout(false);

        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the viewport painter. Viewport painters can be switched and therefore allow 
        /// this control to display whatever the provided viewport painter paints.
        /// </summary>
        public IViewportPainter ViewportPainter
        {
            get 
            { 
                if(_viewport is null || _viewport.ViewportPainter is null) {
                    return null;
                }
                return _viewport.ViewportPainter;
            }
            set
            {
                if(_viewport is object) {
                    _viewport.ViewportPainter = value;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void GraphControl_Resize(object sender, System.EventArgs e)
        {
            UpdateViewportSize();
        }

        private void Viewport_DoubleClick(object sender, System.EventArgs e)
        {
            OnDoubleClick(e);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh the image of the graph being displayed by the control.
        /// </summary>
        public void RefreshImage()
        {
            _viewport.RefreshImage();
        }

        #endregion

        #region Private Methods

        private void UpdateViewportSize()
        {
            if(_viewport is object) {
                _viewport.Size = this.Size;
            }
        }

        #endregion
    }
}
