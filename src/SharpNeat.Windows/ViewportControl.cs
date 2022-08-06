// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SharpNeat.Drawing;

namespace SharpNeat.Windows;

#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

/// <summary>
/// A user control that provides an area on which content can be drawn (by a <see cref="IViewportPainter"/>.
/// Otherwise the control is a simple one, providing no user interaction capability other than resizing.
/// </summary>
public class ViewportControl : UserControl
{
    const PixelFormat __viewportPixelFormat = PixelFormat.Format24bppRgb;

    Rectangle _viewportArea;
    float _zoomFactor = 1f;
    Image _image;

    // Windows.Forms variables.
    PictureBox pictureBox;
    bool _disposed;

    /// <summary>
    /// Default constructor. Required for user controls.
    /// </summary>
    public ViewportControl()
    {
        // This call is required by the Windows.Forms Form Designer.
        InitializeComponent();

        int width = Width;
        int height = Height;
        _viewportArea = new Rectangle(0, 0, width, height);

        // Create a bitmap for the picturebox.
        _image = new Bitmap(width, height, __viewportPixelFormat);
        pictureBox.Image = _image;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the viewport's IViewportPainter.
    /// </summary>
    public IViewportPainter ViewportPainter { get; set; }

    /// <summary>
    /// Gets or sets the viewport's zoom factor.
    /// </summary>
    public float ZoomFactor
    {
        get => _zoomFactor;
        set
        {
            _zoomFactor = value;
            Refresh();
        }
    }

    #endregion

    #region Component Designer Generated Code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
#pragma warning disable SA1120 // Comments should contain text

        this.pictureBox = new PictureBox();
        this.SuspendLayout();
        //
        // pictureBox1
        //
        this.pictureBox.Dock = DockStyle.Fill;
        this.pictureBox.Location = new Point(0, 0);
        this.pictureBox.Name = "pbx1";
        this.pictureBox.Size = new Size(100, 100);
        this.pictureBox.TabIndex = 0;
        this.pictureBox.TabStop = false;
        this.pictureBox.SizeChanged += new EventHandler(this.PictureBox_SizeChanged);
        this.pictureBox.DoubleClick += new EventHandler(this.PictureBox_DoubleClick);
        //
        // Viewport
        //
        this.Controls.Add(this.pictureBox);
        this.Name = "vpt1";
        this.Size = new Size(100, 100);
        this.ResumeLayout(false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Repaint the viewport.
    /// </summary>
    public void RepaintViewport()
    {
        // If a painter has been assigned, then paint the graph.
        if(ViewportPainter is not null)
        {
            Graphics g = Graphics.FromImage(_image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.AssumeLinear;
            ViewportPainter.Paint(g, _viewportArea, _zoomFactor);
        }
    }

    #endregion

    #region Event Handlers

    private void PictureBox_SizeChanged(object sender, EventArgs e)
    {
        const float ImageSizeChangeDelta = 100f;

        // Track viewport area.
        int width = Width;
        int height = Height;
        _viewportArea.Size = new Size(width, height);

        // Handle calls during control initialization (image may not be created yet).
        if(_image is null)
            return;

        // If the viewport has grown beyond the size of the image, then create a new image.
        // Note. If the viewport shrinks we just paint on the existing (larger) image, this prevents unnecessary
        // and expensive construction/destruction of Image objects.
        if(width > _image.Width || height > _image.Height)
        {
            // Reset the image's size. We round up the nearest __imageSizeChangeDelta. This prevents unnecessary
            // and expensive construction/destruction of Image objects as the viewport is resized multiple times.
            int imageWidth = (int)(MathF.Ceiling(width / ImageSizeChangeDelta) * ImageSizeChangeDelta);
            int imageHeight = (int)(MathF.Ceiling(height / ImageSizeChangeDelta) * ImageSizeChangeDelta);
            _image = new Bitmap(imageWidth, imageHeight, __viewportPixelFormat);
            pictureBox.Image = _image;
        }

        // Repaint the viewport.
        RepaintViewport();

        // Update the control/window to show the updated/repainted viewport.
        Refresh();
    }

    private void PictureBox_DoubleClick(object sender, EventArgs e)
    {
        // Propagate the double click event up to the parent control.
        OnDoubleClick(e);
    }

    #endregion

    #region Disposal

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if(_disposed)
            return;

        if(disposing)
            pictureBox.Dispose();

        base.Dispose(disposing);

        _disposed = true;
    }

    #endregion
}
