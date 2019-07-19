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
using System.IO;
using Newtonsoft.Json.Linq;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;
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
        /// Read json from a file into a target instance of <see cref="NeatExperiment{T}"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="filename">The filename of the json to read from.</param>
        public static void ReadFile(
            INeatExperiment<T> target, string filename)
        {
            // Read the entire contents into a string; we don't ever expect to see large json files here, so this fine.
            string jsonStr = File.ReadAllText(filename);

            // Parse the json string.
            var jobj = JObject.Parse(jsonStr);

            // Read the parsed json into our target experiment object.
            Read(target, jobj);
        }

        /// <summary>
        /// Read json into a target instance of <see cref="NeatExperiment{T}"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="jobj">The json object to read from.</param>
        public static void Read(
            INeatExperiment<T> target, JObject jobj)
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
            INeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["evolutionAlgorithmSettings"];
            if(settingsJobj != null) {
                NeatEvolutionAlgorithmSettingsJsonReader.Read(target.NeatEvolutionAlgorithmSettings, settingsJobj);
            }
        }

        private static void ReadNeatReproductionAsexualSettings(
            INeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["reproductionAsexualSettings"];
            if(settingsJobj != null) {
                NeatReproductionAsexualSettingsJsonReader.Read(target.ReproductionAsexualSettings, settingsJobj);
            }
        }

        private static void ReadNeatReproductionSexualSettings(
            INeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["reproductionSexualSettings"];
            if(settingsJobj != null) {
                NeatReproductionSexualSettingsJsonReader.Read(target.ReproductionSexualSettings, settingsJobj);
            }
        }

        private static void ReadComplexityRegulationStrategy(
            INeatExperiment<T> target, JObject jobj)
        {
            JObject settingsJobj = (JObject)jobj["complexityRegulationStrategy"];
            if(settingsJobj != null) {
                target.ComplexityRegulationStrategy = ComplexityRegulationStrategyJsonReader.Read(settingsJobj);    
            }
        }

        #endregion
    }
}
