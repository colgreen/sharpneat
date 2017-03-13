using System;

namespace SharpNeat.Domains.FunctionRegression
{
    /// <summary>
    /// Static helper methods.
    /// </summary>
    public static class FunctionUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Get an instance of the function class for the specified function type.
        /// </summary>
        /// <param name="fnId"></param>
        /// <returns></returns>
        public static IFunction GetFunction(FunctionId fnId)
        {
            switch(fnId)
            {
                case FunctionId.Abs:
                    return new AbsFunction();    

                case FunctionId.Log:
                    return new LogFn();

                case FunctionId.Sin:
                    return new SinFn();                

                case FunctionId.SinXSquared:
                    return new SinXSquaredFn();

                case FunctionId.SinXPlusSinX:
                    return new SinXPlusSinXFn();      
            }
            throw new ArgumentException($"Unknown FunctionId type [{fnId}]");
        }

        #endregion
    }
}
