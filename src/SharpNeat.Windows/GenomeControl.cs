// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Windows;

/// <summary>
/// A user control for genome visualization.
/// This is used for displaying a genome directly (e.g. a neural net structure), or some other type of visualization
/// that uses the genome, e.g. a task view that shows how the genome performs on some task.
/// </summary>
public partial class GenomeControl : UserControl
{
    /// <summary>
    /// The genome being displayed by the control.
    /// </summary>
    protected IGenome _genome;

    /// <summary>
    /// Constructs a new instance of <see cref="GenomeControl"/>.
    /// </summary>
    public GenomeControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets of sets the genome object to display.
    /// </summary>
    public IGenome Genome
    {
        get => _genome;
        set
        {
            _genome = value;
            OnGenomeUpdated();
        }
    }

    /// <summary>
    /// Optional code to run when the genome is updated.
    /// </summary>
    public virtual void OnGenomeUpdated()
    {
    }
}
