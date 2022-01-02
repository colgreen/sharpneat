/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Text.Json;
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
        /// Read json into a target instance of <see cref="NeatExperiment{T}"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="jelem">The json element to read from.</param>
        public static void Read(
            INeatExperiment<T> target, JsonElement jelem)
        {
            ReadStringOptional(jelem, "id", x => target.Id = x);
            ReadStringOptional(jelem, "name", x => target.Name = x);
            ReadStringOptional(jelem, "description", x => target.Description = x);
            ReadBoolOptional(jelem, "isAcyclic", x => target.IsAcyclic = x);
            ReadIntOptional(jelem, "cyclesPerActivation", x => target.CyclesPerActivation = x);
            ReadStringOptional(jelem, "activationFnName", x => target.ActivationFnName = x);

            ReadNeatEvolutionAlgorithmSettings(target, jelem);
            ReadNeatReproductionAsexualSettings(target, jelem);
            ReadNeatReproductionSexualSettings(target, jelem);

            ReadIntOptional(jelem, "populationSize", x => target.PopulationSize = x);
            ReadDoubleOptional(jelem, "initialInterconnectionsProportion", x => target.InitialInterconnectionsProportion = x);
            ReadDoubleOptional(jelem, "connectionWeightScale", x => target.ConnectionWeightScale = x);

            ReadComplexityRegulationStrategy(target, jelem);

            ReadIntOptional(jelem, "degreeOfParallelism", x => target.DegreeOfParallelism = x);
            ReadBoolOptional(jelem, "enableHardwareAcceleratedNeuralNets", x => target.EnableHardwareAcceleratedNeuralNets = x);
            ReadBoolOptional(jelem, "enableHardwareAcceleratedActivationFunctions", x => target.EnableHardwareAcceleratedActivationFunctions = x);
        }

        #endregion

        #region Private Static Methods

        private static void ReadNeatEvolutionAlgorithmSettings(
            INeatExperiment<T> target, JsonElement jelem)
        {
            if(jelem.TryGetProperty("evolutionAlgorithmSettings", out JsonElement settingsElem))
            {
                NeatEvolutionAlgorithmSettingsJsonReader.Read(target.NeatEvolutionAlgorithmSettings, settingsElem);
            }
        }

        private static void ReadNeatReproductionAsexualSettings(
            INeatExperiment<T> target, JsonElement jelem)
        {
            if(jelem.TryGetProperty("reproductionAsexualSettings", out JsonElement settingsElem))
            {
                NeatReproductionAsexualSettingsJsonReader.Read(target.ReproductionAsexualSettings, settingsElem);
            }
        }

        private static void ReadNeatReproductionSexualSettings(
            INeatExperiment<T> target, JsonElement jelem)
        {
            if(jelem.TryGetProperty("reproductionSexualSettings", out JsonElement settingsElem))
            {
                NeatReproductionSexualSettingsJsonReader.Read(target.ReproductionSexualSettings, settingsElem);
            }
        }

        private static void ReadComplexityRegulationStrategy(
            INeatExperiment<T> target, JsonElement jelem)
        {
            if(jelem.TryGetProperty("complexityRegulationStrategy", out JsonElement settingsElem))
            {
                target.ComplexityRegulationStrategy = ComplexityRegulationStrategyJsonReader.Read(settingsElem);
            }
        }

        #endregion
    }
}
