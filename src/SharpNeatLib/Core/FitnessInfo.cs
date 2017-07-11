

namespace SharpNeat.Core
{
    public struct FitnessInfo
    {
        /// <summary>
        /// An array of fitness scores. Most problem tasks will yield just a single fitness value, here we allow for 
        /// multiple fitness values per evaluation to allow for multiple objectives and/or auxiliary 
        /// fitness scores that are for reporting only.
        /// </summary>
        public double[] FitnessArray;

        #region Constructor

        public FitnessInfo(double[] fitnessArray)
        {
            this.FitnessArray = fitnessArray;
        }

        #endregion
    }
}
