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
using System.Windows.Forms;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Windows;

/// <summary>
/// A user control for genome visualization.
/// This is used for displaying a genome's directly (e.g. a neural net structure), or some other type of visualization
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
