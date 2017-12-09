using System;
using System.Runtime.CompilerServices;

namespace ActivationFnBenchmarks
{
    /// <summary>
    /// Single precision float versions of the functions in the Functions class.
    /// </summary>
    public static class FunctionsFloat
    {
        public static float LogisticFunctionSteep(float x)
        {
            return 1.0f/(1.0f + (float)Math.Exp(-4.9f*x));
        }

        public static double LogisticApproximantSteep(float x)
        {
            return 1f / (1f + Exp(-4.9f * x));
        }

        public static float SoftSign(float x)
        {
            return 0.5f + (x / (2.0f*(0.2f+Math.Abs(x))));
        }

        // This might be based on the Pade approximant:
        //   https://en.wikipedia.org/wiki/Pad%C3%A9_approximant
        //   https://math.stackexchange.com/a/107666
        //
        // Or perhaps the maple minimax approximation:
        //   http://www.maplesoft.com/support/helpJP/Maple/view.aspx?path=numapprox/minimax
        public static float PolynomialApproximant(float x)
        {
            // Very close approximation to LogisticFunctionSteep that avoids exp.
            x = x * 4.9f;
            float x2 = x*x;
            float e = 1.0f + Math.Abs(x) + x2*0.555f + x2*x2*0.143f;

            float f = (x > 0f) ? (1.0f / e) : e;
            return 1.0f / (1.0f + f);
        }

        public static float QuadraticSigmoid(float x)
        {
            const float t = 0.999f;
            const float a = 0.00001f;

            float sign = Math.Sign(x);
            x = Math.Abs(x);

            float y = 0;
            if(x >= 0 && x < t) {
                y = t - ((x - t) * (x - t));
            }
            else //if (x >= t) 
            {
                y = t + (x - t) * a;
            }

            return (y * sign * 0.5f) + 0.5f;
        }

        /// <summary>
        /// S-shaped rectified linear activation unit (SReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        ///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units]
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SReLU(float x)
        {
            const float tl = 0.001f; // threshold (left).
            const float tr = 0.999f; // threshold (right).
            const float a = 0.00001f;

            float y;
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

        public static float SReLUShifted(float x)
        {
            const float tl = 0.001f; // threshold (left).
            const float tr = 0.999f; // threshold (right).
            const float a = 0.00001f;

            x += 0.5f;

            float y;
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

        /// <summary>
        /// Rectified linear activation unit (ReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReLU(float x)
        {
            float y;
            if (x > 0.0F) {
                y = x;
            }
            else {
                y = 0.0F;
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
        public static float LeakyReLU(float x)
        {
            const float a = 0.001F;

            float y;
            if (x > 0.0F) {
                y = x;
            }
            else {
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
        public static float LeakyReLUShifted(float x)
        {
            const float a = 0.001F;
            const float offset = 0.5F;

            float y;
            if (x + offset > 0.0F) {
                y = x + offset;
            }
            else {
                y = (x + offset) * a;
            }
            return y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ArcTanF(float x)
        {
            const float halfpi = (float)Math.PI / 2.0F;
            const float piinv = 1.0F / (float)Math.PI;
            return ((float)Math.Atan(x) + halfpi) * piinv;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TanHF(float x)
        {
            return ((float)Math.Tanh(x) + 1.0F) * 0.5F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ArcSinHF(float x)
        {
            // Scaling factor from:
            // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/

            return 1.2567348023993685F * ((AsinhF(x) + 1.0F) * 0.5F);
        }

        /// <summary>
        /// Hyperbolic Area Sine
        /// </summary>
        /// <param name="value">The real value.</param>
        /// <returns>The hyperbolic angle, i.e. the area of its hyperbolic sector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float AsinhF(float value)
        {
            const float MathEF = 2.7182818284590451F;
            return FastLog.Log(value + (float)Math.Sqrt((value * value) + 1F), MathEF);
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
        public static float ScaledELUF(float x)
        {
            const float alpha = 1.6732632423543772848170429916717F;
            const float scale = 1.0507009873554804934193349852946F;

            float y;
            if (x >= 0F) {
                y = scale * x;
            }
            else {
                y = scale * ((alpha * Exp(x)) - alpha);
            }

            return y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxMinusOneF(float x)
        {
            float y;
            if (x > -1F) {
                y = x;
            } 
            else {
                y = -1F;
            }
            return y;
        }


        // Fast exp approximation, from:
        // https://stackoverflow.com/a/412988/15703
        // https://pdfs.semanticscholar.org/35d3/2b272879a2018a2d33d982639d4be489f789.pdf (A Fast, Compact Approximation of the Exponential Function)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Exp(float val)
        {
            long tmp = (long)(1512775 * (double)val + (1072693248 - 60801));
            return (float)BitConverter.Int64BitsToDouble(tmp << 32);
        }
    }
}
