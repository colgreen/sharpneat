using System;
using System.Runtime.CompilerServices;

namespace ActivationFnBenchmarks
{
    public static class Functions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LogisticFunctionSteep(double x)
        {
            return 1.0/(1.0 + Math.Exp(-4.9*x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LogisticApproximantSteep(double x)
        {
            return 1.0 / (1.0 + Exp(-4.9 * x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SoftSign(double x)
        {
            return 0.5 + (x / (2.0*(0.2+Math.Abs(x))));
        }

        // This might be based on the Pade approximant:
        //   https://en.wikipedia.org/wiki/Pad%C3%A9_approximant
        //   https://math.stackexchange.com/a/107666
        //
        // Or perhaps the maple minimax approximation:
        //   http://www.maplesoft.com/support/helpJP/Maple/view.aspx?path=numapprox/minimax
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PolynomialApproximant(double x)
        {
            // Very close approximation to LogisticFunctionSteep that avoids exp.
            x = x * 4.9;
            double x2 = x*x;
            double e = 1.0 + Math.Abs(x) + x2*0.555 + x2*x2*0.143;

            double f = (x > 0) ? (1.0 / e) : e;
            return 1.0 / (1.0 + f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double QuadraticSigmoid(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            double sign = Math.Sign(x);
            x = Math.Abs(x);

            double y = 0;
            if(x >= 0 && x < t) {
                y = t - ((x - t) * (x - t));
            }
            else //if (x >= t) 
            {
                y = t + (x - t) * a;
            }

            return (y * sign * 0.5) + 0.5;
        }

        /// <summary>
        /// Rrectified linear activation unit (ReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReLU(double x)
        {
            double y;
            if (x > 0.0) {
                y = x;
            } else {
                y = 0.0;
            }
            return y;
        }


        /// <summary>
        /// Leaky rectified linear activation unit (ReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LeakyReLU(double x)
        {
            const double a = 0.001;

            double y;
            if (x > 0.0) {
                y = x;
            } else {
                y = x * a;
            }
            return y;
        }

        /// <summary>
        /// Leaky rectified linear activation unit (ReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LeakyReLUShifted(double x)
        {
            const double a = 0.001;
            const double offset = 0.5;

            double y;
            if (x+offset > 0.0) {
                y = x+offset;
            } else {
                y = (x+offset) * a;
            }
            return y;
        }

        /// <summary>
        /// S-shaped rectified linear activation unit (SReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        ///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units]
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SReLU(double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;

            double y;
            if(x > tl && x < tr) {
                y = x;
            }
            else if(x <= tl) {
                y = tl + (x - tl) * a;
            }
            else {
                y = tr + (x - tr) * a;
            }

            return y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SReLUShifted(double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;
            const double offset = 0.5;

            double y;
            if(x+offset > tl && x+offset < tr) {
                y = x+offset;
            }
            else if(x+offset <= tl) {
                y = tl + ((x+offset) - tl) * a;
            }
            else {
                y = tr + ((x+offset) - tr) * a;
            }

            return y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ArcTan(double x)
        {
            const double halfpi = Math.PI / 2.0;
            const double piinv = 1.0 / Math.PI;
            return (Math.Atan(x) + halfpi) * piinv;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TanH(double x)
        {
            return (Math.Tanh(x) + 1.0) * 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ArcSinH(double x)
        {
            // Scaling factor from:
            // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/

            return 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
        }


        // Fast exp approximation, from:
        // https://stackoverflow.com/a/412988/15703
        // https://pdfs.semanticscholar.org/35d3/2b272879a2018a2d33d982639d4be489f789.pdf (A Fast, Compact Approximation of the Exponential Function)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Exp(double val)
        {
            long tmp = (long)(1512775 * val + (1072693248 - 60801));
            return BitConverter.Int64BitsToDouble(tmp << 32);
        }

        /// <summary>
        /// Scaled Exponential Linear Unit (SELU).
        /// 
        /// From:
        ///     Self-Normalizing Neural Networks
        ///     https://arxiv.org/abs/1706.02515
        /// 
        /// Original source code (including parameter values):
        ///     https://github.com/bioinf-jku/SNNs/blob/master/selu.py
        ///    
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ScaledELU(double x)
        {
            double alpha = 1.6732632423543772848170429916717;
            double scale = 1.0507009873554804934193349852946;

            double y;
            if(x > 0) {
                y = scale*x;
            } 
            else {
                y = scale*((alpha*Math.Exp(x)) - alpha);
            }

            return y;
        }



        #region Private Static Methods

        /// <summary>
        /// Hyperbolic Area Sine
        /// </summary>
        /// <param name="value">The real value.</param>
        /// <returns>The hyperbolic angle, i.e. the area of its hyperbolic sector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Asinh(double value)
        {
            return Math.Log(value + Math.Sqrt((value * value) + 1), Math.E);
        }

        #endregion
    }
}
