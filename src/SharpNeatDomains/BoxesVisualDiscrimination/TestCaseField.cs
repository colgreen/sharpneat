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
using SharpNeat.Utility;

namespace SharpNeat.Domains.BoxesVisualDiscrimination
{
    /// <summary>
    /// Represents test cases for the Boxes visual discrimination task. The test case field is fixed at a resolution of 11x11
    /// the visual field of the agents being evaluated on teh task can have a variable visual field resolution - the visual 
    /// field pixels sample the 11x11 pixels in the test field.
    /// </summary>
    public class TestCaseField
    {
        /// <summary>Resolution of the test field pixel grid.</summary>
        public const int TestFieldResolution = 11;
        const int CoordBoundIdx = TestFieldResolution - 1;
        const int TestFieldPixelCount = TestFieldResolution * TestFieldResolution;

        IntPoint _smallBoxTopLeft;
        IntPoint _largeBoxTopLeft;

        FastRandom _rng;

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TestCaseField()
        {
            _rng = new FastRandom();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// (Re)initialize with a fresh test case.
        /// Returns the target point (center of large box).
        /// </summary>
        public IntPoint InitTestCase(int largeBoxRelativePos)
        {
            // Get small and large box center positions.
            IntPoint[] boxPosArr = GenerateRandomTestCase(largeBoxRelativePos);
            _smallBoxTopLeft = boxPosArr[0];
            _largeBoxTopLeft = boxPosArr[1];
            _largeBoxTopLeft._x--;
            _largeBoxTopLeft._y--;
            return boxPosArr[1];
        }

        /// <summary>
        /// Gets the value of the pixel at a position in the 'real/sensor' coordinate space (continuous x and y, -1 to 1).
        /// </summary>
        public double GetPixel(double x, double y)
        {
            // Quantize real position to test field pixel coords.
            int pixelX = (int)(((x + 1.0) * TestFieldResolution) / 2.0);
            int pixelY = (int)(((y + 1.0) * TestFieldResolution) / 2.0);

            // Test for intersection with small box pixel.
            if(_smallBoxTopLeft._x == pixelX && _smallBoxTopLeft._y == pixelY) {
                return 1.0;
            }

            // Test for intersection with large box pixel.
            int deltaX = pixelX - _largeBoxTopLeft._x;
            int deltaY = pixelY - _largeBoxTopLeft._y;
            return (deltaX > -1 && deltaX < 3 && deltaY > -1 && deltaY < 3) ? 1.0 : 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the coordinate of the small box (the small box occupies a single pixel).
        /// </summary>
        public IntPoint SmallBoxTopLeft
        {
            get { return _smallBoxTopLeft; }
            set { _smallBoxTopLeft = value; }
        }

        /// <summary>
        /// Gets the coordinate of the large box's top left pixel.
        /// </summary>
        public IntPoint LargeBoxTopLeft
        {
            get { return _largeBoxTopLeft; }
            set { _largeBoxTopLeft = value; }
        }

        #endregion

        #region Private Methods

        private IntPoint[] GenerateRandomTestCase(int largeBoxRelativePos)
        {
            // Randomly select a position for the small box (the box is a single pixel in size).
            IntPoint smallBoxPos = new IntPoint(_rng.Next(TestFieldResolution), _rng.Next(TestFieldResolution));
            
            // Large box center is 5 pixels to the right, down or diagonally from the small box.
            IntPoint largeBoxPos = smallBoxPos;
            switch(largeBoxRelativePos)
            {
                case 0: // Right
                    largeBoxPos._x += 5;
                    break;
                case 1: // Down
                    largeBoxPos._y += 5;
                    break;
                case 2: // Diagonal
                    // Two alternate position get us to exactly 5 pixels distant from the small box.
                    if(_rng.NextBool())
                    {
                        largeBoxPos._x += 3;
                        largeBoxPos._y += 4;
                    }
                    else
                    {
                        largeBoxPos._x += 4;
                        largeBoxPos._y += 3;
                    }
                    break;
            }

            // Handle cases where the large box is outside the visual field or overlapping the edge.
            if(largeBoxPos._x > CoordBoundIdx) 
            {   // Wrap around.
                largeBoxPos._x -= TestFieldResolution;

                if(0 == largeBoxPos._x)
                {   // Move box fully into the visual field.
                    largeBoxPos._x++;
                }
            }
            else if(CoordBoundIdx == largeBoxPos._x)
            {   // Move box fully into the visual field.
                largeBoxPos._x--;
            }
            else if(0 == largeBoxPos._x)
            {   // Move box fully into the visual field.
                largeBoxPos._x++;
            }


            if(largeBoxPos._y > CoordBoundIdx) 
            {   // Wrap around.
                largeBoxPos._y -= TestFieldResolution;

                if(0 == largeBoxPos._y)
                {   // Move box fully into the visual field.
                    largeBoxPos._y++;
                }
            }
            else if(CoordBoundIdx == largeBoxPos._y)
            {   // Move box fully into the visual field.
                largeBoxPos._y--;
            }
            else if(0 == largeBoxPos._y)
            {   // Move box fully into the visual field.
                largeBoxPos._y++;
            }
            return new IntPoint[] {smallBoxPos, largeBoxPos};
        }

        #endregion
    }
}
