using System;
using System.Collections;

namespace SharpNeatLib.Experiments
{
	public class PreyCaptureTestCase
	{
		private int xAgent, yAgent;
		private int xPrey, yPrey;
		private int randomSeed;
		
		#region Constructor

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="testCase"></param>
		public PreyCaptureTestCase(PreyCaptureTestCase testCase)
		{
			this.xAgent = testCase.xAgent;
			this.yAgent = testCase.yAgent;
			this.xPrey = testCase.xPrey;
			this.yPrey = testCase.yPrey;
			this.randomSeed = testCase.randomSeed;
		}

		public PreyCaptureTestCase(int xAgent, int yAgent, int xPrey, int yPrey, int randomSeed)
		{
			this.xAgent = xAgent;
			this.yAgent = yAgent;
			this.xPrey = xPrey;
			this.yPrey = yPrey;
			this.randomSeed = randomSeed;			
		}

		#endregion

		#region Properties

		public int XAgent
		{
			get
			{
				return xAgent;
			}
		}

		public int YAgent
		{
			get
			{
				return yAgent;
			}
		}

		public int XPrey
		{
			get
			{
				return xPrey;
			}
		}

		public int YPrey
		{
			get
			{
				return yPrey;
			}
		}

		public int RandomSeed
		{
			get
			{
				return randomSeed;
			}
		}

		#endregion

		#region Public Methods

		public override bool Equals(object obj)
		{
			PreyCaptureTestCase pct = (PreyCaptureTestCase)obj;

			return (xAgent==pct.xAgent) &&
					(yAgent==pct.yAgent) &&
					(xPrey==pct.xPrey) &&
					(yPrey==pct.yPrey) && 
					(randomSeed==pct.randomSeed);
		}

		public override int GetHashCode()
		{
			int a = (xAgent ^ ((yAgent>>16) + (yAgent<<16)));
			int b = (xPrey ^ ((yPrey>>16) + (yPrey<<16)));

			return (a^(b<<24) + ((b<<8)&0x00FF0000) + ((b>>8)&0x0000FF00) + (b>>24)) & randomSeed;
		}

		#endregion
	}
}
