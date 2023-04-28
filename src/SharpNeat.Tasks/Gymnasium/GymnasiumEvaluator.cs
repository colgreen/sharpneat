using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Gymnasium;

public sealed class GymnasiumEvaluator : IPhenomeEvaluator<IBlackBox<double>>
{
    readonly int _inputCount;
    readonly int _outputCount;
    readonly bool _isContinious;
    readonly bool _test;
    readonly int _trialsPerEvaluation = 1;

    public GymnasiumEvaluator(int inputCount, int outputCount, bool isContinious, bool test)
    {
        _inputCount = inputCount;
        _outputCount = outputCount;
        _isContinious = isContinious;
        _test = test;
    }

    public FitnessInfo Evaluate(IBlackBox<double> phenome)
    {
        var finesses = new List<FitnessInfo>();
        for (int i = 0; i < _trialsPerEvaluation; i++)
        {
            var episode = new GymnasiumEpisode(_inputCount, _outputCount, _isContinious, _test);
            finesses.Add(episode.Evaluate(phenome));
        }

        return new FitnessInfo(finesses.Average(finesses => finesses.PrimaryFitness));
    }
}
