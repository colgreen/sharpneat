using System;
using System.Diagnostics;
using Redzen.Numerics;
using SharpNeat.Utils;

namespace SharpNeat.Genomes.Neat.Reproduction.Mutation
{
    /// <summary>
    /// Describes a specific form of weight mutation; i.e. the types of selection and mutation and any associated
    /// parameters.
    /// </summary>
    public class ConnectionMutationDescriptor
    {
        #region Auto Properties [Top Level Settings]

        /// <summary>
        /// Defines how connections are selected.
        /// </summary>
        public ConnectionSelectionType ConnectionSelectionType { get; private set; }

        /// <summary>
        /// The type of connection weight mutation to apply.
        /// </summary>
        public WeightMutationType WeightMutationType { get; private set; }

        #endregion

        #region Auto Properties [Connection Selection Settings]

        /// <summary>
        /// The proportion of connections to select. Relevant to ConnectionSelectionType.Proportion only.
        /// </summary>
        public double SelectionProportion { get; private set; }

        /// <summary>
        /// The number of connections to select. Relevant to ConnectionSelectionType.Count only.
        /// </summary>
        public double SelectionCount { get; private set; }

        #endregion

        #region Auto Properties [Weight Mutation Settings]

        /// <summary>
        /// A function that returns random samples from a weight delta distribution. Relevant to the WeightMutationType.Delta only.
        /// </summary>
        public Func<double> WeightDeltaFunc { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionMutationDescriptor()
        { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ConnectionMutationDescriptor(ConnectionMutationDescriptor copyFrom)
        {
            this.ConnectionSelectionType = copyFrom.ConnectionSelectionType;
            this.WeightMutationType = copyFrom.WeightMutationType;
            this.SelectionProportion = copyFrom.SelectionProportion;
            this.SelectionCount = copyFrom.SelectionCount;
            this.WeightDeltaFunc = copyFrom.WeightDeltaFunc;
        }

        #endregion

        #region Public Static Methods [Factory Methods - Mid Level]

        public static ConnectionMutationDescriptor CreateProportionGaussianDelta(double selectionProportion, double stdDev)
        {
            Debug.Assert(selectionProportion >= 0.0 && selectionProportion <= 1.0);
            Debug.Assert(stdDev > 0.0);

            var desc = new ConnectionMutationDescriptor();
            desc.ConnectionSelectionType = ConnectionSelectionType.Proportion;
            desc.SelectionProportion = selectionProportion;

            desc.WeightMutationType = WeightMutationType.Delta;
            var rng = RandomFactory.Create();
            ZigguratGaussianSampler gaussianSampler = new ZigguratGaussianSampler(rng, 0.0, stdDev);
            desc.WeightDeltaFunc = gaussianSampler.NextDouble;
            return desc;
        }

        public static ConnectionMutationDescriptor CreateNumberGaussianDelta(int selectionCount, double stdDev)
        {
            Debug.Assert(selectionCount > 0);

            var desc = new ConnectionMutationDescriptor();
            desc.ConnectionSelectionType = ConnectionSelectionType.Proportion;
            desc.SelectionCount = selectionCount;

            desc.WeightMutationType = WeightMutationType.Delta;
            var rng = RandomFactory.Create();
            ZigguratGaussianSampler gaussianSampler = new ZigguratGaussianSampler(rng, 0.0, stdDev);
            desc.WeightDeltaFunc = gaussianSampler.NextDouble;
            return desc;
        }

        #endregion

        #region Public Static Methods [Factory Methods - Low Level]

        public static ConnectionMutationDescriptor CreateProportionReInit(double selectionProportion)
        {
            Debug.Assert(selectionProportion >= 0.0 && selectionProportion <= 1.0);

            var desc = new ConnectionMutationDescriptor();
            desc.ConnectionSelectionType = ConnectionSelectionType.Proportion;
            desc.SelectionProportion = selectionProportion;
            desc.WeightMutationType = WeightMutationType.ReInit;
            return desc;
        }

        public static ConnectionMutationDescriptor CreateProportionDelta(double selectionProportion, Func<double> weightDeltaFunc)
        {
            Debug.Assert(selectionProportion >= 0.0 && selectionProportion <= 1.0);

            var desc = new ConnectionMutationDescriptor();
            desc.ConnectionSelectionType = ConnectionSelectionType.Proportion;
            desc.SelectionProportion = selectionProportion;

            desc.WeightMutationType = WeightMutationType.Delta;
            desc.WeightDeltaFunc = weightDeltaFunc;
            return desc;
        }

        public static ConnectionMutationDescriptor CreateNumberReInit(int selectionCount)
        {
            Debug.Assert(selectionCount > 0);

            var desc = new ConnectionMutationDescriptor();
            desc.ConnectionSelectionType = ConnectionSelectionType.Count;
            desc.SelectionCount = selectionCount;
            desc.WeightMutationType = WeightMutationType.ReInit;
            return desc;
        }

        public static ConnectionMutationDescriptor CreateNumberDelta(int selectionCount, Func<double> weightDeltaFunc)
        {
            Debug.Assert(selectionCount > 0);

            var desc = new ConnectionMutationDescriptor();
            desc.ConnectionSelectionType = ConnectionSelectionType.Proportion;
            desc.SelectionCount = selectionCount;

            desc.WeightMutationType = WeightMutationType.Delta;
            desc.WeightDeltaFunc = weightDeltaFunc;
            return desc;
        }

        #endregion
    }
}
