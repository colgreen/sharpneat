using System;
using SharpNeatLib.Maths;

namespace SharpNeatLib.Evolution
{
	public class ConnectionMutationParameterGroup
	{
		#region Public Fields

		/// <summary>
		/// This group's activation proportion - relative to the totalled 
		/// ActivationProportion for all groups.
		/// </summary>
		public double ActivationProportion;

		/// <summary>
		/// The type of mutation that this group represents.
		/// </summary>
		public ConnectionPerturbationType PerturbationType;

		/// <summary>
		/// The type of connection selection that this group represents.
		/// </summary>
		public ConnectionSelectionType SelectionType;

		/// <summary>
		/// Specifies the proportion for SelectionType.Proportional
		/// </summary>
		public double Proportion;

		/// <summary>
		/// Specifies the quantity for SelectionType.FixedQuantity
		/// </summary>
		public int Quantity;

		/// <summary>
		/// The perturbation factor for ConnectionPerturbationType.JiggleEven.
		/// </summary>
		public double PerturbationFactor;

		/// <summary>
		/// Sigma for for ConnectionPerturbationType.JiggleND.
		/// </summary>
		public double Sigma;

		#endregion
		
		#region Constructors

		public ConnectionMutationParameterGroup(	double activationProportion, 
													ConnectionPerturbationType perturbationType,
													ConnectionSelectionType selectionType,
													double proportion,
													int quantity,
													double perturbationFactor,
													double sigma)
		{
			ActivationProportion = activationProportion;
			PerturbationType = perturbationType;
			SelectionType = selectionType;
			Proportion = proportion;
			Quantity = quantity;
			PerturbationFactor = perturbationFactor;
			Sigma = sigma;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="copyFrom"></param>
		public ConnectionMutationParameterGroup(ConnectionMutationParameterGroup copyFrom)
		{
			ActivationProportion = copyFrom.ActivationProportion;
			PerturbationType = copyFrom.PerturbationType;
			SelectionType = copyFrom.SelectionType;
			Proportion = copyFrom.Proportion;
			Quantity = copyFrom.Quantity;
			PerturbationFactor = copyFrom.PerturbationFactor;
			Sigma = copyFrom.Sigma;		
		}

		#endregion

		#region Public Methods

//		public void Mutate(double pValueJiggle)
//		{
//			// Determine which parameter to mutate.
//			int possibleOutcomes=2;
//			if(PerturbationType!=ConnectionPerturbationType.Reset)
//				possibleOutcomes++;
//
//			int outcome = RouletteWheel.SingleThrowEven(possibleOutcomes);
//			bool resetOnly=(Utilities.NextDouble() < pValueJiggle);
//			
//			switch(outcome)
//			{
//				case 0: // ActivationProportion.
//				{
//					if(resetOnly)
//					{
//						ActivationProportion = Utilities.NextDouble();
//					}
//					else
//					{
//
//					}
//				}
//				case 1:	// In scope SelectionType parameter.
//				{
//					if(resetOnly)
//					{
//						switch(SelectionType)
//						{
//							case ConnectionSelectionType.FixedQuantity:
//								Quantity
//							case ConnectionSelectionType.Proportional:
//								Proportion
//
//						}
//					}
//					else
//					{
//
//					}
//				}
//				case 2:	// In scope PerturbationType parameter.
//				{
//					if(resetOnly)
//					{
//
//					}
//					else
//					{
//
//					}
//				}
//			}
//		}

		#endregion
	}
}
