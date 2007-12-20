using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using SharpNeatLib.Evolution;
using ZedGraph;

namespace SharpNeat
{
	public partial class ProgressForm : Form
	{
		/// <summary>
		/// Replacement for original RudyGraph-based progress display. Changes:
		/// * Now uses ZedGraph (personal whim)
		/// * Form uses Splitter controls to make sizing a little more flexible
		/// * Complexity, species and compat. threshold get their own graphs
		/// * Added graph of evals per second
		/// 
		/// Areas for consideration
		/// * More graphs! Possibilities:
		///		* Steps to converge (for multi-step "relaxing" networks)
		///		* Species size stacked chart
		///		* cool stuff as in NEVT
		/// * More configurability (for practice in form development within C# in, if nothing else)
		/// 
		/// To-Dos
		/// * Tidy up: there's a fair bit of repeated code that could stand to be refactored. The
		///		line graphs and their logic are all very similar and could be folded into something
		///		that would thin down this code considerably. A Friday afternoon job.
		/// </summary>

        // The max number of points to show on screen befor ethe graph starts rolling old 
        // values of to the left. 900 gives us 15 mins of history witha 1 sec update rate.
        const int HISTORY_LENGTH = 900;

		GraphPane fitnessPane;
		GraphPane complexityPane;
		GraphPane speciesPane;
		GraphPane compatPane;
		GraphPane evalsPane;

        RollingPointPairList bestFitness = new RollingPointPairList(HISTORY_LENGTH);
        RollingPointPairList meanFitness = new RollingPointPairList(HISTORY_LENGTH);
        RollingPointPairList complexity = new RollingPointPairList(HISTORY_LENGTH);
        RollingPointPairList species = new RollingPointPairList(HISTORY_LENGTH);
        RollingPointPairList compatThreshold = new RollingPointPairList(HISTORY_LENGTH);
        RollingPointPairList evals = new RollingPointPairList(HISTORY_LENGTH);

		ulong lastUpdateEvaluationCount;
		long lastUpdateDateTimeTick;

        #region Constructor

        public ProgressForm()
		{
			InitializeComponent();

			fitnessPane = fitnessGraph.GraphPane;
			fitnessPane.Title = "Fitness";
			fitnessPane.XAxis.Title = "Generation";
			fitnessPane.YAxis.Title = "Score";
			fitnessPane.XAxis.IsShowGrid = true;
            fitnessPane.YAxis.IsShowGrid = true;
            fitnessPane.XAxis.Scale.MinAuto = false;
            fitnessPane.XAxis.Scale.MaxAuto = false;
			LineItem bestFitnessCurve = fitnessPane.AddCurve("Best", bestFitness, Color.Red, SymbolType.None);
			LineItem meanFitnessCurve = fitnessPane.AddCurve("Mean", meanFitness, Color.Teal, SymbolType.None);

			complexityPane = complexityGraph.GraphPane;
			complexityPane.Title = "Complexity";
			complexityPane.XAxis.Title = "Generation";
			complexityPane.YAxis.Title = "Value";
			complexityPane.XAxis.IsShowGrid = true;
			complexityPane.YAxis.IsShowGrid = true;
            complexityPane.XAxis.Scale.MinAuto = false;
            complexityPane.XAxis.Scale.MaxAuto = false;
			LineItem complexityCurve = complexityPane.AddCurve("", complexity, Color.Green, SymbolType.None);

			speciesPane = speciesGraph.GraphPane;
			speciesPane.Title = "Species";
			speciesPane.XAxis.Title = "Generation";
			speciesPane.YAxis.Title = "Species";
			speciesPane.YAxis.IsShowGrid = true;
			speciesPane.XAxis.IsShowGrid = true;
            speciesPane.XAxis.Scale.MinAuto = false;
            speciesPane.XAxis.Scale.MaxAuto = false;
			LineItem speciesCurve = speciesPane.AddCurve("# species", species, Color.Red, SymbolType.None);

			compatPane = compatGraph.GraphPane;
			compatPane.Title = "Compatibility Threshold";
			compatPane.XAxis.Title = "Generation";
			compatPane.YAxis.Title = "Threshold Value";
			compatPane.YAxis.IsShowGrid = true;
			compatPane.XAxis.IsShowGrid = true;
            compatPane.XAxis.Scale.MinAuto = false;
            compatPane.XAxis.Scale.MaxAuto = false;
			LineItem compatCurve = compatPane.AddCurve("Threshold", compatThreshold, Color.Green, SymbolType.None);

			evalsPane = evalsPerSecGraph.GraphPane;
			evalsPane.Title = "Evals Per Second";
			evalsPane.XAxis.Title = "Generation";
			evalsPane.YAxis.Title = "Evals";
			evalsPane.YAxis.IsShowGrid = true;
			evalsPane.XAxis.IsShowGrid = true;
            evalsPane.XAxis.Scale.MinAuto = false;
            evalsPane.XAxis.Scale.MaxAuto = false;
			LineItem evalsCurve = evalsPane.AddCurve("Evals", evals, Color.Black, SymbolType.None);
        }

        #endregion

        #region Public Methods

        public void Update(EvolutionAlgorithm ea)
		{
			Population p = ea.Population;

			bestFitness.Enqueue(ea.Generation, ea.BestGenome.Fitness);
            meanFitness.Enqueue(ea.Generation, p.MeanFitness);
            complexity.Enqueue(ea.Generation, p.AvgComplexity);
            species.Enqueue(ea.Generation, p.SpeciesTable.Count);
            compatThreshold.Enqueue(ea.Generation, ea.NeatParameters.compatibilityThreshold);

			long ticksNow = DateTime.Now.Ticks;
			long tickDelta = ticksNow - lastUpdateDateTimeTick;
			lastUpdateDateTimeTick = ticksNow;
			double evaluationsDelta = ea.PopulationEvaluator.EvaluationCount - lastUpdateEvaluationCount;
			lastUpdateEvaluationCount = ea.PopulationEvaluator.EvaluationCount;
			double evaluationsPerSec = evaluationsDelta / (double)tickDelta * 10000000.0;
            evals.Enqueue(ea.Generation, evaluationsPerSec);

            // Update X-Axis scale/ranges.
            fitnessPane.XAxis.Scale.Min = bestFitness[0].X;
            fitnessPane.XAxis.Scale.Max = bestFitness[bestFitness.Count-1].X;

            complexityPane.XAxis.Scale.Min = complexity[0].X;
            complexityPane.XAxis.Scale.Max = complexity[complexity.Count-1].X;

            speciesPane.XAxis.Scale.Min = species[0].X;
            speciesPane.XAxis.Scale.Max = species[species.Count-1].X;

            compatPane.XAxis.Scale.Min = compatThreshold[0].X;
            compatPane.XAxis.Scale.Max = compatThreshold[compatThreshold.Count-1].X;

            evalsPane.XAxis.Scale.Min = evals[0].X;
            evalsPane.XAxis.Scale.Max = evals[evals.Count-1].X;

			UpdateDisplay();
		}

        public void Reset()
        {
            bestFitness.Clear();
            meanFitness.Clear();
            complexity.Clear();
            species.Clear();
            compatThreshold.Clear();
            evals.Clear();
            UpdateDisplay();
        }

		private void UpdateDisplay()
		{
			fitnessGraph.AxisChange();
			complexityGraph.AxisChange();
			speciesGraph.AxisChange();
			compatGraph.AxisChange();
			evalsPerSecGraph.AxisChange();
            Refresh();
        }

        #endregion

        #region Private Methdos

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			this.Visible = false;
        }

        #endregion
    }
}