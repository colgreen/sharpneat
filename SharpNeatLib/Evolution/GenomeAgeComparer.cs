using System;
using System.Collections;
using SharpNeatLib.Evolution;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// Summary description for GenomeAgeComparer.
	/// </summary>
	public class GenomeAgeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			long diff = (((IGenome)x).GenomeAge - ((IGenome)y).GenomeAge);

			// Convert result to an int.
			if(diff <0)
				return -1;
			else if(diff==0)
				return 0;
			else
				return 1;
		}
	}
}
