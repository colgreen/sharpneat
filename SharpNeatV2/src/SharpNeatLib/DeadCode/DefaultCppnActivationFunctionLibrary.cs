using System.Collections.Generic;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.HyperNeat
{
    /// <summary>
    /// Default activation function library for CPPNs.
    /// The activation functions in this library are representative of the functions used so far in
    /// published HyperNEAT experiments and other implementations of HyperNEAT.
    /// </summary>
    public class DefaultCppnActivationFunctionLibrary : DefaultActivationFunctionLibrary
    {
        #region Constructor

        /// <summary>
        /// Constructs an activation function library with a default set of activation functions.
        /// </summary>
        public DefaultCppnActivationFunctionLibrary()
        {
            _functionList = new List<ActivationFunctionInfo>(4);
            double[] probabilities = {0.25, 0.25, 0.25, 0.25};
            _functionList.Add(new ActivationFunctionInfo(0, probabilities[0], Linear.__DefaultInstance));
            _functionList.Add(new ActivationFunctionInfo(1, probabilities[1], BipolarSigmoid.__DefaultInstance));
            _functionList.Add(new ActivationFunctionInfo(2, probabilities[2], Gaussian.__DefaultInstance));
            _functionList.Add(new ActivationFunctionInfo(3, probabilities[3], Sine.__DefaultInstance));
            _rwl = new RouletteWheelLayout(probabilities);

            _functionDict = CreateFunctionDictionary(_functionList);
        }

        #endregion




        //readonly List<ActivationFunctionInfo> _functionList;
        //readonly Dictionary<int,IActivationFunction> _functionDict;
        //readonly RouletteWheelLayout _rwl;

        //#region Constructor

        //// ENHANCEMENT: Constructor that reads from a config file.

        ///// <summary>
        ///// Constructs an activation function library with a default set of activation functions.
        ///// </summary>
        //public DefaultCppnActivationFunctionLibrary()
        //{
        //    _functionList = new List<ActivationFunctionInfo>(4);
        //    double[] probabilities = {0.25, 0.25, 0.25, 0.25};
        //    _functionList.Add(new ActivationFunctionInfo(0, probabilities[0], Linear.__DefaultInstance));
        //    _functionList.Add(new ActivationFunctionInfo(1, probabilities[1], BipolarSigmoid.__DefaultInstance));
        //    _functionList.Add(new ActivationFunctionInfo(2, probabilities[2], Gaussian.__DefaultInstance));
        //    _functionList.Add(new ActivationFunctionInfo(3, probabilities[3], Sine.__DefaultInstance));
        //    _rwl = new RouletteWheelLayout(probabilities);

        //    _functionDict = CreateFunctionDictionary(_functionList);
        //}

        //#endregion

        //#region IActivationFunctionLibrary Members

        ///// <summary>
        ///// Gets the function with the specified integer ID.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public IActivationFunction GetFunction(int id)
        //{
        //    return _functionDict[id];
        //}

        ///// <summary>
        ///// Randomly select a function based on each function's selection probability.
        ///// </summary>
        ///// <param name="rng"></param>
        ///// <returns></returns>
        //public ActivationFunctionInfo GetRandomFunction(FastRandom rng)
        //{
        //    return _functionList[RouletteWheel.SingleThrow(_rwl, rng)];
        //}

        ///// <summary>
        ///// Gets a list of all functions in the library.
        ///// </summary>
        ///// <returns></returns>
        //public List<ActivationFunctionInfo> GetFunctionList()
        //{
        //    return _functionList;
        //}

        //#endregion

        //#region Private Methods

        //private static Dictionary<int,IActivationFunction> CreateFunctionDictionary(List<ActivationFunctionInfo> fnList)
        //{
        //    Dictionary<int,IActivationFunction> dict = new Dictionary<int,IActivationFunction>(fnList.Count);
        //    foreach(ActivationFunctionInfo fnInfo in fnList) {
        //        dict.Add(fnInfo.Id, fnInfo.ActivationFunction);
        //    }
        //    return dict;
        //}

        //#endregion
    }
}
