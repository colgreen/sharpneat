using System;

namespace RudyGraph
{
	/// <summary>
	/// A circular buffer of double values. The buffer has a fixed capacity specified at construction time
	/// but the number of values in the buffer varies depending on how many values have been placed into and
	/// removed from teh buffer using the Enqueue, Push, Dequeue and Pop methods. If a value is added to 
	/// the buffer when it is already full then the oldest value on teh buffer will be overwritten.
	/// </summary>
	public class DoubleCircularBuffer
	{
		#region Class Variables

		public double[] buffer;

		// The index of the previously enqueued item. -1 if buffer is empty.
		int headIdx;

		// The index of the next item to be dequeued. -1 if buffer is empty.
		int tailIdx;

		// The lowest and highest values currentl within the buffer. These are updated as values are inserted and 
		// removed from the buffer.
		double min = 0.0;
		double max = 0.0;

		#endregion

		#region Constructor

		/// <summary>
		/// Construct a buffer with the specified capacity.
		/// </summary>
		/// <param name="capacity"></param>
		public DoubleCircularBuffer(int capacity)
		{
			buffer = new double[capacity];
			headIdx = tailIdx=-1;
		}

		#endregion

		#region properties / Indexer

		/// <summary>
		/// Gets the value at the specified index in the buffer. E.g. 0 gives the value at the tail of buffer
		/// and Length-1 the value at the head of the buffer.
		/// </summary>
		public double this[int index]
		{
			get
			{
				if(index>=Length)
					throw new IndexOutOfRangeException();

				int rebased = tailIdx + index;
				if(rebased>=buffer.Length)
					rebased -= buffer.Length;

				return buffer[rebased];
			}
		}

		/// <summary>
		/// Gets the buffer's capacity.
		/// </summary>
		public int Capacity
		{
			get
			{
				return buffer.Length;
			}
		}

		/// <summary>
		/// Gets the number of values held in the buffer.
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
		/// Indicates if the buffer is empty.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return headIdx==-1;
			}
		}

		/// <summary>
		/// The lowest value in the buffer. Returns 0 if empty.
		/// </summary>
		public double Min
		{
			get
			{
				return min;
			}
		}

		/// <summary>
		/// The highest value in the buffer. Returns 0 if empty.
		/// </summary>
		public double Max
		{
			get
			{
				return max;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clear the buffer of all values. Does not affect the buffer's capacity.
		/// </summary>
		public void Clear()
		{
			headIdx = tailIdx=-1;
		}

		/// <summary>
		/// Enqueue a new item onto the head of the queue, overwriting old values if the buffer overflows.
		/// </summary>
		/// <param name="item"></param>
		/// <returns>True if the buffer overflowed an an old item was overwritten</returns>
		public bool Enqueue(double item)
		{
			if(headIdx==-1)
			{	// buffer is currently empty.
				headIdx = tailIdx = 0;
				buffer[0] = item;
				min=max=item;
				return false;
			}

			// Determine the index to write to.
			if(++headIdx==buffer.Length)
			{	// Wrap around.
				headIdx=0;
			}

			if(headIdx==tailIdx)
			{	// Buffer overflow. Increment tailIdx.
				if(++tailIdx==buffer.Length) 
				{	// Wrap around.
					tailIdx=0;
				}

				bool minMaxScan= (buffer[headIdx]==min || buffer[headIdx]==max);
				buffer[headIdx]=item;

				if(minMaxScan)
				{
					MinMaxScan();
				}
				else
				{
					if(item<min) min=item;
					else if(item>max) max=item;
				}
				return true;
			}

			if(item<min) min=item;
			else if(item>max) max=item;

			buffer[headIdx]=item;
			return false;
		}

		/// <summary>
		/// Dequeue an old item from the tail of the queue.
		/// </summary>
		/// <returns>The dequeued item. Throws an exception if the buffer was empty, 
		/// check the buffer's length or IsEmpty property to avoid exceptions.</returns>
		public double Dequeue()
		{
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}

			double o = buffer[tailIdx];

			if(tailIdx==headIdx)
			{	// The buffer is now empty.
				headIdx=tailIdx=-1;
				min=max=0.0;
				return o;
			}

			if(++tailIdx==buffer.Length)
			{	// Wrap around.
				tailIdx=0;
			}

			if(o==min || o==max)
				MinMaxScan();
			
			return o;
		}

		/// <summary>
		/// Pop an item of the head of the queue.
		/// </summary>
		/// <returns>The popped item. Throws an exception if the buffer was empty.</returns>
		public double Pop()
		{
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}	

			double o = buffer[headIdx];

			if(tailIdx==headIdx)
			{	// The buffer is now empty.
				headIdx=tailIdx=-1;
				min=max=0.0;
				return o;
			}

			if(--headIdx==-1)
			{	// Wrap around.
				headIdx=buffer.Length-1;
			}

			if(o==min || o==max)
				MinMaxScan();

			return o;
		}

		/// <summary>
		/// Peek at the item at the head of the queue.
		/// </summary>
		/// <returns>The item at the head of the queue. Throws an exception if the buffer was empty.</returns>
		public double Peek()
		{	
			if(headIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}
			
			return buffer[headIdx];
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Update the min and max varibales by performing a scan ove rthe buffer's values.
		/// </summary>
		private void MinMaxScan()
		{
			if(headIdx==-1)
			{	// Empty buffer.
				min=max=0.0;
			}
			if(tailIdx==headIdx)
			{	// One item in buffer.
				min=max=buffer[headIdx];
			}
			else if(tailIdx<headIdx)
			{
				min=max=buffer[tailIdx];
				for(int idx=tailIdx+1; idx<=headIdx; idx++)
				{
					if(buffer[idx]<min) min=buffer[idx];
					else if(buffer[idx]>max) max=buffer[idx];
				}
			}
			else if(tailIdx>headIdx)
			{
				min=max=buffer[tailIdx];

				for(int idx=tailIdx+1; idx<buffer.Length; idx++)
				{
					if(buffer[idx]<min) min=buffer[idx];
					else if(buffer[idx]>max) max=buffer[idx];
				}

				for(int idx=0; idx<=headIdx; idx++)
				{
					if(buffer[idx]<min) min=buffer[idx];
					else if(buffer[idx]>max) max=buffer[idx];
				}
			}
		}

		#endregion
	}
}
