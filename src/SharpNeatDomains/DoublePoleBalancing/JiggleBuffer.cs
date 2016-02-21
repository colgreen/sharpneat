/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;

namespace SharpNeat.Domains.DoublePoleBalancing
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
		double[] _buffer;	// The buffer.
		double _jiggleTotal = 0.0;

		// The index of the previously enqueued item. -1 if buffer is empty.
		int _headIdx;

		// The index of the next item to be dequeued. -1 if buffer is empty.
		int _tailIdx;

		#region Constructors

        /// <summary>
        /// Construct buffer with the specified capacity.
        /// </summary>
		public JiggleBuffer(int size)
		{
			_buffer = new double[size];
			_headIdx = _tailIdx=-1;
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
				if(_headIdx==-1) 
					return 0;

				if(_headIdx>_tailIdx)
					return (_headIdx-_tailIdx)+1;

				if(_tailIdx>_headIdx)
					return (_buffer.Length-_tailIdx) + _headIdx+1;

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
				return _jiggleTotal;
			}
		}

		#endregion

		#region Public Methods

        /// <summary>
        /// Clear the buffer.
        /// </summary>
		public void Clear()
		{
			_headIdx = _tailIdx = -1;
			_jiggleTotal = 0.0;
		}

        /// <summary>
        /// Add an item to the front of the buffer.
        /// </summary>
		public void Enqueue(double item)
		{
			if(_headIdx==-1)
			{	// buffer is currently empty.
				_headIdx = _tailIdx = 0;
				_buffer[0] = item;
				_jiggleTotal+=item;
				return;
			}

			// Determine the index to write to.
			if(++_headIdx==_buffer.Length)
			{	// Wrap around.
				_headIdx=0;
			}

			if(_headIdx==_tailIdx)
			{	// Buffer overflow. Increment tailIdx.
				_jiggleTotal-=_buffer[_headIdx];
				if(++_tailIdx==_buffer.Length) 
				{	// Wrap around.
					_tailIdx=0;
					
				}
				_buffer[_headIdx]=item;
				_jiggleTotal+=item;
				return;
			}

			_jiggleTotal+=item;
			_buffer[_headIdx]=item;
			return;
		}

        /// <summary>
        /// Remove an item from the back of the queue.
        /// </summary>
		public double Dequeue()
		{
			if(_tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}

			double o = _buffer[_tailIdx];
			_jiggleTotal-=o;

			if(_tailIdx==_headIdx)
			{	// The buffer is now empty.
				_headIdx=_tailIdx=-1;
				return o;
			}

			if(++_tailIdx==_buffer.Length)
			{	// Wrap around.
				_tailIdx=0;
			}

			return o;
		}

        /// <summary>
        /// Pop an item from the head/top of the queue.
        /// </summary>
		public double Pop()
		{
			if(_tailIdx==-1)
			{	// buffer is currently empty.
				throw new InvalidOperationException("buffer is empty.");
			}	

			double o = _buffer[_headIdx];
			_jiggleTotal-=o;

			if(_tailIdx==_headIdx)
			{	// The buffer is now empty.
				_headIdx=_tailIdx=-1;
				return o;
			}

			if(--_headIdx==-1)
			{	// Wrap around.
				_headIdx=_buffer.Length-1;
			}

			return o;
		}

		#endregion
	}
}
