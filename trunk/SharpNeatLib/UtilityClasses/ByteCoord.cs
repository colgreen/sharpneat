using System;

namespace SharpNeatLib
{
	public struct ByteCoord
	{
		public byte x;
		public byte y;

		public ByteCoord(byte x, byte y)
		{
			this.x = x;
			this.y = y;
		}

		public ByteCoord(int x, int y)
		{
			this.x = (byte)x;
			this.y = (byte)y;
		}
	}
}
