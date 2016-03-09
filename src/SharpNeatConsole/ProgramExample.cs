/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using log4net.Config;
using SharpNeat.Core;
using SharpNeat.Domains.Xor;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace SharpNeatConsole
{
    /// <summary>
    /// Minimal console application that hardwaires the setting up on a evolution algorithm and start it running.
    /// </summary>
    class Program
    {
        static IGenomeFactory<NeatGenome> _genomeFactory;
        static List<NeatGenome> _genomeList;
        static NeatEvolutionAlgorithm<NeatGenome> _ea;

        static void Main(string[] args)
        {
            // Initialise log4net (log to console).
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));

            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            XorExperiment experiment = new XorExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("xor.config.xml");
            experiment.Initialize("XOR", xmlConfig.DocumentElement);

            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            _genomeFactory = experiment.CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            _genomeList = _genomeFactory.CreateGenomeList(150, 0);

            // Create evolution algorithm and attach update event.
            _ea = experiment.CreateEvolutionAlgorithm(_genomeFactory, _genomeList);
            _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

            // Start algorithm (it will run on a background thread).
            _ea.StartContinue();

            // Hit return to quit.
            Console.ReadLine();
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, _ea.Statistics._maxFitness));
        }
    }
}
