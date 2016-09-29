/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpNeat.Core
{
    // TODO: Delete class file. Superseded by Parallel Extensions based version.
    // ENHANCEMENT: Ultimately much of the code here can be replaced by using Parallel.For() which is part of the
    // 'parallel extensions' present in .Net 4.0. For now we use System.Threading directly to minimize any
    // difficulties compiling the code in different environments, e.g. .Net 2.0 and Mono.
    /// <summary>
    /// A concrete implementation of IGenomeListEvaluator that evaluates genomes independently of each 
    /// other and in parallel (on multiple execution threads).
    /// 
    /// Genome decoding is performed by a provided IGenomeDecoder.
    /// Phenome evaluation is performed by a provided IPhenomeEvaluator.
    /// 
    /// This class evaluates on multiple execution threads to improve performance. If the number of
    /// threads is not specified on the constructor then the value from Environment.ProcessorCount is used; 
    /// this value is the number of logical processors which includes multiple CPU cores on a single chip
    /// and also hyperthreaded(HT) cores. E.g. a quad core Intel i7 with Hyperthreading will report 8 cores.
    /// </summary>
    /// <typeparam name="TGenome"></typeparam>
    /// <typeparam name="TPhenome"></typeparam>
    public class ParallelGenomeListEvaluator<TGenome,TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
    {
        #region Instance Fields

        readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
        readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        readonly bool _enablePhenomeCaching;

        readonly WorkerThreadInfo[] _infoArr;
        readonly ManualResetEvent[] _completedWorkEventArr;
        readonly Thread[] _threadArr;
        
        // Duration in ticks of the previous evaluation call. We use this to more intelligently 
        // determine the size of work chunks allocated to each thread. -1 indicates not yet called.
        long _prevDuration = 0L;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder and IPhenomeEvaluator. 
        /// Phenome caching is enabled by default.
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        /// <param name="genomeDecoder"></param>
        /// <param name="phenomeEvaluator"></param>
        public ParallelGenomeListEvaluator( IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                            IPhenomeEvaluator<TPhenome> phenomeEvaluator)
            : this(genomeDecoder, phenomeEvaluator, true, Environment.ProcessorCount)
        { }

        /// <summary>
        /// Construct with the provided IGenomeDecoder, IPhenomeEvaluator and enablePhenomeCaching flag.
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        /// <param name="genomeDecoder"></param>
        /// <param name="phenomeEvaluator"></param>
        /// <param name="enablePhenomeCaching"></param>
        public ParallelGenomeListEvaluator( IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                            IPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                            bool enablePhenomeCaching)
            : this(genomeDecoder, phenomeEvaluator, enablePhenomeCaching, Environment.ProcessorCount)
        { }

        /// <summary>
        /// Construct with the provided IGenomeDecoder, IPhenomeEvaluator, enablePhenomeCaching flag
        /// and thread count.
        /// </summary>
        /// <param name="genomeDecoder"></param>
        /// <param name="phenomeEvaluator"></param>
        /// <param name="enablePhenomeCaching"></param>
        /// <param name="threadCount"></param>
        public ParallelGenomeListEvaluator( IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                            IPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                            bool enablePhenomeCaching,
                                            int threadCount)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _enablePhenomeCaching = enablePhenomeCaching;

            // Determine the appropriate worker-thread method.
            Action<object> workerThreadmethod;
            if(_enablePhenomeCaching) {
                workerThreadmethod = WorkerThreadMethod_Caching;
            } else {
                workerThreadmethod = WorkerThreadMethod_NonCaching;
            }

            _infoArr = new WorkerThreadInfo[threadCount];
            _completedWorkEventArr = new ManualResetEvent[threadCount];
            _threadArr = new Thread[threadCount];
            for(int i=0; i<threadCount; i++)
            {    
                // Create thread and its own info object.
                WorkerThreadInfo info = new WorkerThreadInfo();
                Thread thread = new Thread(new ParameterizedThreadStart(workerThreadmethod));
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.BelowNormal;

                // Store the thread and it's info object in arrays. 
                // Also store references to all of the created _completedWorkEvent objects in an array,
                // this allows to wait for all of them together (WaitHandle.WaitAny/All).
                _threadArr[i] = thread;
                _infoArr[i] = info;
                _completedWorkEventArr[i] = info._completedWorkEvent;

                // Start the thread and pass the info object to this invocation of the thread method.
                thread.Start(info);
            }
        }

        #endregion

        #region IGenomeListEvaluator<TGenome> Members

        /// <summary>
        /// The total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        /// <summary>
        /// Indicates to the evolution algorithm that some goal fitness has been achieved and that
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }

        /// <summary>
        /// Evaluates a list of genomes. Here we decode each genome in using the contained IGenomeDecoder
        /// and evaluate the resulting TPhenome using the contained IPhenomeEvaluator.
        /// </summary>
        /// <param name="genomeList"></param>
        public void Evaluate(IList<TGenome> genomeList)
        {
            // Assign the genome list to all workers.
            int workerCount = _infoArr.Length;
            for(int i=0; i<workerCount; i++) {
                _infoArr[i]._genomeList = genomeList;
            }

            // Determine size of work chunks.
            // Initially we evenly divide the work between workers. Break the work into two chunks per 
            // worker to reduce granularity.
            long chunkSize = genomeList.Count / (2*workerCount);
            if(0L != _prevDuration)
            {   // Try to allocate chunks of no more than 0.5 seconds clock time. This
                // helps prevent some chunks outlasting others by a wide margin (because
                // decode/evaluation time is variable depending on genome size, etc).

                // half-sec chunk size.
                long halfSecChunkSize = (int)((5000000L * genomeList.Count) / _prevDuration);
                
                // Use the half-sec chunk size if it is smaller than the chunk size calculated by 
                // evenly dividing the work.
                chunkSize = Math.Min(chunkSize, halfSecChunkSize);
            }

            // Ensure non-zero chunk size and also cast to a 32bit int.
            int chunkSizeInt = (int)Math.Max(1L, chunkSize);

            // Note the clock tick when we started.
            long startTick = DateTime.Now.Ticks;

            // Allocate work in chunks until all work is completed.
            // Allocate first round of work chunks.
            int genomeIdx = 0;
            int genomeCount = genomeList.Count;
            for(int i=0; i<workerCount && genomeIdx<genomeCount; i++)
            {
                // Assign work chunk parameters.
                WorkerThreadInfo info = _infoArr[i];
                info._startIdx = genomeIdx;
                info._endIdx = genomeIdx = Math.Min(genomeCount, genomeIdx+chunkSizeInt);

                // Reset the completed event as we will be waiting for it. Any that aren't used
                // remain reset (we don't wait for them).
                info._completedWorkEvent.Reset();

                // Signal the worker thread to do the work.
                info._awaitWorkEvent.Set();
            }

            // Keep allocating work chunks to worker threads as they become available and
            // until we run out of available work.
            while(genomeIdx < genomeCount)
            {   
                // Wait for a worker thread to signal completion (and therefore become available for more work).
                int idx = WaitHandle.WaitAny(_completedWorkEventArr);

                // Assign new work chunk parameters.
                WorkerThreadInfo info = _infoArr[idx];
                info._startIdx = genomeIdx;
                info._endIdx = genomeIdx = Math.Min(genomeCount, genomeIdx+chunkSizeInt);

                // Reset the completed event as we will be waiting for it again.
                info._completedWorkEvent.Reset();

                // Signal the worker thread to do the work.
                info._awaitWorkEvent.Set();
            }

            // All work has been allocated. Wait for all worker threads to complete.            
            WaitHandle.WaitAll(_completedWorkEventArr);

            // Keep track of how long the evaluations took in total clock time.
            _prevDuration = DateTime.Now.Ticks - startTick;

            // Reset the genome list reference in all workers. Cleaning up references helps garbage collection.
            for(int i=0; i<workerCount; i++) {
                _infoArr[i]._genomeList = null;
            }
        }

        #endregion

        #region Private Methods [WorkerThreadMethods]

        private void WorkerThreadMethod_NonCaching(object paramObj)
        {
            WorkerThreadInfo info = (WorkerThreadInfo)paramObj;

            for(;;)
            {
                // Wait indefinitely for a signal to wake up and do some work.
                info._awaitWorkEvent.WaitOne();

                // Evaluate the genomes allocated to this worker thread.
                IList<TGenome> genomeList = info._genomeList;
                int endIdx = info._endIdx;

                for(int i=info._startIdx; i<endIdx; i++)
                {
                    TGenome genome = genomeList[i];
                    TPhenome phenome = _genomeDecoder.Decode(genome);
                    if(null == phenome)
                    {   // Non-viable genome.
                        genome.EvaluationInfo.SetFitness(0.0);
                    }
                    else
                    {   
                        double fitness = _phenomeEvaluator.Evaluate(phenome);
                        genome.EvaluationInfo.SetFitness(fitness);
                    }
                }

                // Signal that we have completed the allocated work.
                info._completedWorkEvent.Set();

                // FIXME: What is the proper/clean method of exiting the thread?
            }
        }

        private void WorkerThreadMethod_Caching(object paramObj)
        {
            WorkerThreadInfo info = (WorkerThreadInfo)paramObj;

            for(;;)
            {
                // Wait indefinitely for a signal to wake up and do some work.
                info._awaitWorkEvent.WaitOne();

                // Evaluate the genomes allocated to this worker thread.
                IList<TGenome> genomeList = info._genomeList;
                int endIdx = info._endIdx;

                for(int i=info._startIdx; i<endIdx; i++)
                {
                    TGenome genome = genomeList[i];
                    TPhenome phenome = (TPhenome)genome.CachedPhenome;
                    if(null == phenome) 
                    {   // Decode the phenome and store a ref against the genome.
                        phenome = _genomeDecoder.Decode(genome);
                        genome.CachedPhenome = phenome;
                    }

                    if(null == phenome)
                    {   // Non-viable genome.
                        genome.EvaluationInfo.SetFitness(0.0);
                    }
                    else
                    {   
                        double fitness = _phenomeEvaluator.Evaluate(phenome);
                        genome.EvaluationInfo.SetFitness(fitness);
                    }
                }

                // Signal that we have completed the allocated work.
                info._completedWorkEvent.Set();

                // FIXME: What is the proper/clean method of exiting the thread?
            }
        }

        #endregion

        #region Inner Class [WorkerThreadInfo]

        class WorkerThreadInfo
        {
            public int _startIdx;
            public int _endIdx;
            public IList<TGenome> _genomeList;
            public readonly AutoResetEvent _awaitWorkEvent = new AutoResetEvent(false);
            public readonly ManualResetEvent _completedWorkEvent = new ManualResetEvent(true);
        }

        #endregion
    }
}
