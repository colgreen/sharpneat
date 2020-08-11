using SharpNeat.NeuralNets;
using SharpNeat.NeuralNets.Double.ActivationFunctions.Vectorized;

namespace SharpNeat.Benchmarks
{
    public class ActivationFunctionsVectorizedDouble
    {
        //public static readonly IActivationFunction<double> ArcSinH = new ArcSinH();
        //public static readonly IActivationFunction<double> ArcTan = new ArcTan();
        public static readonly IActivationFunction<double> LeakyReLU = new LeakyReLU();
        public static readonly IActivationFunction<double> LeakyReLUShifted = new LeakyReLUShifted();
        //public static readonly IActivationFunction<double> LogisticApproximantSteep = new LogisticApproximantSteep();
        //public static readonly IActivationFunction<double> Logistic = new Logistic();
        //public static readonly IActivationFunction<double> LogisticSteep = new LogisticSteep();
        public static readonly IActivationFunction<double> MaxMinusOne = new MaxMinusOne();
//        public static readonly IActivationFunction<double> NullFn = new NullFn();
        public static readonly IActivationFunction<double> PolynomialApproximantSteep = new PolynomialApproximantSteep();
        public static readonly IActivationFunction<double> QuadraticSigmoid = new QuadraticSigmoid();
        public static readonly IActivationFunction<double> ReLU = new ReLU();
 //      public static readonly IActivationFunction<double> ScaledELU = new ScaledELU();
        public static readonly IActivationFunction<double> SoftSignSteep = new SoftSignSteep();
        public static readonly IActivationFunction<double> SReLU = new SReLU();
        public static readonly IActivationFunction<double> SReLUShifted = new SReLUShifted();
 //       public static readonly IActivationFunction<double> TanH = new TanH();
    }
}
