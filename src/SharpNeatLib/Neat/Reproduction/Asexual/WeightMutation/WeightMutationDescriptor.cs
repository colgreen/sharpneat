using System;
using System.Diagnostics;
using Redzen.Numerics;
using SharpNeat.Utils;

namespace SharpNeat.Genomes.Neat.Reproduction.WeightMutation
{
    /// <summary>
    /// Describes a specific form of weight mutation; i.e. the types of selection and mutation and any associated
    /// parameters.
    /// </summary>
    public class WeightMutationDescriptor
    {
        #region Auto Properties [Top Level Settings]

        /// <summary>
        /// Defines how connections are selected.
        /// </summary>
        public ConnectionSelectionType ConnectionSelectionType { get; }

        /// <summary>
        /// The type of connection weight mutation to apply.
        /// </summary>
        public WeightMutationType WeightMutationType { get; }

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
        public WeightMutationDescriptor(ConnectionSelectionType selectionType, WeightMutationType mutationType)
        { 
            this.ConnectionSelectionType = selectionType;
            this.WeightMutationType = mutationType;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public WeightMutationDescriptor(WeightMutationDescriptor copyFrom)
        {
            this.ConnectionSelectionType = copyFrom.ConnectionSelectionType;
            this.WeightMutationType = copyFrom.WeightMutationType;
            this.SelectionProportion = copyFrom.SelectionProportion;
            this.SelectionCount = copyFrom.SelectionCount;
            this.WeightDeltaFunc = copyFrom.WeightDeltaFunc;
        }

        #endregion

        #region Public Static Methods [Factory Methods - High Level]

        public static WeightMutationDescriptor CreateSelectProportionGaussianDelta(double selectionProportion, double stdDev)
        {
            Debug.Assert(selectionProportion >= 0.0 && selectionProportion <= 1.0);
            Debug.Assert(stdDev > 0.0);

            var desc = new WeightMutationDescriptor(ConnectionSelectionType.Proportion, WeightMutationType.Delta);
            desc.SelectionProportion = selectionProportion;

            var rng = RandomFactory.Create();
            ZigguratGaussianSampler gaussianSampler = new ZigguratGaussianSampler(rng);
            desc.WeightDeltaFunc = gaussianSampler.NextDouble;
            return desc;
        }

        public static WeightMutationDescriptor CreateSelectCountGaussianDelta(int selectionCount, double stdDev)
        {
            Debug.Assert(selectionCount > 0);

            var desc = new WeightMutationDescriptor(ConnectionSelectionType.Count, WeightMutationType.Delta);
            desc.SelectionCount = selectionCount;

            var rng = RandomFactory.Create();
            ZigguratGaussianSampler gaussianSampler = new ZigguratGaussianSampler(rng);
            desc.WeightDeltaFunc = gaussianSampler.NextDouble;
            return desc;
        }

        #endregion

        #region Public Static Methods [Factory Methods - Low Level]

        public static WeightMutationDescriptor CreateSelectProportionReInit(double selectionProportion)
        {
            Debug.Assert(selectionProportion >= 0.0 && selectionProportion <= 1.0);

            var desc = new WeightMutationDescriptor(ConnectionSelectionType.Proportion, WeightMutationType.ReInit);
            desc.SelectionProportion = selectionProportion;
            return desc;
        }

        public static WeightMutationDescriptor CreateSelectProportionDelta(double selectionProportion, Func<double> weightDeltaFunc)
        {
            Debug.Assert(selectionProportion >= 0.0 && selectionProportion <= 1.0);

            var desc = new WeightMutationDescriptor(ConnectionSelectionType.Proportion, WeightMutationType.Delta);
            desc.SelectionProportion = selectionProportion;
            desc.WeightDeltaFunc = weightDeltaFunc;
            return desc;
        }

        public static WeightMutationDescriptor CreateSelectCountReInit(int selectionCount)
        {
            Debug.Assert(selectionCount > 0);

            var desc = new WeightMutationDescriptor(ConnectionSelectionType.Count, WeightMutationType.ReInit);
            desc.SelectionCount = selectionCount;
            return desc;
        }

        public static WeightMutationDescriptor CreateSelectCountDelta(int selectionCount, Func<double> weightDeltaFunc)
        {
            Debug.Assert(selectionCount > 0);

            var desc = new WeightMutationDescriptor(ConnectionSelectionType.Proportion, WeightMutationType.Delta);
            desc.SelectionCount = selectionCount;
            desc.WeightDeltaFunc = weightDeltaFunc;
            return desc;
        }

        #endregion
    }
}
