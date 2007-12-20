using System;

namespace SharpNeatLib
{

	public class CircularBuffer
	{
		public object[] buffer;

		// The index of the previously enqueued item. -1 if buffer is empty.
		int headIdx;

		// The index of the next item to be dequeued. -1 if buffer is empty.
		int tailIdx;

		public CircularBuffer(int capacity)
		{
			buffer = new object[capacity];
			headIdx = tailIdx=-1;
		}

		public int Capacity
		{
			get
			{
				return buffer.Length;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return headIdx==-1;
			}
		}

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
		/// Enqueue a new item onto the head of the queue, overwriting old values if the buffer overflows.
		/// </summary>
		/// <param name="item"></param>
		/// <returns>True if the buffer overflowed an an old item was overwritten</returns>
		public bool Enqueue(object item)
		{
			if(headIdx==-1)
			{	// buffer is currently empty.
				headIdx = tailIdx = 0;
				buffer[0] = item;
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
				buffer[headIdx]=item;
				return true;
			}

			buffer[headIdx]=item;
			return false;
		}

		/// <summary>
		/// Dequeue an old item from the tail of the queue.
		/// </summary>
		/// <returns>The dequeued item. Throws an exception if the buffer was empty, 
		/// check the buffer's length or IsEmpty property to avoid exceptions.</returns>
		public object Dequeue()
		{
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}

			object o = buffer[tailIdx];

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

		/// <summary>
		/// Pop an item of the head of the queue.
		/// </summary>
		/// <returns>The popped item. Throws an exception if the buffer was empty.</returns>
		public object Pop()
		{
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}	

			object o = buffer[headIdx];

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

		/// <summary>
		/// Peek at the item at the head of the queue.
		/// </summary>
		/// <returns>The item at the head of the queue. Throws an exception if the buffer was empty.</returns>
		public object Peek()
		{	
			if(tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}
			
			return buffer[headIdx];
		}
	}
}
