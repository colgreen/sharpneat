// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SharpNeat.Windows;

namespace SharpNeat.Tasks.Windows.PreyCapture;

#pragma warning disable SA1300 // Element should begin with upper-case letter.

public class PreyCaptureControl : GenomeControl
{
    const PixelFormat ViewportPixelFormat = PixelFormat.Format16bppRgb565;

    protected bool _initializing = true;
    PictureBox _pbx;
    Image _image;

    protected PictureBox Pbx => _pbx;

    protected Image Image
    {
        get => _image;
        set => _image = value;
    }

    /// <summary>
    /// Constructs a new instance of <see cref="PreyCaptureControl"/>.
    /// </summary>
    public PreyCaptureControl()
    {
        InitializeComponent();

        // Create a bitmap for the picturebox.
        int width = Width;
        int height = Height;
        _image = new Bitmap(width, height, ViewportPixelFormat);
        _pbx.Image = _image;
    }

    #region Private Methods [Windows.Forms Designer Code]

    private void InitializeComponent()
    {
        this._pbx = new System.Windows.Forms.PictureBox();
        ((System.ComponentModel.ISupportInitialize)(this._pbx)).BeginInit();
        this.SuspendLayout();

        // pbx
        this._pbx.Dock = System.Windows.Forms.DockStyle.Fill;
        this._pbx.Location = new System.Drawing.Point(0, 0);
        this._pbx.Name = "pbx";
        this._pbx.Size = new System.Drawing.Size(462, 216);
        this._pbx.TabIndex = 1;
        this._pbx.TabStop = false;
        this._pbx.SizeChanged += new System.EventHandler(this.pbx_SizeChanged);

        // PreyCaptureControl
        this.Controls.Add(this._pbx);
        this.Name = "PreyCaptureControl";
        ((System.ComponentModel.ISupportInitialize)(this._pbx)).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    private void pbx_SizeChanged(object sender, EventArgs e)
    {
        const float ImageSizeChangeDelta = 100f;

        if(_initializing)
            return;

        // Track viewport area.
        int width = Width;
        int height = Height;

        // If the viewport has grown beyond the size of the image then create a new image.
        // Note. If the viewport shrinks we just paint on the existing (larger) image, this prevents unnecessary
        // and expensive construction/destruction of Image objects.
        if(width > Image.Width || height > Image.Height)
        {
            // Reset the image's size. We round up the nearest ImageSizeChangeDelta. This prevents unnecessary
            // and expensive construction/destruction of Image objects as the viewport is resized multiple times.
            int imageWidth = (int)(Math.Ceiling(width / ImageSizeChangeDelta) * ImageSizeChangeDelta);
            int imageHeight = (int)(Math.Ceiling(height / ImageSizeChangeDelta) * ImageSizeChangeDelta);
            Image = new Bitmap(imageWidth, imageHeight, ViewportPixelFormat);
            Pbx.Image = Image;
        }

        // Repaint control.
        PaintView();
    }

    protected virtual void PaintView()
    {
    }

    protected override void Dispose(bool disposing)
    {
        if( disposing )
        {
            base.Dispose(disposing);
            _pbx.Dispose();
            _image.Dispose();
        }
    }
}
