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
using Newtonsoft.Json.Linq;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;
using static SharpNeat.IO.JsonReadMandatoryUtils;
using static SharpNeat.IO.JsonReadOptionalUtils;

namespace SharpNeat.Experiments
{
    /// <summary>
    /// Static utility methods for reading property settings of <see cref="NeatExperiment{T}"/> from json.
    /// </summary>
    /// <typeparam name="T">Black box numeric data type.</typeparam>
    public static class NeatExperimentJsonReader<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Read json into a target instance of <see cref="NeatExperiment{T}"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="jobj">The json object to read from.</param>
        public static void Read(
            NeatExperiment<T> target, JObject jobj)
        {
            ReadStringOptional(jobj, "description", x => target.Description = x);
            ReadBoolOptional(jobj, "isAcyclic", x => target.IsAcyclic = x);
            ReadIntOptional(jobj, "cyclesPerActivation", x => target.CyclesPerActivation = x);
            ReadStringOptional(jobj, "activationFnName", x => target.ActivationFnName = x);

            ReadNeatEvolutionAlgorithmSettings(target, jobj);
            ReadNeatReproductionAsexualSettings(target, jobj);
            ReadNeatReproductionSexualSettings(target, jobj);

            ReadIntOptional(jobj, "populationSize", x => target.PopulationSize = x);
            ReadDoubleOptional(jobj, "initialInterconnectionsProportion", x => target.InitialInterconnectionsProportion = x);
            ReadDoubleOptional(jobj, "connectionWeightScale", x => target.ConnectionWeightScale = x);

            ReadComplexityRegulationStrategy(target, jobj);

            ReadIntOptional(jobj, "degreeOfParallelism", x => target.DegreeOfParallelism = x);
            ReadBoolOptional(jobj, "suppressHardwareAcceleration", x => target.SuppressHardwareAcceleration = x);
        }

        #endregion

        #region Private Static Methods

        private static void ReadNeatEvolutionAlgorithmSettings(
            NeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["neatEvolutionAlgorithmSettings"];
            if(settingsJobj != null) {
                NeatEvolutionAlgorithmSettingsJsonReader.Read(target.NeatEvolutionAlgorithmSettings, settingsJobj);
            }
        }

        private static void ReadNeatReproductionAsexualSettings(
            NeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["reproductionAsexualSettings"];
            if(settingsJobj != null) {
                NeatReproductionAsexualSettingsJsonReader.Read(target.ReproductionAsexualSettings, settingsJobj);
            }
        }

        private static void ReadNeatReproductionSexualSettings(
            NeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["reproductionSexualSettings"];
            if(settingsJobj != null) {
                NeatReproductionSexualSettingsJsonReader.Read(target.ReproductionSexualSettings, settingsJobj);
            }
        }

        private static void ReadComplexityRegulationStrategy(
            NeatExperiment<T> target, JObject jobj)
        {
            IComplexityRegulationStrategy strategy = ReadComplexityRegulationStrategy(jobj);

            // If a strategy was explcitly defined, then set it on our target settings object to override the default strategy;
            // otherwise leave the default strategy in place.
            if(strategy != null) {
                target.ComplexityRegulationStrategy = strategy;
            }
        }

        private static IComplexityRegulationStrategy ReadComplexityRegulationStrategy(JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["complexityRegulationStrategy"];
            if(settingsJobj == null) {
                return null;
            }

            string strategyName = (string)settingsJobj["strategyName"];
            switch(strategyName)
            {
                case null:
                    // No strategy defined; continue using whatever strategy is defined by default.
                    return null;

                case "null":
                    // The 'null' strategy has been explicitly defined.
                    return new NullComplexityRegulationStrategy();

                case "absolute":
                    return ReadAbsoluteComplexityRegulationStrategy(settingsJobj);

                case "relative":
                    return ReadRelativeComplexityRegulationStrategy(settingsJobj);

                default:
                    throw new Exception($"Unsupported complexity regulation strategyName [{strategyName}]");
            }
        }

        private static AbsoluteComplexityRegulationStrategy ReadAbsoluteComplexityRegulationStrategy(JObject jobj)
        {
            int complexityCeiling = ReadIntMandatory(jobj, "complexityCeiling");
            int minSimplifcationGenerations = ReadIntMandatory(jobj, "minSimplifcationGenerations");
            return new AbsoluteComplexityRegulationStrategy(minSimplifcationGenerations, complexityCeiling);
        }

        private static RelativeComplexityRegulationStrategy ReadRelativeComplexityRegulationStrategy(JObject jobj)
        {
            int relativeComplexityCeiling = ReadIntMandatory(jobj, "relativeComplexityCeiling");
            int minSimplifcationGenerations = ReadIntMandatory(jobj, "minSimplifcationGenerations");
            return new RelativeComplexityRegulationStrategy(minSimplifcationGenerations, relativeComplexityCeiling);
        }

        #endregion
    }
}
