using System;

using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.Maths;

namespace NeatParameterOptimizer
{
	/// <summary>
	/// A class to wrap a single instance of a NeatParameters class, extending it with genetic operators.
	/// </summary>
	public class NeatParametersWrapper
	{
		NeatParameters neatParameters;

		double fitness = 0;

		// default to 0 so that new offspring get priority over older npw's with
		// equal fitness.
		int orderRandomizer=0;
	
		#region Constructor

		public NeatParametersWrapper(NeatParameters neatParameters)
		{
			this.neatParameters = neatParameters;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="oCopyFrom"></param>
		public NeatParametersWrapper(NeatParametersWrapper oCopyFrom)
		{
			this.neatParameters = new NeatParameters(oCopyFrom.neatParameters);
		}

		#endregion

		#region Properties

		public NeatParameters NeatParameters
		{
			get
			{
				return neatParameters;
			}
		}

		public double Fitness
		{
			get
			{
				return fitness;
			}
			set
			{
				fitness = value;
			}
		}

		public int OrderRandomizer
		{
			get
			{
				return orderRandomizer;
			}
			set
			{
				orderRandomizer = value;
			}
		}

		#endregion

		#region Public Methods

		public NeatParametersWrapper CreateOffspring_Asexual(MetaEvolutionAlgorithm ea)
		{
			// Make an exact copy of this object.
			NeatParametersWrapper oNewWrapper = new NeatParametersWrapper(this);

			// Mutate the new object.
			oNewWrapper.Mutate(ea.MetaNeatParameters, false);
			return oNewWrapper;
		}

		public void Mutate(MetaNeatParameters mnp, bool resetOnly)
		{
			// Pick a parameter at random from the set of mutable parameters.
			ParameterFlag parameterFlag = (ParameterFlag)mnp.MutableParameterFlagList[(int)(Utilities.NextDouble() * mnp.MutableParameterCount)];
			Mutate(mnp, resetOnly, parameterFlag);
		}

		/// <summary>
		/// Randomize this NeatParametersWrapper based on the parameters specified by mnp. 
		/// This is primarily useful for generating an initial population of NeatParametersWrapper
		/// objects for the meta-GA.
		/// </summary>
		/// <param name="mnp"></param>
		/// <returns></returns>
		public void Mutate_ResetParameters(MetaNeatParameters mnp)
		{
			foreach(ParameterFlag parameterFlag in mnp.MutableParameterFlagList)
				Mutate(mnp, true, parameterFlag);
		}

		#endregion

		#region Private Methods [Mutation]

		private void Mutate(MetaNeatParameters mnp, bool resetOnly, ParameterFlag parameterFlag)
		{
			switch(parameterFlag)
			{
				case ParameterFlag.populationSize:
				{
					Mutate_populationSize(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pInitialPopulationInterconnections:
				{
					Mutate_pInitialPopulationInterconnections(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pOffspringAsexual_Sexual:
				{
					Mutate_pOffspringAsexual_Sexual(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pInterspeciesMating:
				{
					Mutate_pInterspeciesMating(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pDisjointExcessGenesRecombined:
				{
					Mutate_pDisjointExcessGenesRecombined(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pMutateType:
				{
					Mutate_pMutateType(mnp, resetOnly);
					break;
				}
				case ParameterFlag.ConnectionMutationParameterGroupList:
				{
					Mutate_ConnectionMutationParameterGroupList(mnp, resetOnly);
					break;
				}
				case ParameterFlag.compatibilityThreshold:
				{
					Mutate_compatibilityThreshold(mnp, resetOnly);
					break;
				}
				case ParameterFlag.compatibilityDisjointCoeff:
				{
					Mutate_compatibilityDisjointCoeff(mnp, resetOnly);
					break;
				}
				case ParameterFlag.compatibilityExcessCoeff:
				{
					Mutate_compatibilityExcessCoeff(mnp, resetOnly);
					break;
				}
				case ParameterFlag.compatibilityWeightDeltaCoeff:
				{
					Mutate_compatibilityExcessCoeff(mnp, resetOnly);
					break;
				}
				case ParameterFlag.elitismProportion:
				{
					Mutate_elitismProportion(mnp, resetOnly);
					break;
				}
				case ParameterFlag.selectionProportion:	
				{
					Mutate_selectionProportion(mnp, resetOnly);
					break;
				}
				case ParameterFlag.targetSpeciesCountWindow:
				{
					Mutate_targetSpeciesCountWindow(mnp, resetOnly);
					break;
				}
				case ParameterFlag.speciesDropoffAge:
				{
					Mutate_speciesDropoffAge(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pruningPhaseBeginComplexityThreshold:
				{
					Mutate_pruningPhaseBeginComplexityThreshold(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pruningPhaseBeginFitnessStagnationThreshold:
				{
					Mutate_pruningPhaseBeginFitnessStagnationThreshold(mnp, resetOnly);
					break;
				}
				case ParameterFlag.pruningPhaseEndComplexityStagnationThreshold:
				{
					Mutate_pruningPhaseEndComplexityStagnationThreshold(mnp, resetOnly);
					break;
				}
				case ParameterFlag.connectionWeightRange:
				{
					Mutate_connectionWeightRange(mnp, resetOnly);
					break;
				}
			}
		}

		private void Mutate_populationSize(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.populationSize += (int)(Utilities.NextDouble()*20.0-10.0);
				neatParameters.populationSize = Math.Max(mnp.populationSize_min, neatParameters.populationSize);
				neatParameters.populationSize = Math.Min(mnp.populationSize_max, neatParameters.populationSize);
			}
			else
			{
				double range = mnp.populationSize_max - mnp.populationSize_min;
				neatParameters.populationSize = mnp.populationSize_min + (int)(Utilities.NextDouble()*range);
			}
		}

		private void Mutate_pInitialPopulationInterconnections(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pInitialPopulationInterconnections += (float)(Utilities.NextDouble()*0.2-0.1);

				neatParameters.pInitialPopulationInterconnections = (float)Math.Max(0.0, neatParameters.pInitialPopulationInterconnections);
				neatParameters.pInitialPopulationInterconnections = (float)Math.Min(1.0, neatParameters.pInitialPopulationInterconnections);
			}
			else
			{
				neatParameters.pInitialPopulationInterconnections = (float)Utilities.NextDouble();
			}
		}

		private void Mutate_pOffspringAsexual_Sexual(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pOffspringAsexual += (Utilities.NextDouble()*0.2-0.1);

				neatParameters.pOffspringAsexual = Math.Max(0.0, neatParameters.pOffspringAsexual);
				neatParameters.pOffspringAsexual = Math.Min(1.0, neatParameters.pOffspringAsexual);
				neatParameters.pOffspringSexual = 1.0 - neatParameters.pOffspringAsexual;
			}
			else
			{
				neatParameters.pOffspringAsexual = Utilities.NextDouble();
				neatParameters.pOffspringSexual = 1.0 - neatParameters.pOffspringAsexual;
			}
		}

		private void Mutate_pInterspeciesMating(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pInterspeciesMating += (Utilities.NextDouble()*0.2-0.1);

				neatParameters.pInterspeciesMating = Math.Max(0.0, neatParameters.pInterspeciesMating);
				neatParameters.pInterspeciesMating = Math.Min(1.0, neatParameters.pInterspeciesMating);
			}
			else
			{
				neatParameters.pInterspeciesMating = Utilities.NextDouble();
			}
		}

		private void Mutate_pDisjointExcessGenesRecombined(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pDisjointExcessGenesRecombined += (Utilities.NextDouble()*0.2-0.1);

				neatParameters.pDisjointExcessGenesRecombined = Math.Max(0.0, neatParameters.pDisjointExcessGenesRecombined);
				neatParameters.pDisjointExcessGenesRecombined = Math.Min(1.0, neatParameters.pDisjointExcessGenesRecombined);
			}
			else
			{
				neatParameters.pDisjointExcessGenesRecombined = Utilities.NextDouble();
			}
		}

		private void Mutate_pMutateType(MetaNeatParameters mnp, bool resetOnly)
		{
			double total=0;
//			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
//			{
				// Select one of the five parameters at random, jiggle it and then re-normalize
				// the five values to ensure they add up to 1.0
				switch(RouletteWheel.SingleThrowEven(5))
				{
					case 0:
						neatParameters.pMutateConnectionWeights += (Utilities.NextDouble()*0.02-0.01);
						neatParameters.pMutateConnectionWeights = Utilities.LimitRange(neatParameters.pMutateConnectionWeights, 0.0, 1.0);
						break;
					case 1:
						neatParameters.pMutateAddNode += (Utilities.NextDouble()*0.02-0.01);
						neatParameters.pMutateAddNode = Utilities.LimitRange(neatParameters.pMutateAddNode, 0.0, 1.0);
						break;
					case 2:
						neatParameters.pMutateAddConnection += (Utilities.NextDouble()*0.02-0.01);
						neatParameters.pMutateAddConnection = Utilities.LimitRange(neatParameters.pMutateAddConnection, 0.0, 1.0);
						break;
					case 3:
						neatParameters.pMutateDeleteConnection += (Utilities.NextDouble()*0.02-0.01);
						neatParameters.pMutateDeleteConnection = Utilities.LimitRange(neatParameters.pMutateDeleteConnection, 0.0, 1.0);
						break;
					case 4:
						neatParameters.pMutateDeleteSimpleNeuron += (Utilities.NextDouble()*0.02-0.01);
						neatParameters.pMutateDeleteSimpleNeuron = Utilities.LimitRange(neatParameters.pMutateDeleteSimpleNeuron, 0.0, 1.0);
						break;
				}
//			}
//			else
//			{
//				// Reset each of the five values to between 0 and 1 and then normalize them so that
//				// they add up to 1.0. 
//				neatParameters.pMutateConnectionWeights = Utilities.NextDouble();
//				neatParameters.pMutateAddNode = Utilities.NextDouble();
//				neatParameters.pMutateAddConnection = Utilities.NextDouble();
//				neatParameters.pMutateDeleteConnection = Utilities.NextDouble();
//				neatParameters.pMutateDeleteSimpleNeuron = Utilities.NextDouble();
//			}

			total = neatParameters.pMutateConnectionWeights;
			total += neatParameters.pMutateAddNode;
			total += neatParameters.pMutateAddConnection;
			total += neatParameters.pMutateDeleteConnection;
			total += neatParameters.pMutateDeleteSimpleNeuron;

			// Normalize.
			neatParameters.pMutateConnectionWeights /= total;
			neatParameters.pMutateAddNode /= total;
			neatParameters.pMutateAddConnection /= total;
			neatParameters.pMutateDeleteConnection /= total;
			neatParameters.pMutateDeleteSimpleNeuron /= total;
		}


		private void Mutate_ConnectionMutationParameterGroupList(MetaNeatParameters mnp, bool resetOnly)
		{
			// Two types of mutation can be applied here, either optimize the current set of
			// ConnectionMutationParameterGroups or give free reign to the meta algorithm to evolve a set of groups.

		//----- Type 1 - Optimize the existing set.
			// Select a group at random and mutate it.
			int outcome = RouletteWheel.SingleThrowEven(neatParameters.ConnectionMutationParameterGroupList.Count);
			ConnectionMutationParameterGroup mutationParameterGroup = (ConnectionMutationParameterGroup)neatParameters.ConnectionMutationParameterGroupList[outcome];
			Mutate_ConnectionMutationParameterGroup(mnp, resetOnly, mutationParameterGroup);
	
			// Ensure the ActivationProportions are normalized.
			double total=0;
			foreach(ConnectionMutationParameterGroup paramGroup in neatParameters.ConnectionMutationParameterGroupList)
				total += paramGroup.ActivationProportion;

			foreach(ConnectionMutationParameterGroup paramGroup in neatParameters.ConnectionMutationParameterGroupList)
				paramGroup.ActivationProportion /= total;
		}


		private void Mutate_ConnectionMutationParameterGroup(MetaNeatParameters mnp, bool resetOnly, ConnectionMutationParameterGroup paramGroup)
		{
			// Determine which parameter to mutate.
			int possibleOutcomes=2;
			if(paramGroup.PerturbationType!=ConnectionPerturbationType.Reset)
				possibleOutcomes++;

			int outcome = RouletteWheel.SingleThrowEven(possibleOutcomes);
			switch(outcome)
			{
				case 0: // ActivationProportion.
				{
					if(resetOnly)
					{
						paramGroup.ActivationProportion = Utilities.NextDouble();
					}
					else
					{
						paramGroup.ActivationProportion = (Utilities.NextDouble()*0.2-0.1);
						// No need to enforce limits - the value should be normalized by the calling routine.
					}
					break;
				}
				case 1:	// In scope SelectionType parameter.
				{
					if(resetOnly)
					{
						switch(paramGroup.SelectionType)
						{
							case ConnectionSelectionType.FixedQuantity:
							{
								double range = mnp.connectionMutationFixedQuantity_max - mnp.connectionMutationFixedQuantity_min;
								paramGroup.Quantity = mnp.connectionMutationFixedQuantity_min + (int)(range * Utilities.NextDouble());
								break;
							}
							case ConnectionSelectionType.Proportional:
							{
								paramGroup.Proportion = Utilities.NextDouble();
								break;
							}
						}
					}
					else
					{
						switch(paramGroup.SelectionType)
						{
							case ConnectionSelectionType.FixedQuantity:
							{
								double range = (mnp.connectionMutationFixedQuantity_max - mnp.connectionMutationFixedQuantity_min)/10;
								paramGroup.Quantity += (int)(range*Utilities.NextDouble()-(range/2.0));

								// Enforce limits.
								paramGroup.Quantity = Math.Max(mnp.connectionMutationFixedQuantity_min, paramGroup.Quantity);
								paramGroup.Quantity = Math.Min(mnp.connectionMutationFixedQuantity_max, paramGroup.Quantity);
								break;
							}
							case ConnectionSelectionType.Proportional:
							{
								paramGroup.Proportion += (Utilities.NextDouble()*0.2-0.1);

								// Enforce limits.
								paramGroup.Proportion = Math.Max(0.0, paramGroup.Proportion);
								paramGroup.Proportion = Math.Min(1.0, paramGroup.Proportion);
								break;
							}
						}
					}
					break;
				}
				case 2:	// In scope PerturbationType parameter.
				{
					if(resetOnly)
					{
						switch(paramGroup.PerturbationType)
						{
							case ConnectionPerturbationType.JiggleEven:
								paramGroup.PerturbationFactor = Utilities.NextDouble() * 0.2;
								break;
							case ConnectionPerturbationType.JiggleND:
								paramGroup.Sigma = Utilities.NextDouble() * 0.2;
								break;
						}
					}
					else
					{
						switch(paramGroup.PerturbationType)
						{
							case ConnectionPerturbationType.JiggleEven:
							{
								paramGroup.PerturbationFactor += (Utilities.NextDouble()*0.1-0.05);
								// Enforce limits.
								paramGroup.PerturbationFactor = Math.Max(0.0, paramGroup.PerturbationFactor);
								paramGroup.PerturbationFactor = Math.Min(0.2, paramGroup.PerturbationFactor);
								break;
							}
							case ConnectionPerturbationType.JiggleND:
							{
								paramGroup.Sigma += (Utilities.NextDouble()*0.1-0.05);
								// Enforce limits.
								paramGroup.Sigma = Math.Max(0.0, paramGroup.Sigma);
								paramGroup.Sigma = Math.Min(0.2, paramGroup.Sigma);
								break;
							}
						}
					}
					break;
				}
			}
		}
































		private void Mutate_compatibilityThreshold(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.compatibilityThreshold += (Utilities.NextDouble()*2.0-1.0);
				neatParameters.compatibilityThreshold = Math.Max(0.0, neatParameters.compatibilityThreshold);
			}
			else
			{
				neatParameters.compatibilityThreshold = Utilities.NextDouble()*30;
			}	
		}


		private void Mutate_compatibilityDisjointCoeff(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.compatibilityDisjointCoeff += (Utilities.NextDouble()*0.5-0.25);
				neatParameters.compatibilityDisjointCoeff = Math.Max(0.0, neatParameters.compatibilityDisjointCoeff);
			}
			else
			{
				neatParameters.compatibilityDisjointCoeff = Utilities.NextDouble()*5;
			}
		}

		private void Mutate_compatibilityExcessCoeff(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.compatibilityExcessCoeff += (Utilities.NextDouble()*0.5-0.25);
				neatParameters.compatibilityExcessCoeff = Math.Max(0.0, neatParameters.compatibilityExcessCoeff);
			}
			else
			{
				neatParameters.compatibilityExcessCoeff = Utilities.NextDouble()*5.0;
			}
		}

		private void Mutate_elitismProportion(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.elitismProportion += (Utilities.NextDouble()*0.05-0.025);
				neatParameters.elitismProportion = Math.Max(0.0, neatParameters.elitismProportion);
				neatParameters.elitismProportion = Math.Min(0.9, neatParameters.elitismProportion);
			}
			else
			{
				neatParameters.elitismProportion = Utilities.NextDouble()*0.9;
			}
		}

		private void Mutate_selectionProportion(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.selectionProportion += (Utilities.NextDouble()*0.05-0.025);
				neatParameters.selectionProportion = Math.Max(0.0, neatParameters.selectionProportion);
				neatParameters.selectionProportion = Math.Min(0.9, neatParameters.selectionProportion);
			}
			else
			{
				neatParameters.selectionProportion = Utilities.NextDouble()*0.9;
			}
		}

		private void Mutate_targetSpeciesCountWindow(MetaNeatParameters mnp, bool resetOnly)
		{
			int range = neatParameters.targetSpeciesCountMax - neatParameters.targetSpeciesCountMin;

			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				if(Utilities.NextDouble() < 0.5)
				{
					neatParameters.targetSpeciesCountMin += (int)(Utilities.NextDouble()*4.0-2.0);
					neatParameters.targetSpeciesCountMin = Math.Max(1, neatParameters.targetSpeciesCountMin);
				}
				else
				{
					range += (int)(Utilities.NextDouble()*4-2);
					range = Math.Max(0, range);
				}
				neatParameters.targetSpeciesCountMax = neatParameters.targetSpeciesCountMin + range;
			}
			else
			{
				if(Utilities.NextDouble() < 0.5)
				{
					neatParameters.targetSpeciesCountMin = 1+(int)(Utilities.NextDouble()*10.0);
				}
				else
				{
					range = (int)(Utilities.NextDouble()*30.0);
				}
				neatParameters.targetSpeciesCountMax = neatParameters.targetSpeciesCountMin + range;
			}
		}

		private void Mutate_speciesDropoffAge(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.speciesDropoffAge += (int)(Utilities.NextDouble()*50.0-25.0);
				neatParameters.speciesDropoffAge = Math.Max(0, neatParameters.speciesDropoffAge);
				neatParameters.speciesDropoffAge = Math.Min(10000, neatParameters.speciesDropoffAge);
			}
			else
			{
				neatParameters.speciesDropoffAge = (int)(Utilities.NextDouble()*10000.0);
			}
		}

		private void Mutate_pruningPhaseBeginComplexityThreshold(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pruningPhaseBeginComplexityThreshold += (float)(Utilities.NextDouble()*4.0-2.0);
				neatParameters.pruningPhaseBeginComplexityThreshold = Math.Max(10F, neatParameters.pruningPhaseBeginComplexityThreshold);
				neatParameters.pruningPhaseBeginComplexityThreshold = Math.Min(500F, neatParameters.pruningPhaseBeginComplexityThreshold);
			}
			else
			{
				neatParameters.pruningPhaseBeginComplexityThreshold = 10F+(float)(Utilities.NextDouble()*490.0);
			}
		}

		private void Mutate_pruningPhaseBeginFitnessStagnationThreshold(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pruningPhaseBeginComplexityThreshold += (float)(Utilities.NextDouble()*4.0-2.0);
				neatParameters.pruningPhaseBeginComplexityThreshold = Math.Max(10F, neatParameters.pruningPhaseBeginComplexityThreshold);
				neatParameters.pruningPhaseBeginComplexityThreshold = Math.Min(500F, neatParameters.pruningPhaseBeginComplexityThreshold);
			}
			else
			{
				neatParameters.pruningPhaseBeginComplexityThreshold = 10F+(float)(Utilities.NextDouble()*490.0);
			}
		}

		private void Mutate_pruningPhaseEndComplexityStagnationThreshold(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.pruningPhaseBeginComplexityThreshold += (float)(Utilities.NextDouble()*4.0-2.0);
				neatParameters.pruningPhaseBeginComplexityThreshold = Math.Max(10F, neatParameters.pruningPhaseBeginComplexityThreshold);
				neatParameters.pruningPhaseBeginComplexityThreshold = Math.Min(50F, neatParameters.pruningPhaseBeginComplexityThreshold);
			}
			else
			{
				neatParameters.pruningPhaseBeginComplexityThreshold = 10F+(float)(Utilities.NextDouble()*40.0);
			}
		}

		private void Mutate_connectionWeightRange(MetaNeatParameters mnp, bool resetOnly)
		{
			if(!resetOnly && (Utilities.NextDouble() < mnp.pValueJiggle))
			{
				neatParameters.connectionWeightRange += (double)(Utilities.NextDouble()*0.5-0.25);
				neatParameters.connectionWeightRange = Math.Max(mnp.connectionWeightRange_min, neatParameters.connectionWeightRange);
				neatParameters.connectionWeightRange = Math.Min(mnp.connectionWeightRange_max, neatParameters.connectionWeightRange);
			}
			else
			{
				double range = mnp.connectionWeightRange_max - mnp.connectionWeightRange_min;
				neatParameters.connectionWeightRange = mnp.connectionWeightRange_min + Utilities.NextDouble()*range;
			}
		}

		#endregion
	}
}
