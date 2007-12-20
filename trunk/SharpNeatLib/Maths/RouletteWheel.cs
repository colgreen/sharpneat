using System;

namespace SharpNeatLib.Maths
{
	public class RouletteWheel
	{
		static private Random random = new Random();

		/// <summary>
		/// A simple single throw routine.
		/// </summary>
		/// <param name="probability">A probability between 0..1 that the throw will result in a true result.</param>
		/// <returns></returns>
		static public bool SingleThrow(double probability)
		{
			if(random.NextDouble() <=probability)
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// Performs a single throw for a given number of outcomes with equal probabilities.
		/// </summary>
		/// <param name="numberOfOutcomes"></param>
		/// <returns>An integer between 0..numberOfOutcomes-1. In effect this routine selects one of the possible outcomes.</returns>
		static public int SingleThrowEven(int numberOfOutcomes)
		{
			double probability= 1.0 / (double)numberOfOutcomes;
			double accumulator=0;
			double throwValue = random.NextDouble();

			for(int i=0; i<numberOfOutcomes; i++)
			{
				accumulator+=probability;
				if(throwValue<=accumulator)
					return i;
			}
			throw new Exception("PeannutLib.Maths.SingleThrowEven() - invalid outcome.");
		}

		/// <summary>
		/// Performs a single thrown onto a roulette wheel where the wheel's space is unevenly divided.
		/// The probabilty that a segment will be selected is given by that segment's value in the 'probabilities'
		/// array. The probabilities are normalised before tossing the ball so that their total is always equal to 1.0.
		/// </summary>
		/// <param name="probabilities"></param>
		/// <returns></returns>
		static public int SingleThrow(double[] probabilities)
		{
			double pTotal=0;	// Total probability
			
			//----- 
			for(int i=0; i<probabilities.Length; i++)
				pTotal+=probabilities[i];

			//----- Now throw the ball and return an integer indicating the outcome.
			double throwValue = random.NextDouble() * pTotal;
			double accumulator=0;

			for(int j=0; j<probabilities.Length; j++)
			{
				accumulator+=probabilities[j];

				if(throwValue<=accumulator)
					return j;
			}
			throw new Exception("PeannutLib.Maths.SingleThrow() - invalid outcome.");
		}


		/// <summary>
		/// Similar in functionality to SingleThrow(double[] probabilities). However the 'probabilities' array is
		/// not normalised. Therefore if the total goes beyond 1 then we allow extra throws, thus if the total is 10
		/// then we perform 10 throws.
		/// </summary>
		/// <param name="probabilities"></param>
		/// <returns></returns>
		static public int[] MultipleThrows(double[] probabilities)
		{
			double pTotal=0;	// Total probability
			int numberOfThrows;
			
			//----- Determine how many throws of the ball onto the wheel.
			for(int i=0; i<probabilities.Length; i++)
				pTotal+=probabilities[i];

			// If total probabilty is > 1 then we take this as meaning more than one throw of the ball.
			double pTotalInteger = Math.Floor(pTotal);
			double pTotalRemainder = pTotal - pTotalInteger;
			numberOfThrows = (int)pTotalInteger;

			if(random.NextDouble() <= pTotalRemainder)
				numberOfThrows++;

			//----- Now throw the ball the determined number of times. For each throw store an integer indicating the outcome.
			int[] outcomes = new int[numberOfThrows];

			for(int i=0; i<numberOfThrows; i++)
			{
				double throwValue = random.NextDouble() * pTotal;
				double accumulator=0;

				for(int j=0; j<probabilities.Length; j++)
				{
					accumulator+=probabilities[j];

					if(throwValue<=accumulator)
					{
						outcomes[i] = j;
						break;
					}
				}
			}

			return outcomes;
		}
	}
}
