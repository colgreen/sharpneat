using System;
using System.Threading;
using Redzen;

namespace SharpNeat.Evaluation
{
    internal sealed class EvaluationStateObjectPool
    {
        #region Instance Fields

        readonly uint _actualPoolSize;
        readonly object[] _stateObjArr;
        readonly SemaphoreSlim _semaphore;
        // Round robin accumulator.
        int _roundRobinAcc = 0;

        #endregion

        #region Constructor

        public EvaluationStateObjectPool(int minPoolSize, Func<object> factoryFn)
        {
            if(minPoolSize < 1) {
                throw new ArgumentException("Must be at least 1.", nameof(minPoolSize));
            }

            // The actual pool size is required to be a power of two, thus the actual level is chosen
            // to be the nearest power of two that is greater than or equal to minPoolSize.
            int actualPoolSize = MathUtils.CeilingToPowerOfTwo(minPoolSize);
            _actualPoolSize = (uint)actualPoolSize;

            // Pre-construct the required number of evaluation state objects.
            _stateObjArr = new object[actualPoolSize];
            for(int i=0; i < actualPoolSize; i++) {
                _stateObjArr[i] = factoryFn();
            }

            // Create a semaphore that will allow N pool objects be in-flight at a given point in time, and no more.
            _semaphore = new SemaphoreSlim(actualPoolSize, actualPoolSize);
        }

        #endregion

        #region Public Methods

        public object GetEvaluationStateObject()
        {
            // FIXME: This could return a state object that is still in use, i.e. the next object on the round robin list may not have been released yet.

            // Wait (block calling thread) if all of the pool objects have been issued.
            // In reality this method is called by the Parallel.ForEach local state initialisation routine
            // and the number of these running concurrently is limited by the ParallelOptions.MaxDegreeOfParallelism
            // setting. Thus if the pool size is at least {MaxDegreeOfParallelism} then the semaphore should never block.
            _semaphore.Wait();

            // Rotate through _stateObjArr array.
            // Notes.
            // We are inside the semaphore gated critical section, but there can still be multiple 
            // concurrent threads executing here, thus Interlocked is used as a cheap/fast way to 
            // synchronise access to _roundRobinAcc
            //
            // _actualPoolSize is required to be a power of two, so that the modulus result cycles 
            // through the pool indexes without jumping when _roundRobinAcc transitions from 
            // 0xffff_ffff to 0x0000_0000.
            // Note. The modulus operation is generally expensive to compute; here a much cheaper/faster
            // alternative method can be used because _actualPoolSize is guaranteed to be a power
            // of two.
            uint idx = ((uint)Interlocked.Increment(ref _roundRobinAcc)) & (_actualPoolSize-1);

            // Return a reference to the selected pool object.
            return _stateObjArr[idx];
        }

        public void ReleaseEvaluationStateObject()
        {
            // Increment the semaphore counter.
            _semaphore.Release();
        }

        #endregion
    }
}
