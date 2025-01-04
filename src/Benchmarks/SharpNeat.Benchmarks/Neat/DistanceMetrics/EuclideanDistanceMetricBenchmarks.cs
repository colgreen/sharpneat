using BenchmarkDotNet.Attributes;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.Neat.DistanceMetrics;

public class EuclideanDistanceMetricBenchmarks
{
    readonly EuclideanDistanceMetric<float> _distanceMetric = new();
    readonly ConnectionGenes<float>[] _genomeArr;

    public EuclideanDistanceMetricBenchmarks()
    {
        var metaNeatGenome = MetaNeatGenome<float>.CreateAcyclic(12, 1, new LeakyReLU<float>());
        var popLoader = new NeatPopulationLoader<float>(metaNeatGenome);
        List<NeatGenome<float>> genomeList = popLoader.LoadFromZipArchive("data/binary11.pop");
        _genomeArr = genomeList.Select(x => x.ConnectionGenes).ToArray();
    }

    [Benchmark]
    public void FindMedoid()
    {
        _ = _distanceMetric.FindMedoid(_genomeArr);
    }
}
