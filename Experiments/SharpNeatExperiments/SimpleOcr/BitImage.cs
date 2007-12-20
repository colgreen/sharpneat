using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharpNeatLib.Experiments
{
	public class BitImage
	{
		int height;
		int width;
		BitArray[] rowArray;
		double[] signalArray;

		#region Constructors

		public BitImage(Bitmap bitmap, int xBase, int yBase, int width, int height)
		{
			this.height = height;
			this.width = width;

			rowArray = new BitArray[height];
			signalArray = new double[width*height];
			int signalIdx=0;

			for(int yIdx=0; yIdx<height; yIdx++)
			{
				
				BitArray rowData = new BitArray(width);
				rowArray[yIdx] = rowData;
	
				int y = yIdx+yBase;
				for(int xIdx=0; xIdx<width; xIdx++, signalIdx++)
				{	
					int x = xIdx+xBase;
					if(bitmap.GetPixel(x,y).ToArgb() != -1)
					{
						rowData[xIdx] = true;
						signalArray[signalIdx] = 1.0;
					}
				}
			}
		}

		#endregion
		
		#region Properties

		public int Height
		{
			get
			{
				return height;
			}
		}

		public int Width
		{
			get
			{
				return width;
			}
		}

		public double[] SignalArray
		{
			get
			{
				return signalArray;
			}
		}

		#endregion

		#region Public Methods

		public bool GetPixel(int x, int y)
		{
			return rowArray[y][x];
		}

		#endregion
	}
}
