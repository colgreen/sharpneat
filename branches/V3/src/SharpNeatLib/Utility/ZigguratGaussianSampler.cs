/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2011 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Diagnostics;

namespace SharpNeat.Utility
{
    // ENHANCEMENT: Further performance improvement can be obtained by using a less precise method
    // whereby we represent the distribution curve as a piecewise linear curve, i.e. approximate
    // the curve using stacked trapezoids instead of stacked rectangles, and skip handling of the
    // corner cases where we would normally perform expensive calculations to obtain precise 
    // results. Such an approach should be suitable for generating mutations for evolutionary
    // computing.
    // For details about this approach see:
    // Hardware-Optimized Ziggurat Algorithm for High-Speed Gaussian Random Number Generators,
    // Hassan M. Edrees, Brian Cheung, McCullen Sandora, David Nummey, Deian Stefan
    // (http://www.ee.cooper.edu/~stefan/pubs/conference/ersa2009.pdf)
    //
    /// <summary>
    /// A fast Gaussian distribution sampler for .Net
    /// Colin Green, 11/09/2011
    ///
    /// An implementation of the Ziggurat algorithm for random sampling from a Gaussian 
    /// distribution. See:
    ///  - Wikipedia:Ziggurat algorithm (http://en.wikipedia.org/wiki/Ziggurat_algorithm).
    ///  - The Ziggurat Method for Generating Random Variables, George Marsaglia and
    ///    Wai Wan Tsang (http://www.jstatsoft.org/v05/i08/paper).
    ///  - An Improved Ziggurat Method to Generate Normal Random Samples, Jurgen A Doornik 
    ///    (http://www.doornik.com/research/ziggurat.pdf)
    ///  
    /// 
    /// Ziggurat Algorithm Overview
    /// ============================
    /// 
    /// Consider the right hand side of the Gaussian probability density function (for x >=0) as
    /// described by y = f(x). This half of the distribution is covered by a series of stacked
    /// horizontal rectangles, like so:
    /// 
    ///  _____
    /// |     |                    R6  S6
    /// |     |
    /// |_____|_                   
    /// |       |                  R5  S5
    /// |_______|_                 
    /// |         |                R4  S4
    /// |_________|__       
    /// |____________|__           R3  S3
    /// |_______________|________  R2  S2
    /// |________________________| R1  S1
    /// |________________________| R0  S0
    ///           (X)
    /// 
    /// 
    /// The Basics
    /// ----------
    /// (1) 
    /// Each rectangle is assigned a number (the R numbers shown above). 
    /// 
    /// (2) 
    /// The right hand edge of each rectangle is placed so that it just covers the distribution,
    /// that is, the bottom right corner is on the curve, and therefore some of the area in the 
    /// top right of the rectangle is outside of the distribution (points with y greater than 
    /// f(x)), except for R0 (see next point). Therefore the rectangles taken together cover an
    /// area slightly larger than the distribution curve.
    /// 
    /// (3) 
    /// R0 is a special case. The tail of the Gaussian effectively projects into x=Infinity
    /// asymptotically approaching zero, thus we do not cover the tail with a rectangle. Instead
    /// we define a cut-off point (x=3.442619855899 in this implementation). R0's right hand
    /// edge is at the cut-off point with its top right corner on the distribution curve. The
    /// tail is then defined as that part of the distribution with x > tailCutOff and is
    /// combined with R0 to form segment S0. Note that the whole of R0 is within the
    /// distribution, unlike the other rectangles.
    /// 
    /// (4)
    /// Segments. Each rectangle is also referred to as a segment with the exception of R0 which
    /// is a special case as explained above. Essentially S[i] == R[i], except for 
    /// S[0] == R[0] + tail.
    /// 
    /// (5)
    /// Each segment has identical area A, this also applies to the special segment S0, thus the
    /// area of R0 is A minus the area represented by the tail. For all other segments the
    /// segment area is the same as the rectangle area.
    /// 
    /// (6)
    /// R[i] has right hand edge x[i]. And from drawing the rectangles over the distribution
    /// curve it is clear that the region of R[i] to the left of x[i+1] is entirely within the
    /// distribution curve, whereas the region greater than x[i+1] is partially above the
    /// distribution curve. 
    /// 
    /// (7)
    /// R[i] has top edge of y[i].
    /// 
    /// 
    /// Operation
    /// ---------
    /// (1)
    /// Randomly select a segment to sample from, call this S[i], this amounts to a low
    /// resolution random y coordinate. Because the segments have equal area we can select from
    /// them with equal probability. (Also see special notes, below).
    /// 
    /// (2)
    /// Segment 0 is a special case, if S0 is selected then generate a random area value w
    /// between 0 and A. If w is less than or equal to the area of R0 then we are sampling a
    /// point from within R0 (step 2A), otherwise we are sampling from the tail (step 2B).
    /// 
    /// (2A)
    /// Sampling from R0. R0 is entirely within the distribution curve and we have already
    /// generated a random area value w. Convert w to an x value that we can return by dividing
    /// w by the height of R0 (y[0]).
    /// 
    /// (2B)
    /// Sampling from the tail. To sample from the tail we fall back to a slow implementation
    /// using logarithms, see: 
    /// Generating a Variable from the Tail of the Normal Distribution, George Marsaglia (1963).
    /// (http://www.dtic.mil/cgi-bin/GetTRDoc?AD=AD423993&Location=U2&doc=GetTRDoc.pdf)
    /// The area represented by the tail is relatively small and therefore this execution
    /// pathway is avoided for a significant proportion of samples generated.
    ///
    /// (3)
    /// Sampling from all other rectangles/segments other then R0/S0. 
    /// Randomly select x from within R[i]. If x is less than x[i+1] then x is within the curve,
    /// return x.
    ///    
    /// If x is greater than or equal to x[i+1] then generate a random y variable from within
    /// R[i] (this amounts to producing a high resolution y coordinate, a refinement of the low
    /// resolution y coord we effectively produced by selecting a rectangle/segment).
    ///    
    /// If y is below f(x) then return x, otherwise we disregard the sample point and return to
    /// step 1. We specifically do *not* re-attempt to sample from R[i] until a valid point is
    /// found (see special notes 1).
    /// 
    /// (4)
    /// Finally, all of the above describes sampling from the positive half of the distribution
    /// (x greater than or equal to zero) hence to obtain a symetrical distribution we need one
    /// more random bit to decide whether to flip the sign of the returned x.
    /// 
    /// 
    /// Special notes
    /// -------------
    /// (Note 1) 
    /// Segments have equal area and are thus selected with equal probability. However, the area
    /// under the distribution curve covered by each segment/rectangle differs where rectangles
    /// overlap the edge of the distribution curve. Thus it has been suggested that to avoid
    /// sampling bias that the segments should be selected with a probability that reflects the
    /// area of the distribution curve they cover not their total area, this is an incorrect
    /// approach for the algorithm as described above and implemented in this class. To explain
    /// why consider an extreme case. 
    /// 
    /// Say that rectangle R1 covers an area entirely within the distribution curve, now consider
    /// R2 that covers an area only 10% within the curve. Both rectangles are chosen with equal
    /// probability, thus the argument is that R2 will be 10x overrepresented (will generate
    /// sample points as often as R1 despite covering a much smaller proportion of the area under
    /// the distribution curve). In reality sample points within R2 will be rejected 90% of the
    /// time and we disregard the attempt to sample from R2 and go back to step 1 (select a
    /// segment to sample from).
    /// 
    /// If instead we re-attempted sampling from R2 until a valid point was found then R2 would 
    /// indeed become over-represented, hence we do not do this and the algorithm therefore does
    /// not exhibit any such bias.
    /// 
    /// (Note 2)
    /// George Marsaglia's original implementation used a single random number (32bit unsigned
    /// integer) for both selecting the segment and producing the x coordinate with the chosen
    /// segment. The segment index was taken from the the least significant bits (so the least
    /// significant 7 bits if using 128 segments). This effectively created a perculair type of
    /// bias in which all x coords produced within a given segment would have an identical least
    /// significant 7 bits, albeit prior to casting to a floating point value. The bias is perhaps
    /// small especially in comparison to the performance gain (one less call to the RNG). This 
    /// implementation avoids this bias by not re-using random bits in such a way. For more info 
    /// see:
    /// An Improved Ziggurat Method to Generate Normal Random Samples, Jurgen A Doornik 
    /// (http://www.doornik.com/research/ziggurat.pdf)
    /// 
    /// 
    /// Optimizations
    /// -------------
    /// (Optimization 1) 
    /// On selecting a segment/rectangle we generate a random x value within the range of the
    /// rectangle (or the range of the area of S0), this requires multiplying a random number with
    /// range [0,1] to the requried x range before performing the first test for x being within the
    /// 'certain' left-hand side of the rectangle. We avoid this multiplication and indeed
    /// conversion of a random integer into a float with range [0,1], thus allowing the first 
    /// comparison to be performed using integer arithmetic.
    /// 
    /// Instead of using the x coord of RN+1 to test whether a randomly generated point within RN
    /// is within the 'certain' left hand side part of the distribution, we precalculate the
    /// probability of a random x coord being within the safe part for each rectangle. Furthermore
    /// we store this probability as a UInt with range [0, 0xffffffff] thus allowing direct
    /// comparison with randomly generated UInts from the RNG, this allows the comparison to be
    /// performed using integer arithmetic. If the test succeeds then we continue to convert the
    /// random value into an appropriate x sample value.
    ///  
    /// (Optimization 2)
    /// Simple collapsing of calculations into precomputed values where possible. This affects 
    /// readability, but hopefully the above explanations will help understand the code if necessary.
    /// 
    /// (Optimization 3)
    /// The gaussian probability density function (PDF) contains terms for distribution mean and 
    /// standard deviation. We remove all excess terms and denormalise the function to obtain a 
    /// simpler equation with the same shape. This simplified equation is no longer a PDF as the
    /// total area under the curve is no loner 1.0 (a key property of PDFs), however as it has the
    /// same overall shape it remains suitable for sampling from a Gaussian using rejection methods
    /// such as the Ziggurat algorithm (it's the shape of the curve that matters, not the absolute
    /// area under the curve).
    /// </summary>
    public class ZigguratGaussianSampler
    {
        #region Static Fields [Defaults]

        /// <summary>
        /// Number of blocks.
        /// </summary>
        const int __blockCount = 128;
        /// <summary>
        /// Right hand x coord of the base rectangle, thus also the left hand x coord of the tail 
        /// (pre-determined/computed for 128 blocks).
        /// </summary>
        const double __R = 3.442619855899;
        /// <summary>
        /// Area of each rectangle (pre-determined/computed for 128 blocks).
        /// </summary>
        const double __A = 9.91256303526217e-3;
        /// <summary>
        /// Scale factor for converting a UInt with range [0,0xffffffff] to a double with range [0,1].
        /// </summary>
        const double __UIntToU = 1.0 / (double)uint.MaxValue;

        #endregion

        #region Instance Fields

        readonly FastRandom _rng;

        // _x[i] and _y[i] describe the top-right position ox rectangle i.
        readonly double[] _x;
        readonly double[] _y;

        // The proprtion of each segment that is entirely within the distribution, expressed as uint where 
        // a value of 0 indicates 0% and uint.MaxValue 100%. Expressing this as an integer allows some floating
        // points operations to be replaced with integer ones.
        readonly uint[] _xComp;

        // Useful precomputed values.
        // Area A divided by the height of B0. Note. This is *not* the same as _x[i] because the area 
        // of B0 is __A minus the area of the distribution tail.
        readonly double _A_Div_Y0;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public ZigguratGaussianSampler() 
            : this(new FastRandom())
        {
        }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public ZigguratGaussianSampler(int seed) 
            : this(new FastRandom(seed))
        {
        }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public ZigguratGaussianSampler(FastRandom rng)
        {
            _rng = rng;

            // Initialise rectangle position data. 
            // _x[i] and _y[i] describe the top-right position ox Box i.

            // Allocate storage. We add one to the length of _x so that we have an entry at _x[_blockCount], this avoids having 
            // to do a special case test when sampling from the top box.
            _x = new double[__blockCount + 1];
            _y = new double[__blockCount];

            // Determine top right position of the base rectangle/box (the rectangle with the Gaussian tale attached). 
            // We call this Box 0 or B0 for short.
            // Note. x[0] also describes the right-hand edge of B1. (See diagram).
            _x[0] = __R; 
            _y[0] = GaussianPdfDenorm(__R);

            // The next box (B1) has a right hand X edge the same as B0. 
            // Note. B1's height is the box area divided by its width, hence B1 has a smaller height than B0 because
            // B0's total area includes the attached distribution tail.
            _x[1] = __R;
            _y[1] =  _y[0] + (__A / _x[1]);

            // Calc positions of all remaining rectangles.
            for(int i=2; i<__blockCount; i++)
            {
                _x[i] = GaussianPdfDenormInv(_y[i-1]);
                _y[i] = _y[i-1] + (__A / _x[i]);   
            }

            // For completeness we define the right-hand edge of a notional box 6 as being zero (a box with no area).
            _x[__blockCount] = 0.0;

            // Useful precomputed values.
            _A_Div_Y0 = __A / _y[0];
            _xComp = new uint[__blockCount];

            // Special case for base box. _xComp[0] stores the area of B0 as a proportion of __R 
            // (recalling that all segments have area __A, but that the base segment is the combination of B0 and the distribution tail).
            // Thus -xComp[0[ is the probability that a sample point is within the box part of the segment.
            _xComp[0] = (uint)(((__R * _y[0]) / __A) * (double)uint.MaxValue);

            for(int i=1; i<__blockCount-1; i++) {
                _xComp[i] = (uint)((_x[i+1] / _x[i]) * (double)uint.MaxValue);
            }
            _xComp[__blockCount-1] = 0;  // Shown for completeness.

            // Sanity check. Test that the top edge of the topmost rectangle is at y=1.0.
            // Note. We expect there to be a tiny drift away from 1.0 due to the inexactness of floating
            // point arithmetic.
            Debug.Assert(Math.Abs(1.0 - _y[__blockCount-1]) < 1e-10);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the next sample value from the gaussian distribution.
        /// </summary>
        public double NextSample()
        {
            for(;;)
            {
                // Select box at random.
                byte u = _rng.NextByte();
                int i = (int)(u & 0x7F);
                double sign = ((u & 0x80) == 0) ? -1.0 : 1.0;

                // Generate uniform random value with range [0,0xffffffff].
                uint u2 = _rng.NextUInt();

                // Special case for the base segment.
                if(0 == i)
                {
                    if(u2 < _xComp[0]) 
                    {   // Generated x is within R0.
                        return u2 * __UIntToU * _A_Div_Y0 * sign;
                    }
                    // Generated x is in the tail of the distribution.
                    return SampleTail() * sign;
                }

                // All other segments.
                if(u2 < _xComp[i]) 
                {   // Generated x is within the rectangle.
                    return u2 * __UIntToU * _x[i] * sign;
                }

                // Generated x is outside of the rectangle.
                // Generate a random y coordinate and test if our (x,y) is within the distribution curve.
                // This execution path is relatively slow/expensive (makes a call to Math.Exp()) but relatively rarely executed,
                // although more often than the 'tail' path (above).
                double x = u2 * __UIntToU * _x[i];
                if(_y[i-1] + ((_y[i] - _y[i-1]) * _rng.NextDouble()) < GaussianPdfDenorm(x) ) {
                    return x * sign;
                }
            }
        }

        /// <summary>
        /// Get the next sample value from the gaussian distribution.
        /// </summary>
        /// <param name="mu">The distribution's mean.</param>
        /// <param name="mu">The distribution's standard deviation.</param>
        public double NextSample(double mu, double sigma)
        {
            return mu + (NextSample() * sigma);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sample from the distribution tail (defined as having x >= __R).
        /// </summary>
        /// <returns></returns>
        private double SampleTail()
        {
            double x, y;
            do
            {
                // Note. we use NextDoubleNonZero() because Log(0) returns NaN and will also tend to be a very slow execution path (when it occurs, which is rarely).
                x = -Math.Log(_rng.NextDoubleNonZero()) / __R;
                y = -Math.Log(_rng.NextDoubleNonZero());
            }
            while(y+y < x*x);
            return __R + x;
        }

        /// <summary>
        /// Gaussian probability density function, denormailised, that is, y = e^-(x^2/2).
        /// </summary>
        private double GaussianPdfDenorm(double x)
        {
            return Math.Exp(-(x*x / 2.0));
        }

        /// <summary>
        /// Inverse function of GaussianPdfDenorm(x)
        /// </summary>
        private double GaussianPdfDenormInv(double y)
        {   
            // Operates over the y range (0,1], which happens to be the y range of the pdf, 
            // with the exception that it does not include y=0, but we would never call with 
            // y=0 so it doesn't matter. Remember that a Gaussian effectively has a tail going
            // off into x == infinity, hence asking what is x when y=0 is an invalid question
            // in the context of this class.
            return Math.Sqrt(-2.0 * Math.Log(y));
        }

        #endregion
    }
}
