using System;
using System.Collections;
using SharpNeatLib.Maths;

namespace SharpNeatLib.Experiments
{

	public class PTSPUtilities
	{
		static FastRandom random = new FastRandom();

		public static ShortCoord[] GenerateRoute(ShortCoord[] masterCoords, int[] sequence)
		{
			int length = Math.Min(masterCoords.Length, sequence.Length);
			
			ShortCoord[] route = new ShortCoord[length];

			for(int i=0; i<length; i++)
				route[i]=masterCoords[sequence[i]];

			return route;
		}

		public static Vector2d[] GenerateRoute_Vector2d(ShortCoord[] masterCoords, int[] sequence)
		{
			int length = Math.Min(masterCoords.Length, sequence.Length);
			
			Vector2d[] route = new Vector2d[length];

			for(int i=0; i<length; i++)
			{
				route[i].x=(double)masterCoords[sequence[i]].X;
				route[i].y=(double)masterCoords[sequence[i]].Y;
			}

			return route;
		}

		public static ShortCoord[] GenerateRandomRoute(ShortCoord[] masterCoords)
		{
			int length = masterCoords.Length;
			int[] rndSequence = GenerateRandomSequence(length);
			ShortCoord[] rndRoute = new ShortCoord[length];

			for(int i=0; i<length; i++)
				rndRoute[i]=masterCoords[rndSequence[i]];

			return rndRoute;
		}

		private static int[] GenerateRandomSequence(int length)
		{
			ArrayList sequenceIndexes = new ArrayList(length);
			for(int i=0; i<length; i++)
			{
				sequenceIndexes.Add(i);
			}

			// Keep picking entries at random until sequenceIndexes is empty.
			int[] rndSequence = new int[length];
			int remaining = length;
			int j=0;
			
			do
			{
				int index = random.Next(remaining);
				rndSequence[j++] = (int)sequenceIndexes[index];
				sequenceIndexes.RemoveAt(index);
			}
			while(--remaining>0);

			return rndSequence;
		}

		public static float CalculateRouteLength(ShortCoord[] route)
		{
			if(route.Length < 2)
				return 0.0F;

			float totalLength = 0.0F;
			ShortCoord prevCoord = route[0];

			for(int i=1; i<route.Length; i++)
			{
				ShortCoord coord = route[i];

				float x_delta = (coord.X-prevCoord.X);
				float y_delta = (coord.Y-prevCoord.Y);

				totalLength += (float)Math.Sqrt(x_delta*x_delta + y_delta*y_delta);
			}

			return totalLength;
		}
	}
}
