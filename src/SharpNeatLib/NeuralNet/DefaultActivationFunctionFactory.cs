using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpNeat.NeuralNet
{
    /// <summary>
    /// Default implementation of IActivationFunctionFactory.
    /// 
    /// A factory class for obtaining instances of IActivationFunction<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class DefaultActivationFunctionFactory<T> : IActivationFunctionFactory<T>
        where T : struct
    {
        #region Instance Fields

        /// <summary>
        /// If true then hardware accelerated activation functions are not used even when they are available.
        /// </summary>
        readonly bool _suppressHardwareAcceleration = false;

        /// <summary>
        /// A dictionary of activation function instances keyed by class name.
        /// </summary>
        readonly Dictionary<string,IActivationFunction<T>> _fnByName = new Dictionary<string, IActivationFunction<T>>();

        readonly object _lockObj = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DefaultActivationFunctionFactory()
        {}

        /// <summary>
        /// Construct with the provided options.
        /// </summary>
        /// <param name="suppressHardwareAcceleration">If true then hardware accelerated activation functions are not used even when they are available.</param>
        public DefaultActivationFunctionFactory(bool suppressHardwareAcceleration)
        {
            _suppressHardwareAcceleration = suppressHardwareAcceleration;
        }

        #endregion

        #region Public Methods

        public IActivationFunction<T> GetActivationFunction(string name)
        {
            lock(_lockObj)
            {
                // Check for an exiting instance in the activation function cache.
                if(_fnByName.TryGetValue(name, out IActivationFunction<T> actFn)) {
                    return actFn;
                }

                // No entry in the cache, attempt to create a new instance.
                if(!_suppressHardwareAcceleration && Vector.IsHardwareAccelerated)
                {   // Attempt to get a hardware accelerated instance.
                    actFn = TryCreateVectorized(name);
                }

                if(null == actFn)
                {   // Attempt to get a non hardware accelerated instance.
                    actFn = TryCreate(name);
                }

                if(null != actFn)
                {
                    // Add to the cache for future use.
                    _fnByName.Add(name, actFn);

                    return actFn;
                }
            }

            throw new Exception("Unknown activation function name [{name}].");
        }

        #endregion

        #region Private Methods

        private IActivationFunction<T> TryCreate(string name)
        {
            // Get the generic type parameter name (i.e. Float or Double).
            string valueType = this.GetType().GetGenericArguments()[0].Name;

            // Build fully namespaced type name.
            string fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.{name}";

            // Attempt to get an instance with the full name.
            var actFn = TryCreateFromFullName(fullName);
            if(null != actFn) {
                return actFn;
            }

            // Attempt again in the CPPN sub-namespace.
            fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.Cppn.{name}";
            return TryCreateFromFullName(fullName);
        }

        private IActivationFunction<T> TryCreateVectorized(string name)
        {
            // Get the generic type parameter name (i.e. Float or Double).
            string valueType = this.GetType().GetGenericArguments()[0].Name;

            // Build fully namespaced type name.
            string fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.Vectorized.{name}";

            // Attempt to get an instance with the full name.
            var actFn = TryCreateFromFullName(fullName);
            if(null != actFn) {
                return actFn;
            }

            // Attempt again in the CPPN sub-namespace.
            fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.Cppn.Vectorized.{name}";
            return TryCreateFromFullName(fullName);
        }

        private static IActivationFunction<T> TryCreateFromFullName(string fullName)
        {
            // Attempt to get an activation type with the specified name.
            Type type = Type.GetType(fullName);

            // If no such type found then return null.
            if(null == type) {
                return null;
            }

            return (IActivationFunction<T>)Activator.CreateInstance(type);
        }

        #endregion
    }
}
