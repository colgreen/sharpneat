using System;
using System.Linq;
using Xunit;
using static SharpNeat.Tests.ArrayTestUtils;

namespace SharpNeat.BlackBox.Tests
{
    public class VectorSegmentTests
    {
        #region Test Methods [Segment Tests]

        [Fact]
        public void SimpleSegment1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 5, 6, 7 };
            ConponentwiseEqual(expectedArr, vecSeg);
        }

        [Fact]
        public void SimpleSegment2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 0, 10);

            // Test the segment yields the expected values.
            int[] expectedArr = Enumerable.Range(0, 10).ToArray();
            ConponentwiseEqual(expectedArr, vecSeg);
        }

        [Fact]
        public void InvalidSegments()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();

            // Test constructor throws exceptions for invalid ranges over the inner array.
            Assert.Throws<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 8, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 9, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 0, 11));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, -1, 1));
        }

        #endregion

        #region Test Methods [CopyTo Tests]

        [Fact]
        public void CopyToTest1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];
            vecSeg.CopyTo(tgtArr, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Equal(expectedArr, tgtArr);
        }

        [Fact]
        public void CopyToTest2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];
            vecSeg.CopyTo(tgtArr, 2, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Equal(expectedArr, tgtArr);
        }

        [Fact]
        public void CopyToTest3()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];
            vecSeg.CopyTo(tgtArr, 2, 1, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Equal(expectedArr, tgtArr);
        }

        [Fact]
        public void CopyToTest_InvalidCopyOperations()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];

            //--- Two param tests.
            // Copy beyond end of tgtArr.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 10));

            // Invalid target index.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, -1));

            //--- Three param tests.
            // Copy length longer then vecSeg length.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 0, 4));

            // Invalid target index.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, 0, -1));

            // Copy beyond end of tgtArr.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 11, 2));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 12, 1));

            //--- Four param tests.
            // Copy beyond end of vecSeg.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 0, 1, 3));

            // Copy beyond end of tgtArr.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 11, 1, 2));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 12, 1, 1));

            // Invalid source and target indexes.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, -1, 0, 1));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 0, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, 0, 1, -1));
        }

        #endregion

        #region Test Methods [CopyFrom Tests]

        [Fact]
        public void CopyFromTest1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();
            vecSeg.CopyFrom(srcArr, 0);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 100, 101, 102, 8, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 2).ToArray();
            vecSeg.CopyFrom(srcArr, 1);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 5, 100, 101, 8, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest3()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 2).ToArray();
            vecSeg.CopyFrom(srcArr, 1, 1);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 5, 100, 7, 8, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest4()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 10).ToArray();
            vecSeg.CopyFrom(srcArr, 5, 1, 2);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 5, 105, 106, 8, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest_InvalidCopyOperations()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 4);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();

            //--- Two param tests.
            // Copy beyond end of vecSeg.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 2));

            // Invalid target index.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, -1));

            //--- Three param tests.

            // Copy length longer than srcArr.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 4));

            // Invalid source index.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyFrom(srcArr, 0, -1));

            // Copy beyond the end of vecSeg.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 2, 3));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 3, 2));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 4, 1));

            // Four param tests.

            // Copy beyond end of srcArr.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 0, 4));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 1, 0, 3));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 2, 0, 2));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 3, 0, 1));

            // Copy beyond the end of vecSeg.
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 3, 2));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 2, 3));

            // Invalid source and target indexes.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyFrom(srcArr, -1, 0, 1));
            Assert.Throws<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentOutOfRangeException>(() => vecSeg.CopyFrom(srcArr, 0, 0, -1));
        }

        #endregion
    }
}
