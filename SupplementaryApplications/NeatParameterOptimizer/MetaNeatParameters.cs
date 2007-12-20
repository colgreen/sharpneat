using System;
using System.Collections;

namespace NeatParameterOptimizer
{
	#region ParameterFlags enum

	// A set of flags that indicate which parameters should be searched by the meta-GA.
	public enum ParameterFlag
	{
		populationSize									= 0x00000001,
		pInitialPopulationInterconnections				= 0x00000002,

		pOffspringAsexual_Sexual						= 0x00000004,
//		pOffspringSexual								= 0x00000004,
		pInterspeciesMating								= 0x00000008,
		pDisjointExcessGenesRecombined					= 0x00000010,

		pMutateType										= 0x00000020,
//		pMutateConnectionWeights						= 0x00000020,
//		pMutateAddNode									= 0x00000040,
//		pMutateAddConnection							= 0x00000080,
//		pMutateDeleteConnection							= 0x00000100,
//		pMutateDeleteSimpleNeuron						= 0x00000200,

		ConnectionMutationParameterGroupList			= 0x00000040,

		compatibilityThreshold							= 0x00000080,
		compatibilityDisjointCoeff						= 0x00000100,
		compatibilityExcessCoeff						= 0x00000200,
		compatibilityWeightDeltaCoeff					= 0x00000400,
		elitismProportion								= 0x00000800,
		selectionProportion								= 0x00001000,
		targetSpeciesCountWindow						= 0x00002000,
//		targetSpeciesCountMax							= 0x00002000,
		speciesDropoffAge								= 0x00004000,
		pruningPhaseBeginComplexityThreshold			= 0x00008000,
		pruningPhaseBeginFitnessStagnationThreshold		= 0x00010000,
		pruningPhaseEndComplexityStagnationThreshold	= 0x00020000,

		connectionWeightRange							= 0x00040000,
	}

	#endregion

	public class MetaNeatParameters
	{
		

		ParameterFlag parameterFlags;
		int mutableParameterCount;

		/// <summary>
		/// The IDs of ParameterFlags that are mutable (are set).
		/// </summary>
		ArrayList mutableParameterFlagList = new ArrayList(32);

		/// <summary>
		/// The number of times we run an EvolutionAlgorithm for each NeatParameter object
		/// in a single Meta-algorithm evaluation - we then average out best fitness achieved
		/// on each run.
		/// </summary>
		public int EaRunsPerMetaEvaluation = 20;

		/// <summary>
		/// The proportion of times we reset/jiggle the NeatParameter being mutated.
		/// </summary>
		public double pValueReset = 0.1;
		public double pValueJiggle = 0.9;
		
		public int populationSize_min = 10;
		public int populationSize_max = 1000;
		public double connectionWeightRange_min = 4;
		public double connectionWeightRange_max = 20;

		public int connectionMutationFixedQuantity_min = 1;
		public int connectionMutationFixedQuantity_max = 50;

		#region Properties

		public ParameterFlag ParameterFlags
		{
			get
			{
				return parameterFlags;
			}
			set
			{
				parameterFlags = value;
				UpdateMutableParameterIdList();
			}
		}

		public int MutableParameterCount
		{
			get
			{
				return mutableParameterCount;
			}
		}

		public ArrayList MutableParameterFlagList
		{
			get
			{
				return mutableParameterFlagList;
			}
		}

		#endregion

		#region Private Methods

		private void UpdateMutableParameterIdList()
		{
			mutableParameterCount = 0;
			mutableParameterFlagList.Clear();
			int bitMask = 0x1;
			for(int i=0; i<31; i++)
			{
				if(((int)parameterFlags & bitMask) !=0)
				{
					mutableParameterCount++;
					mutableParameterFlagList.Add(bitMask);
				}
				bitMask<<=1;
			}
		}

		#endregion
	}
}
