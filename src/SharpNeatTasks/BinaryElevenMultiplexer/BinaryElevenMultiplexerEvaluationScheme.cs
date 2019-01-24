using System.Collections.Generic;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinaryElevenMultiplexer
{
    /// <summary>
    /// Evaluation scheme for the Binary 11-Multiplexer task.
    /// </summary>
    public class BinaryElevenMultiplexerEvaluationScheme : IPhenomeEvaluationScheme<IBlackBox<double>>
    {
        #region Properties

        /// <summary>
        /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
        /// </summary>
        /// <remarks>
        /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation 
        /// for the same genome, such as scheme is non-deterministic.
        /// </remarks>
        public bool IsDeterministic => true;

        /// <summary>
        /// Gets a fitness comparer for the scheme.
        /// </summary>
        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        /// <summary>
        /// Gets a fitness comparer for the scheme.
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo(s), i.e. to be able to 
        /// determine which is the better FitnessInfo between any two.
        /// </remarks>
        public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

        /// <summary>
        /// Indicates if the evaluators created by <see cref="Create"/> have state.
        /// </summary>
        /// <remarks>
        /// If an evaluator has no state then it is sufficient to create a single instance and to use that evaluator concurrently on multiple threads.
        /// If an evaluator has state then concurrent use requires the creation of one evaluator instance per thread.
        /// </remarks>
        public bool EvaluatorsHaveState => false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new evaluator object.
        /// </summary>
        /// <returns>A new instance of <see cref="IPhenomeEvaluator{T}"/>.</returns>
        public IPhenomeEvaluator<IBlackBox<double>> Create()
        {
            return new BinaryElevenMultiplexerEvaluator();
        }

        /// <summary>
        /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
        /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
        /// </summary>
        /// <param name="fitnessInfo">The fitness info object to test.</param>
        /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return (fitnessInfo.PrimaryFitness >= 10_000);
        }

        #endregion
    }
}
