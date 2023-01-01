// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;

namespace SharpNeat.Drawing.Graph.Painting;

/// <summary>
/// Graph painter settings.
/// </summary>
public sealed class PainterSettings
{
    /// <summary>
    /// Positive connection weight color.
    /// </summary>
    public Color PositiveWeightColor { get; } = Color.Red;

    /// <summary>
    /// Negative connection weight color.
    /// </summary>
    public Color NegativeWeightColor { get; } = Color.FromArgb(0x3e, 0x3e, 0xc2);

    /// <summary>
    /// Node diameter.
    /// </summary>
    public float NodeDiameter { get; } = 10f;

    /// <summary>
    /// Node border color.
    /// </summary>
    public Color NodeBorderColor { get; } = Color.Black;

    /// <summary>
    /// Node fill color.
    /// </summary>
    public Color NodeFillColor { get; } = Color.GhostWhite;

    /// <summary>
    /// Node label color.
    /// </summary>
    public Color NodeLabelColor { get; } = Color.Black;

    /// <summary>
    /// Font for drawing node ID text on the viewport.
    /// </summary>
    public Font NodeLabelFont { get; } = new Font("Microsoft Sans Serif", 7.0F);

    /// <summary>
    /// Expected/nominal range of connection weights. Used to determine width of drawn connections.
    /// </summary>
    public float ConnectionWeightRange { get; } = 5f;
}
