using System;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// This is a circular buffer of double precision floating point numbers, customised
	/// for use in the double pole experiments. 
	/// 
	/// This buffer maintains a total of all of the values it contains, adjusting for
	/// values that are overwritten when the buffer overwrites old values.
	/// </summary>
	public class JiggleBuffer
	{
		double[] buffer;	// The buffer.
		double jiggleTotal = 0.0;

		// The index of the previously enqueued item. -1 if buffer is empty.
		int headIdx;

		// The index of the next item to be dequeued. -1 if buffer is empty.
		int tailIdx;

		#region Constructors

		public JiggleBuffer(int size)
		{
			buffer = new double[size];
			headIdx = tailIdx=-1;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The length of the buffer.
		/// </summary>
		public int Length
		{
			get
			{
				if(headIdx==-1) 
					return 0;

				if(headIdx>tailIdx)
					return (headIdx-tailIdx)+1;

				if(tailIdx>headIdx)
					return (buffer.Length-tailIdx) + headIdx+1;

				return 1;
			}
		}

		/// <summary>
		/// The sum of all values on in the buffer.
		/// </summary>
		public double Total
		{
			get
			{
				return jiggleTotal;
			}
		}

		#endregion

		#region Public Methods

		public void Clear()
		{
			headIdx = tailIdx = -1;
			jiggleTotal = 0.0;
		}

		public void Enqueue(double item)
		{
			if(headIdx==-1)
			{	// buffer is currently empty.
				headIdx = tailIdx = 0;
				buffer[0] = item;
				jiggleTotal+=item;
				return;
			}

			// Determine the index to write to.
			if(++headIdx==buffer.Length)
			{	// Wrap around.
				headIdx=0;
			}

			if(headIdx==tailIdx)
			{	// Buffer overflow. Increment tailIdx.
				jiggleTotal-=buffer[headIdx];
				if(++tailIdx==buffer.Length) 
				{	// Wrap around.
					tailIdx=0;
					
				}
				buffer[headIdx]=item;
				jiggleTotal+=item;
				return;
			}

			jiggleTotal+=item;
			buffer[headIdx]=item;
			return;
		}

		public double Dequeue()
		{
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}

			double o = buffer[tailIdx];
			jiggleTotal-=o;

			if(tailIdx==headIdx)
			{	// The buffer is now empty.
				headIdx=tailIdx=-1;
				return o;
			}

			if(++tailIdx==buffer.Length)
			{	// Wrap around.
				tailIdx=0;
			}

			return o;
		}

		public double Pop()
		{
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}	

			double o = buffer[headIdx];
			jiggleTotal-=o;

			if(tailIdx==headIdx)
			{	// The buffer is now empty.
				headIdx=tailIdx=-1;
				return o;
			}

			if(--headIdx==-1)
			{	// Wrap around.
				headIdx=buffer.Length-1;
			}

			return o;
		}

		#endregion
	}
}
