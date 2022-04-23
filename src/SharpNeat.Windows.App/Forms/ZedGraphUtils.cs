// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

internal static class ZedGraphUtils
{
    /// <summary>
    /// Apply a consistent style to the provided ZedGraph line item.
    /// </summary>
    /// <param name="lineItem">The line item.</param>
    public static void ApplyLineStyle(LineItem lineItem)
    {
        lineItem.Line.IsAntiAlias = true;
        lineItem.Line.Width = 2f;
    }
}
