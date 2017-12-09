using System;
using System.Runtime.InteropServices;

namespace ActivationFnBenchmarks
{
    public static class FastLog
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Ieee754
        {
            [FieldOffset(0)] public float Single;
            [FieldOffset(0)] public uint UnsignedBits;
            [FieldOffset(0)] public int SignedBits;

            public uint Sign => UnsignedBits >> 31;
            public int Exponent => (SignedBits >> 23) & 0xFF;
            public uint Mantissa => UnsignedBits & 0x007FFFFF;
        }

        static readonly float[] __mantissaLogs = new float[(int)Math.Pow(2, 23)];
        const float __base10 = 3.321928F;
        const float __baseE = 1.442695F;

        #region Static Initializer

        static FastLog()
        {
            // Create lookup table.
            for (uint i = 0; i < __mantissaLogs.Length; i++)
            {
                var n = new Ieee754 { UnsignedBits = i | 0x3F800000 }; //added the implicit 1 leading bit
                __mantissaLogs[i] = (float)Math.Log(n.Single, 2);
            }
        }

        #endregion

        #region Public Static Methods

        public static float Log2(float value)
        {
            if (value == 0f) {
                return float.NegativeInfinity;
            }

            var number = new Ieee754 { Single = value };

            if (number.UnsignedBits >> 31 == 1) 
            {   //NOTE: didn't call Sign property for higher performance
                return float.NaN;
            }

            return (((number.SignedBits >> 23) & 0xFF) - 127) + __mantissaLogs[number.UnsignedBits & 0x007FFFFF];
            //NOTE: didn't call Exponent and Mantissa properties for higher performance
        }

        public static float Log10(float value)
        {
            return Log2(value) / __base10;
        }

        public static float Ln(float value)
        {
            return Log2(value) / __baseE;
        }

        public static float Log(float value, float valueBase)
        {
            return Log2(value) / Log2(valueBase);
        }

        #endregion
    }
}
