
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.FunctionRegression
{
    public interface IBlackBoxProbe
    {
        void Probe(IBlackBox box, double[] responseArr);
    }
}
