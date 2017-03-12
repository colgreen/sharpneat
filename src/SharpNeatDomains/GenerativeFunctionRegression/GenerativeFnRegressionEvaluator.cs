using SharpNeat.Domains.FunctionRegression;

namespace SharpNeat.Domains.GenerativeFunctionRegression
{
    public class GenerativeFnRegressionEvaluator : FnRegressionEvaluator
    {
        /// <summary>
        /// Construct a generative function regression evaluator with the provided parameter sampling info and function to regress.
        /// </summary>
        public GenerativeFnRegressionEvaluator(IFunction fn, ParamSamplingInfo paramSamplingInfo)
            : base(fn, paramSamplingInfo, CreateGenerativeBlackBoxProbe(fn, paramSamplingInfo))
        {
        }

        private static GenerativeBlackBoxProbe CreateGenerativeBlackBoxProbe(IFunction fn, ParamSamplingInfo paramSamplingInfo)
        {
            // Determine the mid output value of the function (over the specified sample points) and a scaling factor
            // to apply the to neural netwkrk response for it to be able to recreate the function (because the neural net
            // output range is [0,1] when using the logistic function as the neurn activation function).
            double scale, mid;
            FnRegressionUtils.CalcFunctionMidAndScale(fn, paramSamplingInfo, out mid, out scale);

            var blackBoxProbe = new GenerativeBlackBoxProbe(paramSamplingInfo, mid, scale);
            return blackBoxProbe;
        }
    }
}
