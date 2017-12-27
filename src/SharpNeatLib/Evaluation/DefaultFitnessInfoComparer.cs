using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    public class DefaultFitnessInfoComparer : IComparer<FitnessInfo>
    {
        public static DefaultFitnessInfoComparer Singleton = new DefaultFitnessInfoComparer();

        public int Compare(FitnessInfo x, FitnessInfo y)
        {
            return x.PrimaryFitness.CompareTo(y.PrimaryFitness);
        }
    }
}
