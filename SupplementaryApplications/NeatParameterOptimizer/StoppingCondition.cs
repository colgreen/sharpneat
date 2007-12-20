using System;

namespace NeatParameterOptimizer
{
	public enum EAStoppingConditionType
	{
		MaxDuration,
		MaxGenerations,
		Combined
	}

	/// <summary>
	/// EvolutionAlgorithmStoppingCondition.
	/// </summary>
	public class EAStoppingCondition
	{
		#region Class Variables

		private EAStoppingConditionType stoppingConditionType;
		private int maxEvaluationDurationSecs;
		private int maxGenerations;

		#endregion

		#region Private Constructor

		private EAStoppingCondition(EAStoppingConditionType type, int limit1, int limit2)
		{
			switch(type)
			{
				case EAStoppingConditionType.MaxDuration:
				{
					maxEvaluationDurationSecs = limit1;
					break;
				}
				case EAStoppingConditionType.MaxGenerations:
				{
					maxGenerations = limit1;
					break;
				}
				case EAStoppingConditionType.Combined:
				{
					maxEvaluationDurationSecs = limit1;
					maxGenerations = limit2;
					break;
				}
			}
			stoppingConditionType = type;
		}

		#endregion

		#region Properties

		public EAStoppingConditionType StoppingConditionType
		{
			get
			{
				return stoppingConditionType;
			}
		}

		public int MaxEvaluationDurationSecs
		{
			get
			{
				return maxEvaluationDurationSecs;
			}
		}

		public int MaxGenerations
		{
			get
			{
				return maxGenerations;
			}
		}

		#endregion

		#region Public Static Methods [Factory methods]

		public static EAStoppingCondition CreateStoppingCondition_MaxDuration(int maxEvaluationDurationSecs)
		{
			return new EAStoppingCondition(EAStoppingConditionType.MaxDuration, maxEvaluationDurationSecs, 0);
		}

		public static EAStoppingCondition CreateStoppingCondition_MaxGenerations(int maxGenerations)
		{
			return new EAStoppingCondition(EAStoppingConditionType.MaxGenerations, maxGenerations, 0);
		}

		public static EAStoppingCondition CreateStoppingCondition_Combined(int maxEvaluationDurationSecs, int maxGenerations)
		{
			return new EAStoppingCondition(EAStoppingConditionType.Combined, maxEvaluationDurationSecs, maxGenerations);
		}

		#endregion
	}
}
