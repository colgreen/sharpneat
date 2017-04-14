
namespace SharpNeat.Core
{
    public class Genome : IGenome
    {
        readonly uint _id;
        readonly uint _birthGeneration;
        readonly object[] _auxObjects;
        FitnessInfo _fitnessInfo;

        #region Constructor

        public Genome(uint id, uint birthGeneration, object[] auxObjects)
        {
            _id = id;
            _birthGeneration = birthGeneration;
            _auxObjects = auxObjects;
        }

        #endregion

        #region Properties

        public uint Id => _id;
        public uint BirthGeneration => _birthGeneration;
        public object[] AuxObjects => _auxObjects;
        public FitnessInfo FitnessInfo 
        { 
            get => _fitnessInfo; 
            set => _fitnessInfo = value;
        }

        #endregion
    }
}
