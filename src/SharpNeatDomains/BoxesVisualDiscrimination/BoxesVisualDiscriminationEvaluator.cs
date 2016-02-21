using System;
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
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.BoxesVisualDiscrimination
{
    /// <summary>
    /// Boxes Visual Discrimination Task.
    /// </summary>
    public class BoxesVisualDiscriminationEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        /// <summary>
        /// Width and length of the visual field in the 'real' coordinate system that 
        /// substrate nodes are located within (and therefore sensor and output pixels).
        /// </summary>
        public const double VisualFieldEdgeLength = 2.0;
        /// <summary>
        /// The root mean square distance (rmsd) between two random points in the unit square.
        /// An agent that attempts this problem domain by selecting random points will produce this value as a score 
        /// when the size of the visual field is 1x1 (the unit square). For other visual field sizes we can obtain the
        /// random agent's score  by simply multiplying this value by the edge length of the visual field (the score scales 
        /// linearly with the edge length).
        ///
        /// This value can be derived starting with the function for the mean length of a line between two random points
        /// in the unit square, as given in: http://mathworld.wolfram.com/SquareLinePicking.html
        /// 
        /// Alternatively the value can be experimentally determined/approximated. The value here was found computationally.
        /// </summary>
        const double MeanLineInSquareRootMeanSquareLength = 0.5773;
        /// <summary>
        /// Maximum fitness for this evaulator. Problem domain is considered perfectly 'solved' if this score is achieved.
        /// </summary>
        const double MaxFitness = 110.0;
        /// <summary>
        /// The resoltion of the visual and output fields.
        /// </summary>
        int _visualFieldResolution;
        /// <summary>
        /// The width and height of a visual field pixel in the real coordinate system.
        /// </summary>
        double _visualPixelSize;
        /// <summary>
        /// The X and Y position of the visual field's origin pixel in the real coordinate system (the center position of the origin pixel).
        /// </summary>
        double _visualOriginPixelXY;
        /// <summary>
        /// Number of evaluations.
        /// </summary>
        ulong _evalCount;
        /// <summary>
        /// Indicates if some stop condition has been achieved.
        /// </summary>
        bool _stopConditionSatisfied;

        #region Public Static Methods

        /// <summary>
        /// Apply the provided test case to the provided black box's inputs (visual input field).
        /// </summary>
        public static void ApplyVisualFieldToBlackBox(TestCaseField testCaseField, IBlackBox box, int visualFieldResolution, double visualOriginPixelXY, double visualPixelSize)
        {
            int inputIdx = 0;
            double yReal = visualOriginPixelXY;
            for(int y=0; y<visualFieldResolution; y++, yReal += visualPixelSize)
            {
                double xReal = visualOriginPixelXY;
                for(int x=0; x<visualFieldResolution; x++, xReal += visualPixelSize, inputIdx++)
                {
                    box.InputSignalArray[inputIdx] = testCaseField.GetPixel(xReal, yReal);
                }
            }
        }

        /// <summary>
        /// Determine the coordinate of the pixel with the highest activation.
        /// </summary>
        public static IntPoint FindMaxActivationOutput(IBlackBox box, int visualFieldResolution, out double minActivation, out double maxActivation)
        {
            minActivation = maxActivation = box.OutputSignalArray[0];
            int maxOutputIdx = 0;

            int len = box.OutputSignalArray.Length;
            for(int i=1; i<len; i++)
            {
                double val = box.OutputSignalArray[i];

                if(val > maxActivation)
                {
                    maxActivation = val;
                    maxOutputIdx = i;
                } 
                else if(val < minActivation)
                {
                    minActivation = val;
                }
            }

            int y = maxOutputIdx / visualFieldResolution;
            int x = maxOutputIdx - (y * visualFieldResolution);
            return new IntPoint(x, y);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the specified sensor and output pixel array resolution.
        /// </summary>
        /// <param name="visualFieldResolution"></param>
        public BoxesVisualDiscriminationEvaluator(int visualFieldResolution)
        {
            _visualFieldResolution = visualFieldResolution;
            _visualPixelSize = VisualFieldEdgeLength / _visualFieldResolution;
            _visualOriginPixelXY = -1.0 + (_visualPixelSize/2.0);
        }

        #endregion

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox against the XOR problem domain and return its fitness score.
        /// 
        /// Fitness value explanation.
        /// 1) Max distance from target position in each trial is sqrt(2)*VisualFieldEdgeLength (target in one corner and selected target in 
        /// opposite corner). 
        ///
        /// 2) An agents is scored by squaring the distance of its selected target from the actual target, squaring the value,
        /// taking the average over all test cases and then taking the square root. This is referred to as the root mean squared distance (RMSD) 
        /// and is effectively an  implementation of least squares (least squared error). The square root term converts values back into units 
        /// of distance (rather than squared distance)
        ///
        /// 3) An agent selecting points at random will score VisualFieldEdgeLength * MeanLineInSquareRootMeanSquareLength. Any agent scoring 
        /// this amount or less is assigned a fitness of zero. All other scores are scaled and translated into the range 0-100 where 0 is no better
        /// or worse than a random agent, and 100 is perfectly selecting the correct target for all test cases (distance of zero between target and 
        /// selected target).
        /// 
        /// 4)  In addition to this the range of output values is scaled to 0-10 and added to the final score, this encourages solutions with a wide
        /// output range between the highest activation (the selected pixel) and the lowest activation (this encourages prominent/clear selection).
        ///
        /// An alternative scheme is fitness = 1/RMSD  (separately handling the special case where RMSD==0).
        /// However, this gives a non-linear increase in fitness as RMSD decreases linearly, which in turns produces a 'spikier' fitness landscape
        /// which is more likely to cause genomes and species to get caught in a local maximum.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;

            // Accumulate square distance from each test case.
            double acc = 0.0;
            double activationRangeAcc = 0.0;
            TestCaseField testCaseField = new TestCaseField();
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<25; j++) {
                    double activationRange;
                    acc += RunTrial(box, testCaseField, i, out activationRange);
                    activationRangeAcc += activationRange;
                }
            }

            // Calc root mean squared distance (RMSD) and calculate fitness based comparison to the random agent.
            const double threshold = VisualFieldEdgeLength * 0.5772;
            double rmsd = Math.Sqrt(acc / 75.0);
            double fitness;
            if(rmsd > threshold) {
                fitness = 0.0;
            } else {
                fitness = (((threshold-rmsd) * 100.0) / threshold) + (activationRangeAcc / 7.5);
            }

            // Set stop flag when max fitness is attained.
            if(!_stopConditionSatisfied && fitness == MaxFitness) {
                _stopConditionSatisfied = true;
            }
            return new FitnessInfo(fitness, rmsd);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// Note. The XOR problem domain has no internal state. This method does nothing.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Run a single trial
        /// 1) Generate random test case with the box orientation specified by largeBoxRelativePos.
        /// 2) Apply test case visual field to black box inputs.
        /// 3) Activate black box.
        /// 4) Determine black box output with highest output, this is the selected pixel.
        /// 
        /// Returns square of distance between target pixel (center of large box) and pixel selected by the black box.
        /// </summary>
        private double RunTrial(IBlackBox box, TestCaseField testCaseField, int largeBoxRelativePos, out double activationRange)
        {
            // Generate random test case. Also gets the center position of the large box.
            IntPoint targetPos = testCaseField.InitTestCase(0);

            // Apply test case visual field to black box inputs.
            ApplyVisualFieldToBlackBox(testCaseField, box, _visualFieldResolution, _visualOriginPixelXY, _visualPixelSize);

            // Clear any pre-existign state and activate.
            box.ResetState();
            box.Activate();
            if(!box.IsStateValid) 
            {   // Any black box that gets itself into an invalid state is unlikely to be
                // any good, so lets just bail out here.
                activationRange = 0.0;
                return 0.0;
            }

            // Find output pixel with highest activation.
            double minActivation, maxActivation;
            IntPoint highestActivationPoint = FindMaxActivationOutput(box, _visualFieldResolution, out minActivation, out maxActivation);
            activationRange = Math.Max(0.0, maxActivation - minActivation);

            // Get the distance between the target and activated pixels, in the real coordinate space.
            // We actually want squared distance (not distance) thus we can skip taking the square root (expensive CPU operation).
            return CalcRealDistanceSquared(targetPos, highestActivationPoint);
        }

        private double CalcRealDistanceSquared(IntPoint a, IntPoint b)
        {
            // We can skip calculating abs(val) because we square the values.
            double xdelta = (a._x - b._x) * _visualPixelSize;
            double ydelta = (a._y - b._y) * _visualPixelSize;
            return xdelta*xdelta + ydelta*ydelta;
        }

        #endregion
    }
}
