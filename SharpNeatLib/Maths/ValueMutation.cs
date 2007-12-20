using System;

namespace SharpNeatLib.Maths
{
	/// <summary>
	/// Summary description for ValueMutation.
	/// </summary>
	public class ValueMutation
	{
		static FastRandom random = new FastRandom();


		/// <summary>
		/// Boundless mutation.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		static public double Mutate(double v, double sigma)
		{
			// Sigma=0.1 gives numbers in the range -0.5 to 0.5. 
			// Multiply by delta to adjust the mutation's scale in line with magnitude of the value.
			v+= RandLib.gennor(0, 0.015); //;0.025);
			return v;
		}

//
//		/// <summary>
//		/// Boundless mutation.
//		/// </summary>
//		/// <param name="v"></param>
//		/// <returns></returns>
//		static public double Mutate(double v, double baseValue)
//		{
//			double delta = Math.Abs(v-baseValue);
//
//			// Sigma=0.1 gives numbers in the range -0.5 to 0.5. 
//			// Multiply by delta to adjust the mutation's scale in line with magnitude of the value.
//			v+= delta*RandLib.gennor(0, 0.1);
//			return v;
//		}


		static public double Mutate(double v, double baseValue, double lowerLimit)
		{
			double delta = Math.Abs(v-baseValue);

			v+= delta*RandLib.gennor(0, 0.1);

			if(v<lowerLimit)
				v=lowerLimit + lowerLimit-v;
			return v;
		}

		static public double Mutate(double v, double baseValue, double lowerLimit, double highLimit)
		{
			double delta = Math.Abs(v-baseValue);

			v+= delta*RandLib.gennor(0, 0.1);

			while(v<lowerLimit || v>highLimit)
			{
				if(v<lowerLimit)
					v=lowerLimit + lowerLimit-v;

				if(v>highLimit)
					v=highLimit-(v-highLimit);
			}
			return v;
		}

		static public int Mutate(int v, int baseValue)
		{
			
			int delta = Math.Abs(v-baseValue);

			if(delta <= 10)
				v+= (int)Math.Round(RandLib.gennor(baseValue, 5));
			else if(delta>10 && delta<=100)
				v+= (int)Math.Round((double)delta*RandLib.gennor(0, 1));
			else// if(delta>100)
				v+= (int)Math.Round((double)delta*RandLib.gennor(0, 0.1));

			return v;
		}

		static public int Mutate(int v, int baseValue, int lowerLimit, int highLimit)
		{
			v=Mutate(v,baseValue);

			if(v<lowerLimit) 
				v=lowerLimit;
			else if(v>highLimit)
				v=highLimit;

			return v;
		}
	}
}
