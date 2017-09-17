
namespace SharpNeat.NeuralNets
{
    public interface IActivationFunctionFactory<T> where T : struct
    {
        IActivationFunction<T> GetActivationFunction(string name);
    }
}
