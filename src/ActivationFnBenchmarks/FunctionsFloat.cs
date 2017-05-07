using System;

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
    }
}
