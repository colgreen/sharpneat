/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
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

// ENHANCEMENT: Replace usages of this class with the superceding version from Math.Net.
using System;

namespace SharpNeat.Utility
{
    /// <summary>
    /// A fast random number generator for .NET
    /// Colin Green, January 2005
    /// 
    /// Key points:
    ///  1) Based on a simple and fast xor-shift pseudo random number generator (RNG) specified in: 
    ///  Marsaglia, George. (2003). Xorshift RNGs.
    ///  http://www.jstatsoft.org/v08/i14/paper
    ///  
    ///  This particular implementation of xorshift has a period of 2^128-1. See the above paper to see
    ///  how this can be easily extened if you need a longer period. At the time of writing I could find no 
    ///  information on the period of System.Random for comparison.
    /// 
    ///  2) Faster than System.Random. Up to 8x faster, depending on which methods are called.
    /// 
    ///  3) Direct replacement for System.Random. This class implements all of the methods that System.Random 
    ///  does plus some additional methods. The like named methods are functionally equivalent.
    ///  
    ///  4) Allows fast re-initialisation with a seed, unlike System.Random which accepts a seed at construction
    ///  time which then executes a relatively expensive initialisation routine. This provides a vast speed improvement
    ///  if you need to reset the pseudo-random number sequence many times, e.g. if you want to re-generate the same
    ///  sequence of random numbers many times. An alternative might be to cache random numbers in an array, but that 
    ///  approach is limited by memory capacity and the fact that you may also want a large number of different sequences 
    ///  cached. Each sequence can be represented by a single seed value (int) when using FastRandom.
    ///  
    ///  Notes.
    ///  A further performance improvement can be obtained by declaring local variables as static, thus avoiding 
    ///  re-allocation of variables on each call. However care should be taken if multiple instances of
    ///  FastRandom are in use or if being used in a multi-threaded environment.
    /// 
    /// 
    /// Colin Green, September 4th 2005
    ///   - Added NextBytesUnsafe() - commented out by default.
    ///   - Fixed bug in Reinitialise() - y,z and w variables were not being reset.
    ///     
    /// Colin Green, December 2008.
    ///   - Fix to Next() - Was previously able to return int.MaxValue, contrary to the method's contract and comments.
    ///   - Modified NextBool() to use _bitMask instead of a count of remaining bits. Also reset the bit buffer in Reinitialise().
    ///    
    /// Colin Green, 2011-08-31
    ///   - Added NextByte() method.
    ///   - Added new statically declared seedRng FastRandom to allow easy creation of multiple FastRandoms with different seeds 
    ///     within a single clock tick.
    ///     
    /// Colin Green, 2011-10-04
    ///  - Seeds are now hashed. Without this the first random sample for nearby seeds (1,2,3, etc.) are very similar 
    ///    (have a similar bit pattern). Thanks to Francois Guibert for identifying this problem.
    /// 
    /// </summary>
    public class FastRandom
    {
        #region Static Fields
        /// <summary>
        /// A static RNG that is used to generate seed values when constructing new instances of FastRandom.
        /// This overcomes the problem whereby multiple FastRandom instances are instantiated within the same
        /// tick count and thus obtain the same seed, that approach can result in extreme biases occuring 
        /// in some cases depending on how the RNG is used.
        /// </summary>
        static readonly FastRandom __seedRng = new FastRandom((int)Environment.TickCount);
        #endregion

        #region Instance Fields

        // The +1 ensures NextDouble doesn't generate 1.0
        const double REAL_UNIT_INT = 1.0/((double)int.MaxValue+1.0);
        const double REAL_UNIT_UINT = 1.0/((double)uint.MaxValue+1.0);
        const uint Y=842502087, Z=3579807591, W=273326509;

        uint _x, _y, _z, _w;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance using a seed generated from the class's static seed RNG.
        /// </summary>
        public FastRandom()
        {
            Reinitialise(__seedRng.NextInt());
        }

        /// <summary>
        /// Initialises a new instance using an int value as seed.
        /// This constructor signature is provided to maintain compatibility with
        /// System.Random
        /// </summary>
        public FastRandom(int seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Reinitialisation]

        /// <summary>
        /// Reinitialises using an int value as a seed.
        /// </summary>
        public void Reinitialise(int seed)
        {
            // The only stipulation stated for the xorshift RNG is that at least one of
            // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
            // resetting of the x seed.

            // The first random sample will be very closely related to the value of _x we set here. 
            // Thus setting _x = seed will result in a close correlation between the bit patterns of the seed and
            // the first random sample, therefore if the seed has a pattern (e.g. 1,2,3) then there will also be 
            // a recognisable pattern across the first random samples.
            //
            // Such a strong correlation between the seed and the first random sample is an undesirable
            // charactersitic of a RNG, therefore we significantly weaken any correlation by hashing the seed's bits. 
            // This is achieved by multiplying the seed with four large primes each with bits distributed over the
            // full length of a 32bit value, finally adding the results to give _x.
            _x = (uint)(  (seed * 1431655781) 
                        + (seed * 1183186591)
                        + (seed * 622729787)
                        + (seed * 338294347));

            _y = Y;
            _z = Z;
            _w = W;

            _bitBuffer = 0;
            _bitMask=1;
        }

        #endregion

        #region Public Methods [System.Random functionally equivalent methods]

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue-1.
        /// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
        /// This does slightly eat into some of the performance gain over System.Random, but not much.
        /// For better performance see:
        /// 
        /// Call NextInt() for an int over the range 0 to int.MaxValue.
        /// 
        /// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range
        /// including negative values. 
        /// </summary>
        public int Next()
        {
            uint t = _x^(_x<<11);
            _x=_y; _y=_z; _z=_w;
            _w = (_w^(_w>>19))^(t^(t>>8));

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = _w&0x7FFFFFFF;
            if(rtn==0x7FFFFFFF) {
                return Next();
            }
            return (int)rtn;            
        }

        /// <summary>
        /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
        /// </summary>
        public int Next(int upperBound)
        {
            if(upperBound<0) {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
            }

            uint t = _x^(_x<<11);
            _x=_y; _y=_z; _z=_w;

            // ENHANCEMENT: Can we do this without converting to a double and back again?
            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            return (int)((REAL_UNIT_INT*(int)(0x7FFFFFFF&(_w=(_w^(_w>>19))^(t^(t>>8)))))*upperBound);
        }

        /// <summary>
        /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
        /// upperBound must be >= lowerBound. lowerBound may be negative.
        /// </summary>
        public int Next(int lowerBound, int upperBound)
        {
            if(lowerBound>upperBound) {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
            }

            uint t = _x^(_x<<11);
            _x=_y; _y=_z; _z=_w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            int range = upperBound-lowerBound;
            if(range<0)
            {   // If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
                // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.  
                return lowerBound+(int)((REAL_UNIT_UINT*(double)(_w=(_w^(_w>>19))^(t^(t>>8))))*(double)((long)upperBound-(long)lowerBound));
            }
                
            // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
            // a little more performance.
            return lowerBound+(int)((REAL_UNIT_INT*(double)(int)(0x7FFFFFFF&(_w=(_w^(_w>>19))^(t^(t>>8)))))*(double)range);
        }

        /// <summary>
        /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
        /// </summary>
        public double NextDouble()
        {   
            uint t = _x^(_x<<11);
            _x=_y; _y=_z; _z=_w;

            // Here we can gain a 2x speed improvement by generating a value that can be cast to 
            // an int instead of the more easily available uint. If we then explicitly cast to an 
            // int the compiler will then cast the int to a double to perform the multiplication, 
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect 
            // of the extra cast is a significant performance improvement.
            //
            // Also note that the loss of one bit of precision is equivalent to what occurs within 
            // System.Random.
            return REAL_UNIT_INT*(int)(0x7FFFFFFF&(_w=(_w^(_w>>19))^(t^(t>>8))));         
        }

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// This method is functionally equivalent to System.Random.NextBytes(). 
        /// </summary>
        public void NextBytes(byte[] buffer)
        {
            // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
            uint x=this._x, y=this._y, z=this._z, w=this._w;
            int i=0;
            uint t;
            for(int bound=buffer.Length-3; i<bound;)
            {   
                // Generate 4 bytes. 
                // Increased performance is achieved by generating 4 random bytes per loop.
                // Also note that no mask needs to be applied to zero out the higher order bytes before
                // casting because the cast ignores thos bytes. Thanks to Stefan Trosch�tz for pointing this out.
                t = x^(x<<11);
                x=y; y=z; z=w;
                w=(w^(w>>19))^(t^(t>>8));

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w>>8);
                buffer[i++] = (byte)(w>>16);
                buffer[i++] = (byte)(w>>24);
            }

            // Fill up any remaining bytes in the buffer.
            if(i<buffer.Length)
            {
                // Generate 4 bytes.
                t = x^(x<<11);
                x=y; y=z; z=w;
                w=(w^(w>>19))^(t^(t>>8));

                buffer[i++] = (byte)w;
                if(i<buffer.Length)
                {
                    buffer[i++]=(byte)(w>>8);
                    if(i<buffer.Length)
                    {   
                        buffer[i++] = (byte)(w>>16);
                        if(i<buffer.Length)
                        {   
                            buffer[i] = (byte)(w>>24);
                        }
                    }
                }
            }
            this._x=x; this._y=y; this._z=z; this._w=w;
        }

      ///// <summary>
      ///// A version of NextBytes that uses a pointer to set 4 bytes of the byte buffer in one operation
      ///// thus providing a nice speedup. The loop is also partially unrolled to allow out-of-order-execution,
      ///// this results in about a x2 speedup on an AMD Athlon. Thus performance may vary wildly on different CPUs
      ///// depending on the number of execution units available.
      ///// 
      ///// Another significant speedup is obtained by setting the 4 bytes by indexing pDWord (e.g. pDWord[i++]=_w)
      ///// instead of dereferencing it (e.g. *pDWord++=_w).
      ///// 
      ///// Note that this routine requires the unsafe compilation flag to be specified and so is commented out by default.
      ///// </summary>
      ///// <param name="buffer"></param>
//      public unsafe void NextBytesUnsafe(byte[] buffer)
//      {
//          if(buffer.Length % 8 != 0)
//              throw new ArgumentException("Buffer length must be divisible by 8", "buffer");
//
//          uint _x=this._x, _y=this._y, _z=this._z, _w=this._w;
//          
//          fixed(byte* pByte0 = buffer)
//          {
//              uint* pDWord = (uint*)pByte0;
//              for(int i=0, len=buffer.Length>>2; i < len; i+=2) 
//              {
//                  uint t=(_x^(_x<<11));
//                  _x=_y; _y=_z; _z=_w;
//                  pDWord[i] = _w = (_w^(_w>>19))^(t^(t>>8));
//
//                  t=(_x^(_x<<11));
//                  _x=_y; _y=_z; _z=_w;
//                  pDWord[i+1] = _w = (_w^(_w>>19))^(t^(t>>8));
//              }
//          }
//
//          this._x=_x; this._y=_y; this._z=_z; this._w=_w;
//      }
        #endregion

        #region Public Methods [Methods not present on System.Random]

        /// <summary>
        /// Generates a uint. Values returned are over the full range of a uint, 
        /// uint.MinValue to uint.MaxValue, inclusive.
        /// 
        /// This is the fastest method for generating a single random number because the underlying
        /// random number generator algorithm generates 32 random bits that can be cast directly to 
        /// a uint.
        /// </summary>
        public uint NextUInt()
        {
            uint t = _x^(_x<<11);
            _x=_y; _y=_z; _z=_w;
            return _w=(_w^(_w>>19))^(t^(t>>8));
        }

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue, inclusive. 
        /// This method differs from Next() only in that the range is 0 to int.MaxValue
        /// and not 0 to int.MaxValue-1.
        /// 
        /// The slight difference in range means this method is slightly faster than Next()
        /// but is not functionally equivalent to System.Random.Next().
        /// </summary>
        public int NextInt()
        {
            uint t = _x^(_x<<11);
            _x=_y; _y=_z; _z=_w;
            return (int)(0x7FFFFFFF&(_w=(_w^(_w>>19))^(t^(t>>8))));
        }

        // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
        // with bitMask.
        uint _bitBuffer;
        uint _bitMask;

        /// <summary>
        /// Generates a single random bit.
        /// This method's performance is improved by generating 32 bits in one operation and storing them
        /// ready for future calls.
        /// </summary>
        public bool NextBool()
        {
            if(0 == _bitMask)
            {   
                // Generate 32 more bits.
                uint t = _x^(_x<<11);
                _x=_y; _y=_z; _z=_w;
                _bitBuffer=_w=(_w^(_w>>19))^(t^(t>>8));

                // Reset the bitMask that tells us which bit to read next.
                _bitMask = 0x80000000;
                return (_bitBuffer & _bitMask)==0;
            }

            return (_bitBuffer & (_bitMask>>=1))==0;
        }

        // Buffer of random bytes. A single UInt32 is used to buffer 4 bytes.
        // _byteBufferState tracks how bytes remain in the buffer, a value of 
        // zero  indicates that the buffer is empty.
        uint _byteBuffer;
        byte _byteBufferState;

        /// <summary>
        /// Generates a signle random byte with range [0,255].
        /// This method's performance is improved by generating 4 bytes in one operation and storing them
        /// ready for future calls.
        /// </summary>
        public byte NextByte()
        {
            if(0 == _byteBufferState)
            {
                // Generate 4 more bytes.
                uint t = _x^(_x<<11);
                _x=_y; _y=_z; _z=_w;
                _byteBuffer = _w=(_w^(_w>>19))^(t^(t>>8));
                _byteBufferState = 0x4;
                return (byte)_byteBuffer;  // Note. Masking with 0xFF is unnecessary.
            }
            _byteBufferState >>= 1;
            return (byte)(_byteBuffer >>=1);
        }

        #endregion
    }
}
