// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live graph of data taken from the standard statistics objects.
/// </summary>
public class StatsGraphForm : GraphForm
{
    /// <summary>
    /// Construct with the given titles.
    /// </summary>
    /// <param name="title">Graph title.</param>
    /// <param name="xAxisTitle">X-axis title.</param>
    /// <param name="y1AxisTitle">Y-axis title.</param>
    /// <param name="y2AxisTitle">Y2-axis title (optional).</param>
    public StatsGraphForm(
        string title,
        string xAxisTitle,
        string y1AxisTitle,
        string y2AxisTitle)
        : base(title, xAxisTitle, y1AxisTitle, y2AxisTitle)
    {}

    /// <summary>
    /// Update the graph data.
    /// </summary>
    /// <param name="eaStats">Evolution algorithm statistics object.</param>
    /// <param name="popStats">Population statistics object.</param>
    public virtual void UpdateData(
        EvolutionAlgorithmStatistics eaStats,
        PopulationStatistics popStats)
    {
        // Note. This method could be defined as abstract, but that would prevent the Window Forms UI designer from working;
        // therefore instead it is defined as virtual with no implementation.
    }
}
