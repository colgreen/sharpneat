using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Core
{
    public interface IPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Evaluate the provided phenome and return its fitness score.
        /// </summary>
        /// <returns>A fitness score or scores for the phenome.</returns>/returns>
        FitnessInfo Evaluate(TPhenome phenome);

        /// <summary>
        /// Test for evaulator stopping condition, i.e. is the provided fitness score 
        /// the optimal score (cannot be improved further).
        /// 
        /// Simply return false to keep evolution running indefinitely.
        /// </summary>
        /// <param name="fitnessArr">The fitness array to test.</param>
        /// <returns>True if the fitness score meets stopping condition.</returns>
        bool TestStoppingCondition(double[] fitnessArr);
    }
}
