﻿/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
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

            WriteHelp();

            // Read key commands from the console.
            for(;;)
            {
                // Read command.
                Console.Write(">");
                string cmdstring = Console.ReadLine();

                // Parse command.
                string[] cmdArgs = cmdstring.Split(' ');

                try
                {
                    // Process command.
                    switch(cmdArgs[0])
                    {
                        // Init commands.
                        case "randpop":
                        {
                            if(null != _ea && _ea.RunState == RunState.Running) {
                                Console.WriteLine("Error. Cannot create population while algorithm is running.");
                                break;
                            }

                            // Attempt to parse population size arg.
                            if(cmdArgs.Length <= 1) {
                                Console.WriteLine("Error. Missing {size} argument.");
                                break;
                            }

                            int populationSize;
                            if(!int.TryParse(cmdArgs[1], out populationSize)) {
                                Console.WriteLine(string.Format("Error. Invalid {size} argument [{0}].", cmdArgs[1]));
                                break;
                            }

                            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
                            _genomeFactory = experiment.CreateGenomeFactory();

                            // Create an initial population of randomly generated genomes.
                            _genomeList = _genomeFactory.CreateGenomeList(populationSize, 0);
                            Console.WriteLine(string.Format("Created [{0}] random genomes.", populationSize));
                            break;
                        }
                        case "loadpop":
                        {
                            if(null != _ea && _ea.RunState == RunState.Running) {
                                Console.WriteLine("Error. Cannot load population while algorithm is running.");
                                break;
                            }

                            // Attempt to get population filename arg.
                            if(cmdArgs.Length <= 1) {
                                Console.WriteLine("Error. Missing {filename} argument.");
                                break;
                            }

                            // Open and load population XML file.
                            using(XmlReader xr = XmlReader.Create(cmdArgs[1])) {
                                _genomeList = experiment.LoadPopulation(xr);
                            }
                            _genomeFactory = _genomeList[0].GenomeFactory;
                            Console.WriteLine(string.Format("Loaded [{0}] genomes.", _genomeList.Count));
                            break;
                        }
                        case "loadseed":
                        {   
                            if(null != _ea && _ea.RunState == RunState.Running) {
                                Console.WriteLine("Error. Cannot load population while algorithm is running.");
                                break;
                            }

                            // Attempt to get genome filename arg.
                            if(cmdArgs.Length <= 1) {
                                Console.WriteLine("Error. Missing {filename} argument.");
                                break;
                            }

                            // Attempt to parse population size arg.
                            if(cmdArgs.Length <= 2) {
                                Console.WriteLine("Error. Missing {size} argument.");
                                break;
                            }

                            int populationSize;
                            if(!int.TryParse(cmdArgs[1], out populationSize)) {
                                Console.WriteLine(string.Format("Error. Invalid {size} argument [{0}].", cmdArgs[1]));
                                break;
                            }

                            // Open and load genome XML file.
                            using(XmlReader xr = XmlReader.Create(cmdArgs[1])) {
                                _genomeList = experiment.LoadPopulation(xr);
                            }

                            if(_genomeList.Count == 0) {
                                Console.WriteLine(string.Format("No genome loaded from file [{0}]", cmdArgs[1]));
                                _genomeList = null;
                                break;;
                            }

                            // Create genome list from seed.
                            _genomeFactory = _genomeList[0].GenomeFactory;
                            _genomeList = _genomeFactory.CreateGenomeList(populationSize, 0u, _genomeList[0]);
                            Console.WriteLine(string.Format("Created [{0}] genomes from loaded seed genome.", _genomeList.Count));
                            break;
                        }

                        // Execution control commands.
                        case "start":
                        {
                            if(null == _ea) 
                            {   // Create new evolution algorithm.
                                if(null == _genomeList) {
                                    Console.WriteLine("Error. No loaded genomes");
                                    break;
                                }
                                _ea = experiment.CreateEvolutionAlgorithm(_genomeFactory, _genomeList);
                                _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
                            }

                            Console.WriteLine("Starting...");
                            _ea.StartContinue();
                            break;
                        }
                        case "stop":
                        {
                            Console.WriteLine("Stopping. Please wait...");
                            _ea.RequestPauseAndWait();
                            Console.WriteLine("Stopped.");
                            break;
                        }
                        case "reset":
                        {
                            if(null != _ea && _ea.RunState == RunState.Running) {
                                Console.WriteLine("Error. Cannot reset while algorithm is running.");
                                break;
                            }

                            _ea = null;
                            _genomeFactory = null;
                            _genomeList = null;
                            Console.WriteLine("Reset completed.");
                            break;
                        }

                        // Genome saving commands.
                        case "savepop":
                        {
                            if(null != _ea && _ea.RunState == RunState.Running) {
                                Console.WriteLine("Error. Cannot save population while algorithm is running.");
                                break;
                            }
                            if(null == _genomeList) {
                                Console.WriteLine("Error. No population to save.");
                                break;
                            }

                            // Attempt to get population filename arg.
                            if(cmdArgs.Length <= 1) {
                                Console.WriteLine("Error. Missing {filename} argument.");
                                break;
                            }

                            // Save genomes to xml file.
                            XmlWriterSettings xwSettings = new XmlWriterSettings();
                            xwSettings.Indent = true;
                            using(XmlWriter xw = XmlWriter.Create(cmdArgs[1], xwSettings)) {
                                experiment.SavePopulation(xw, _genomeList);
                            }
                            Console.WriteLine(string.Format("[{0}] genomes saved to file [{1}]", _genomeList.Count, cmdArgs[1]));
                            break;
                        }
                        case "savebest":
                        {
                            if(null != _ea && _ea.RunState == RunState.Running) {
                                Console.WriteLine("Error. Cannot save population while algorithm is running.");
                                break;
                            }
                            if(null == _ea || null == _ea.CurrentChampGenome) {
                                Console.WriteLine("Error. No best genome to save.");
                                break;
                            }

                            // Attempt to get genome filename arg.
                            if(cmdArgs.Length <= 1) {
                                Console.WriteLine("Error. Missing {filename} argument.");
                                break;
                            }

                            // Save genome to xml file.
                            XmlWriterSettings xwSettings = new XmlWriterSettings();
                            xwSettings.Indent = true;
                            using(XmlWriter xw = XmlWriter.Create(cmdArgs[1], xwSettings)) {
                                experiment.SavePopulation(xw, new NeatGenome[] {_ea.CurrentChampGenome});
                            }

                            Console.WriteLine(string.Format("Best genome saved to file [{1}]", _genomeList.Count, cmdArgs[1]));
                            break;
                        }

                        // Misc commands
                        case "help":
                        {
                            WriteHelp();
                            break;
                        }
                        case "quit":
                        case "exit":
                        {
                            Console.WriteLine("Stopping. Please wait...");
                            _ea.RequestPauseAndWait();
                            Console.WriteLine("Stopped.");
                            goto quit;
                        }
                        case "":
                        {
                            // Do nothing.
                            break;
                        }
                        default:
                        {
                            Console.WriteLine(string.Format("Unknown command [{0}]", cmdArgs[0]));
                            break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format("Exception [{0}]", ex.Message));
                }
            }

            quit:
            Console.WriteLine("bye!");
        }

        static void WriteHelp()
        {
            Console.Write(
@"SharpNEAT 2.0 command line interface (CLI).

Commands...
Initialisation
    randpop {size}                  Create random population.
    loadpop {filename}              Load existing population from file.
    loadseed {filename} {popSize}   Load seed genome from file.

Execution control:
    start                   Start/Continue evolution algorithm.
    stop                    Stop/Pause evolution algorithm.
    reset                   Discard any current population and evolution algorithm.

Genome saving commands:
    savepop {filename}      Save population to XML file.
    savebest {filename}     Save best genome to XML file.

Misc
    help                    Show this help text.  
    exit, quit              Exit CLI.

");
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, _ea.Statistics._maxFitness));
        }
    }
}
