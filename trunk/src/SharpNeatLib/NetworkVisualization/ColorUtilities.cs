using System;
using System.Drawing;

namespace SharpNeatLib.NetworkVisualization
{
	public class ColorUtilities
	{
		const float oneThird = 1F/3F;
		const float twoThirds = 2F/3F;

		/// <summary>
		/// HSL values = From 0 to 1
		/// </summary>
		/// <param name="H"></param>
		/// <param name="S"></param>
		/// <param name="L"></param>
		/// <returns></returns>
		public static Color FromHls(float H, float S, float L)
		{
			int R, G, B;
			
			if ( S == 0 )                      
			{
				R = (int)(L * 255F);                      //RGB results = From 0 to 255
				G = (int)(L * 255F);
				B = (int)(L * 255F);
			}
			else
			{
				float var_1, var_2;
				if (L<0.5F) 
					var_2 = L * (1F + S);
				else
					var_2 = (L + S) - (S * L);

				var_1 = 2F * L - var_2;

				R = (int)(255F * Hue_2_RGB(var_1, var_2, H + oneThird));
				G = (int)(255F * Hue_2_RGB(var_1, var_2, H));
				B = (int)(255F * Hue_2_RGB(var_1, var_2, H - oneThird));
			} 

			return Color.FromArgb(R,G,B);
		}


		private static float Hue_2_RGB(float v1, float v2, float vH)
		{
			

			if (vH < 0F) vH += 1F;
			if (vH > 1F) vH -= 1F;
			if ( (6F * vH) < 1F ) return v1 + (v2-v1) * 6F * vH;
			if ( (2F * vH) < 1F ) return v2;
			if ( (3F * vH) < 2F ) return v1 + (v2-v1) * (twoThirds - vH) * 6F;
			return v1;
		}

		/// <summary>
		/// Blue-Red scale (blue to red). 
		/// </summary>
		/// <param name="temp">0.0=blue, 1.0=red</param>
		/// <returns></returns>
		public static Color FromBlueRedScale(float temp)
		{	
			// Trim invalid values.
			temp=(float)Math.Max(0.0, temp);
			temp=(float)Math.Min(1.0, temp);
			int red=(int)(temp*255F);
			int blue=(int)((1.0-temp)*255F);

			return Color.FromArgb(red, 0, blue);
		}
	}






}
