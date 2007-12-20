using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharpNeatLib.Experiments
{
	public class BitImageCollection : CollectionBase
	{
		SolidBrush oBrushWhite = new SolidBrush(Color.White);
		SolidBrush oBrushBlack = new SolidBrush(Color.Black);

		#region Constructors

		public BitImageCollection()
		{
		}

		public BitImageCollection(Font font, int xBase, int yBase, int widthTrim, int heightTrim, char firstChar, char lastChar)
		{
			Bitmap bitmap = new Bitmap(20,20, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bitmap);

			int charHeight = font.Height;
			int charWidth = (int)Math.Ceiling(g.MeasureString(firstChar.ToString(), font, 20, StringFormat.GenericTypographic).Width);

			for(char currChar=firstChar; currChar<=lastChar; currChar++)
			{
				g.FillRectangle(oBrushWhite, 0, 0, bitmap.Width, bitmap.Height);
				g.DrawString(currChar.ToString(), font, oBrushBlack, 0F, 0F);

				BitImage bitImage = new BitImage(bitmap, xBase, yBase, charWidth-widthTrim, charHeight-heightTrim);
				List.Add(bitImage);
			}
		}

		public BitImageCollection(Font font, int xBase, int yBase, int widthTrim, int heightTrim, char[] charArray)
		{
			Bitmap bitmap = new Bitmap(20,20, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bitmap);

			int charHeight = font.Height;
			int charWidth = (int)Math.Ceiling(g.MeasureString(charArray[0].ToString(), font, 20, StringFormat.GenericTypographic).Width);

			foreach(char currChar in charArray)
			{				
				g.FillRectangle(oBrushWhite, 0, 0, bitmap.Width, bitmap.Height);
				g.DrawString(currChar.ToString(), font, oBrushBlack, 0F, 0F);

				BitImage bitImage = new BitImage(bitmap, xBase, yBase, charWidth-widthTrim, charHeight-heightTrim);
				List.Add(bitImage);
			}
		}

		#endregion

		#region Properties

		public BitImage this[int index]
		{
			get
			{
				return ((BitImage)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		#endregion

		#region Public Methods

		public int Add(BitImage bitImage)
		{
			return (List.Add(bitImage));
		}

		#endregion
	}
}
