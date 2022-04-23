// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;

namespace SharpNeat.Drawing;

/// <summary>
/// Represents types that paint to a viewport.
/// </summary>
public interface IViewportPainter
{
    /// <summary>
    /// Paints onto the specified viewport.
    /// </summary>
    /// <param name="g">The GDI+ drawing surface to paint on to.</param>
    /// <param name="viewportArea">The area to paint within.</param>
    /// <param name="zoomFactor">Zoom factor.</param>
    void Paint(Graphics g, Rectangle viewportArea, float zoomFactor);
}
