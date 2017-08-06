using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Phenomes;

namespace SharpNeatLib.Tests
{
    public static class TestUtils
    {

        public static void Compare(int[] expectedArr, IVector<int> vec)
        {
            Assert.AreEqual(expectedArr.Length, vec.Length);
            for(int i=0; i<expectedArr.Length; i++) {
                Assert.AreEqual(expectedArr[i], vec[i]);
            }
        }

        public static void Compare<T>(T[] expectedArr, T[] actualArr)
        {
            Assert.AreEqual(expectedArr.Length, actualArr.Length);
            for(int i=0; i<expectedArr.Length; i++) {
                Assert.AreEqual(expectedArr[i], actualArr[i]);
            }
        }

        public static void Compare<T>(T[] expectedArr, T[] actualArr, int startIdx, int endIdx)
        {
            for(int i=startIdx; i<endIdx; i++) {
                Assert.AreEqual(expectedArr[i], actualArr[i]);
            }
        }


        public static bool AreEqual<T>(T[] expectedArr, T[] actualArr)
        {
            if(expectedArr.Length != actualArr.Length) {
                return false;
            }

            for(int i=0; i<expectedArr.Length; i++) {
                if(!expectedArr[i].Equals(actualArr[i])) {
                    return false;
                }
            }
            return true;
        }

        public static bool AreEqual<T>(T[] expectedArr, T[] actualArr, int startIdx, int endIdx)
        {
            for(int i=startIdx; i<endIdx; i++) {
                if(!expectedArr[i].Equals(actualArr[i])) {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Function monotonicity test.
        /// </summary>
        public static bool IsMonotonicIncreasing(Func<double,double> fn, double min, double max, double incr, bool strict)
        {
            double y_prev = fn(min);
   
            if(strict)
            {
                // Strictly monotonic text, i.e. must be increasing and not unchanged.
                for(double x = min+incr; x <= max; x += incr)
                {
                    double y = fn(x);
                    y_prev = y;
                }
            }
            else
            {
                for(double x = min+incr; x <= max; x += incr)
                {
                    double y = fn(x);
                    if(y < y_prev) {
                        return false;
                    }
                    y_prev = y;
                }
            }
            return true;
        }
    }
}
