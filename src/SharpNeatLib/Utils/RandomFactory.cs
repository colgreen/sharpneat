using System;
using System.Security.Cryptography;
using Redzen.Numerics;

namespace SharpNeat.Utils
{
    /// <summary>
    /// A source of IRandomSource instance that are guaranteed to have weel distributed seeds.
    /// </summary>
    /// <remarks>
    /// This solves the problem of using classes such as System.Ranom with the default constructor that uses
    /// the current system time as a ranodm seed. In modern systems it's possible to create a gret many Random instances
    /// within the time of a single system tick, thus the RNGs all get the same seed.
    /// </remarks>
    public static class RandomFactory
    {
        static readonly XorShiftRandom __seedRng;
        static readonly object __lockObj = new object();

        #region Static Initializer

        static RandomFactory()
        {
            using (RNGCryptoServiceProvider cryptoRng = new RNGCryptoServiceProvider())
            {
                // Create a random seed. Note. this uses system entropy as a source of external randomness.
                byte[] buf = new byte[4];
                cryptoRng.GetBytes(buf);
                int seed = BitConverter.ToInt32(buf, 0);

                // Init a single pseudo-random RNG for generating seeds for other RNGs.
                __seedRng = new XorShiftRandom(seed);
            }
        }

        #endregion

        #region Public Static Factory Method

        public static IRandomSource Create()
        {
            // Get a new seed. 
            // Calls to __seedRng need to be sync locked because it has state and is not re-entrant; as such 
            // we get and release the lock as quickly as possible.
            int seed;
            lock(__lockObj){
                seed = __seedRng.NextInt();
            }

            return new XorShiftRandom(seed);
        }

        #endregion
    }
}
