
namespace SharpNeat.NeuralNets
{
    /// <summary>
    /// An interface that represents factory classes for obtaining instances of IActivationFunction<typeparamref name="T"/>.
    /// </summary>
    public interface IActivationFunctionFactory<T> where T : struct
    {
        IActivationFunction<T> GetActivationFunction(string name);
    }
}
