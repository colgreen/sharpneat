using System;
using Redzen.Numerics;

namespace SharpNeat.Genomes.Neat.Reproduction.Mutation
{
    /// <summary>
    /// Connection weight mutation scheme.
    /// </summary>
    public class ConnectionMutationScheme
    {
        ConnectionMutationDescriptor[] _descriptors;
        DiscreteDistribution _dist;

        #region Constructor

        /// <summary>
        /// Construct with the provided descriptoes and associated set of probabilities; these define the relative probability of each type of
        /// mutation defined by the descriptors array.
        /// </summary>
        /// <param name="descriptors">An array of mutation desscriptors.</param>
        /// <param name="dist">A discrete distribution that defines the relative probability of each of the items in the descriptors array.</param>
        public ConnectionMutationScheme (ConnectionMutationDescriptor[] descriptors, DiscreteDistribution dist)
        {
            _descriptors = descriptors;
            _dist = dist;

            if(dist.Probabilities.Length != descriptors.Length) {
                throw new Exception("Counts don't match for descriptors array and associated probabilities array.");
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copyFrom">The object to copy.</param>
        public ConnectionMutationScheme(ConnectionMutationScheme copyFrom)
        {
            // Copy descriptor array.
            var descriptors = copyFrom._descriptors;
            _descriptors = new ConnectionMutationDescriptor[descriptors.Length];
            for(int i=0; i<descriptors.Length; i++) {
                _descriptors[i] = descriptors[i];
            }

            // Copy discrete distribution.
            _dist = new DiscreteDistribution(copyFrom._dist);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a randmply chosen descriptor, based on the selection probabilities defined at construction time.
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <returns>An <see cref="ConnectionMutationDescriptor"/>.</returns>
        public ConnectionMutationDescriptor GetDescriptor(IRandomSource rng)
        {
            int sample = _dist.Sample(rng);
            return _descriptors[sample];
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates a default connection mutation scheme.
        /// </summary>
        /// <remarks> This weight mutation scheme is equivalent to the one defined in SharpNEAT 2.x.</remarks>
        /// <returns></returns>
        public static ConnectionMutationScheme CreateDefault()
        {
            var descArr = new ConnectionMutationDescriptor[6];

            // Select a fixed number of connections; apply gaussian noise.
            descArr[0] = ConnectionMutationDescriptor.CreateSelectCountGaussianDelta(1, 0.01);
            descArr[1] = ConnectionMutationDescriptor.CreateSelectCountGaussianDelta(2, 0.01);
            descArr[2] = ConnectionMutationDescriptor.CreateSelectCountGaussianDelta(3, 0.01);

            // Select a fixed number of connections; re-initialise weights.
            descArr[3] = ConnectionMutationDescriptor.CreateSelectCountReInit(1);
            descArr[4] = ConnectionMutationDescriptor.CreateSelectCountReInit(2);
            descArr[5] = ConnectionMutationDescriptor.CreateSelectCountReInit(3);

            // Array of probabilities.
            // Note. These are based on the defaults in SharpNEAT 2.x. However the defaults in that version were not normalised (this was by error)
            // as stated, but were normalised at runtime, hence these pre-normalised probabilities are equivalent.
            var pArr = new double[] { 0.5752, 0.2869, 0.0947, 0.0144, 0.0144, 0.0144 };
            var dist = new DiscreteDistribution(pArr);

            return new ConnectionMutationScheme(descArr, dist);
        }

        #endregion
    }
}
