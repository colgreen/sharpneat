using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Gymnasium;

internal class GymnasiumEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    /// <inheritdoc/>
    public int InputCount => 24;

    /// <inheritdoc/>
    public int OutputCount => 4;

    /// <inheritdoc/>
    public bool IsDeterministic => false;

    /// <inheritdoc/>
    public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

    /// <inheritdoc/>
    public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

    /// <inheritdoc/>
    public bool EvaluatorsHaveState => false;

    /// <inheritdoc/>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new GymnasiumEvaluator(InputCount, OutputCount, true, false);
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 300.0);
    }
}
