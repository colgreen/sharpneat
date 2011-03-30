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
namespace SharpNeat.Utility
{
    /// <summary>
    /// Static methods for roulette wheel selection from a set of choices with predefined probabilities.
    /// </summary>
    public static class RouletteWheel
    {
        /// <summary>
        /// A simple single throw routine.
        /// </summary>
        /// <param name="probability">A probability between 0..1 that the throw will result in a true result.</param>
        /// <param name="rng">Random number generator.</param>
        public static bool SingleThrow(double probability, FastRandom rng)
        {
            return rng.NextDouble() < probability;
        }
        
        /// <summary>
        /// Performs a single throw for a given number of outcomes with equal probabilities.
        /// </summary>
        /// <param name="numberOfOutcomes">The number of possible outcomes.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>An integer between 0..numberOfOutcomes-1. In effect this routine selects one of the possible outcomes.</returns>
        public static int SingleThrowEven(int numberOfOutcomes, FastRandom rng)
        {
            return (int)(rng.NextDouble() * numberOfOutcomes);
        }

        /// <summary>
        /// Performs a single throw onto a roulette wheel where the wheel's space is unevenly divided between outcomes.
        /// The probabilty that a segment will be selected is given by that segment's value in the 'probabilities'
        /// array within the specified RouletteWheelLayout. The probabilities within a RouletteWheelLayout have already 
        /// been normalised so that their total is always equal to 1.0.
        /// </summary>
        /// <param name="layout">The roulette wheel layout.</param>
        /// <param name="rng">Random number generator.</param>
        public static int SingleThrow(RouletteWheelLayout layout, FastRandom rng)
        {
            // Throw the ball and return an integer indicating the outcome.
            double throwValue = rng.NextDouble();
            double accumulator = 0.0;

            for(int i=0; i<layout.Probabilities.Length; i++)
            {
                accumulator += layout.Probabilities[i];
                if(throwValue < accumulator) {
                    return i;
                }
            }

            // We might get here through floating point arithmetic rounding issues. 
            // e.g. accumulator == throwValue. 

            // Find a nearby non-zero probability to select.
            // Wrap around to start of array.
            for(int i=0; i<layout.Probabilities.Length; i++)
            {
                if(layout.Probabilities[i] != 0.0) {
                    return i;
                }
            }

            // If we get here then we have an array of zero probabilities.
            throw new SharpNeatException("Invalid operation. No non-zero probabilities to select.");
        }

        /// <summary>
        /// Performs a single throw onto a roulette wheel where the wheel's space is unevenly divided between outcomes.
        /// The probabilty that a segment will be selected is given by that segment's value in the 'probabilities'
        /// array within the specified RouletteWheelLayout. The probabilities within a RouletteWheelLayout have already 
        /// been normalised so that their total is always equal to 1.0.
        /// 
        /// This overload of SingleThrow() allows one of the segments of the roulette wheel to be eliminated. The item at
        /// skipIdx will not be selected. As such there must be at least one other non-zero probability item in addition to
        /// the one being avoided/skipped; if there is not then -1 is returned to indicate that no item was selected.
        /// </summary>
        /// <param name="layout">The roulette wheel layout.</param>
        /// <param name="rng">Random number generator.</param>
        /// <param name="skipIdx">The index of the probability/item to avoid.</param>
        public static int SingleThrow(RouletteWheelLayout layout, FastRandom rng, int skipIdx)
        {
            // Consider the roulette wheel with a segment missing (skipIdx).
            // Subtract the skipped items probability from 1.0.
            double totalProb = 1.0 - layout.Probabilities[skipIdx];

            // Throw the ball and return an integer indicating the outcome.
            double throwValue = rng.NextDouble() * totalProb;
            double accumulator = 0.0;

            // Check all items before skipIdx.
            for(int i=0; i<skipIdx; i++)
            {
                accumulator += layout.Probabilities[i];
                if(accumulator > throwValue) {
                    return i;
                }
            }

            // Check all items after skipIdx.
            for(int i=skipIdx+1; i<layout.Probabilities.Length; i++)
            {
                accumulator += layout.Probabilities[i];
                if(accumulator > throwValue) {
                    return i;
                }
            }

            // We might get here through floating point arithmetic rounding issues. 
            // e.g. accumulator == throwValue. 

            // Find a nearby non-zero probability to select.
            // Wrap around to start of array.
            for(int i=0; i<skipIdx; i++)
            {
                if(layout.Probabilities[i] != 0.0) {
                    return i;
                }
            }
            for(int i=skipIdx+1; i<layout.Probabilities.Length; i++)
            {
                if(layout.Probabilities[i] != 0.0) {
                    return i;
                }
            }

            // If we get here then we have an array of zero probabilities apart from the skipIdx item.
            // Return -1 to indicate there is no selectable item with a non-zero probability.
            return -1;
        }
    }
}
