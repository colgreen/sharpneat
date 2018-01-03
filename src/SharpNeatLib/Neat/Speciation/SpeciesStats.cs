
namespace SharpNeat.Neat.Speciation
{
    public class SpeciesStats
    {
        // Real/continuous stats.
        public double MeanFitness;
        public double TargetSizeReal;

        // Integer stats.
        public int TargetSizeInt;
        public int EliteSizeInt;
        public int OffspringCount;
        public int OffspringAsexualCount;
        public int OffspringSexualCount;
      
        // Selection data.
        public int SelectionSizeInt;
    }
}
