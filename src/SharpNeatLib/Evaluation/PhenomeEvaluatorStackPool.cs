/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using Redzen.Collections;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// A pool of phenome evaluators, in which the pool is implemented with a stack structure with thread synchronised access to the stack.
    /// </summary>
    /// <typeparam name="TPhenome">Phenome type.</typeparam>
    public sealed class PhenomeEvaluatorStackPool<TPhenome> : IPhenomeEvaluatorPool<TPhenome>
    {
        readonly IPhenomeEvaluationScheme<TPhenome> _phenomeEvaluationScheme;
        readonly LightweightStack<IPhenomeEvaluator<TPhenome>> _evaluatorStack;
        readonly object _stackLockObj = new object();

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="phenomeEvaluationScheme">Phenome evaluation scheme.</param>
        /// <param name="initialPoolSize">Initial pool size.</param>
        public PhenomeEvaluatorStackPool(
            IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme,
            int initialPoolSize)
        {
            if(!phenomeEvaluationScheme.EvaluatorsHaveState) {
                throw new InvalidOperationException("A stateless evaluation scheme does not require an evaluator pool; just use a single evaluator instance concurrently.");
            }

            _phenomeEvaluationScheme = phenomeEvaluationScheme;

            // Create the stack with the spare capacity; to avoid re-alloc overhead if we require additional capacity.
            _evaluatorStack = new LightweightStack<IPhenomeEvaluator<TPhenome>>(initialPoolSize * 2);
            
            // Pre-populate with evaluators.
            for(int i=0; i < initialPoolSize; i++) {
                _evaluatorStack.Push(phenomeEvaluationScheme.CreateEvaluator());
            }
        }

        #endregion

        #region IPhenomeEvaluatorPool

        /// <summary>
        /// Get an evaluator from the pool.
        /// </summary>
        /// <returns>An evaluator instance.</returns>
        public IPhenomeEvaluator<TPhenome> GetEvaluator()
        {
            lock(_stackLockObj)
            {
                if(_evaluatorStack.Count > 0) {
                    return _evaluatorStack.Pop();
                }

                // If the pool is empty then create a new evaluator instance; this should get released
                // back into the pool, thus increasing the total number of evaluators being managed by the pool.
                return _phenomeEvaluationScheme.CreateEvaluator();
            }
        }

        /// <summary>
        /// Releases an evaluator back into the pool.
        /// </summary>
        /// <param name="evaluator">The evaluator to release.</param>
        public void ReleaseEvaluator(IPhenomeEvaluator<TPhenome> evaluator)
        {
            lock(_stackLockObj)
            {
                _evaluatorStack.Push(evaluator);
            }
        }

        #endregion
    }
}
