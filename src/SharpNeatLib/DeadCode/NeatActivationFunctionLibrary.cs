using System.Collections.Generic;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// An IActivationFunctionLibrary for NEAT. 
    /// NEAT uses the same activation function at each node and therefore does not strictly require an
    /// activation function library. This implementation takes a single activation function at construction 
    /// time and returns that same activation function regardless of what function ID is requested.
    /// </summary>
    public class NeatActivationFunctionLibrary : IActivationFunctionLibrary
    {
        readonly IActivationFunction _activationFn;
        readonly ActivationFunctionInfo _activationFnInfo;
        readonly List<ActivationFunctionInfo> _activationFnInfoList;

        #region Constructor

        /// <summary>
        /// Construct with a single IActivationFunction.
        /// </summary>
        /// <param name="activationFn"></param>
        public NeatActivationFunctionLibrary(IActivationFunction activationFn)
        {
            _activationFn = activationFn;
            _activationFnInfo = new ActivationFunctionInfo(0, 1.0, activationFn);
            _activationFnInfoList = new List<ActivationFunctionInfo>(1);
            _activationFnInfoList.Add(_activationFnInfo);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the single NEAT activation function provided at construction time.
        /// The id parameter is ignored.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActivationFunction GetFunction(int id)
        {
            return _activationFn; 
        }

        /// <summary>
        /// Gets the single NEAT activation function provided at construction time.
        /// The provided random number generator is not used in this implementation.
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ActivationFunctionInfo GetRandomFunction(FastRandom rng)
        {
            return _activationFnInfo;
        }

        /// <summary>
        /// Gets a list containing the single NEAT activation function.
        /// </summary>
        /// <returns></returns>
        public IList<ActivationFunctionInfo> GetFunctionList()
        {
            return _activationFnInfoList;
        }

        #endregion
    }
}
