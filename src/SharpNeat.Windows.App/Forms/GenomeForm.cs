// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for genome visualization.
/// This is used for displaying genome's directly (e.g. a neural net structure), or some other type of visualization
/// that uses the genome, e.g. a task view that shows how the genome performs on some task.
/// </summary>
public partial class GenomeForm : Form
{
    #region Constructor

    /// <summary>
    /// Construct with the provided form title, genome view/renderer and evolution algorithm. We listen to update events
    /// from the evolution algorithm and cleanly detach from it when this form closes.
    /// </summary>
    public GenomeForm(string title, GenomeControl genomeCtrl)
    {
        ArgumentNullException.ThrowIfNull(genomeCtrl);

        this.genomeCtrl = genomeCtrl;

        InitializeComponent();
        this.Text = title;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the genome to render.
    /// </summary>
    public IGenome Genome
    {
        get => this.genomeCtrl.Genome;
        set => this.genomeCtrl.Genome = value;
    }

    #endregion
}
