using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Gymnasium;

public sealed class GymnasiumEvaluator : IPhenomeEvaluator<IBlackBox<double>>
{
    private const int TrialsPerEvaluation = 1;
    private readonly int _inputCount;
    private readonly int _outputCount;
    private readonly bool _isContinuous;
    private readonly bool _test;

    public GymnasiumEvaluator(int inputCount, int outputCount, bool isContinuous, bool test)
    {
        _inputCount = inputCount;
        _outputCount = outputCount;
        _isContinuous = isContinuous;
        _test = test;
    }

    public FitnessInfo Evaluate(IBlackBox<double> phenome)
    {
        var finesses = new List<FitnessInfo>();
        for (var i = 0; i < TrialsPerEvaluation; i++)
        {
            var episode = new GymnasiumEpisode(_inputCount, _outputCount, _isContinuous, _test);
            finesses.Add(episode.Evaluate(phenome));
        }

        return new FitnessInfo(finesses.Average(fitness => fitness.PrimaryFitness));
    }
}
