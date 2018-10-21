
namespace SharpNeat.NeuralNet
{
    /// <summary>
    /// An interface that represents factory classes for obtaining instances of IActivationFunction<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public interface IActivationFunctionFactory<T> where T : struct
    {
        IActivationFunction<T> GetActivationFunction(string name);
    }
}
