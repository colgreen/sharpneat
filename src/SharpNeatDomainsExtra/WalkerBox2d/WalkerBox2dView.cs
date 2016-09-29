/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using Redzen.Numerics;
using SharpNeat.Core;
using SharpNeat.DomainsExtra.Box2D;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// View class for the Walker2D problem domain.
    /// </summary>
    public class WalkerBox2dView : Box2dDomainView
    {
        WalkerController _walkerController;

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder, this is used to decode genome(s) into IBlackBox controllers.
        /// </summary>
        public WalkerBox2dView(IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
            : base(genomeDecoder)
        {}

        #endregion

        #region Override Methods [BOX2D Simulation Creation / Control]

        /// <summary>
        /// Create a Box2D simulation world.
        /// </summary>
        protected override SimulationWorld CreateSimulationWorld()
        {
            // Init Box2D world.
            WalkerWorld world = new WalkerWorld(new XorShiftRandom());
            world.InitSimulationWorld();

            // Create an interface onto the walker.
            WalkerInterface walkerIface = world.CreateWalkerInterface();

            // Create a neural net controller for the walker.
            _walkerController = new NeuralNetController(walkerIface, _box, world.SimulationParameters._frameRate);
            return world;
        }

        /// <summary>
        /// Invoke any required control logic in the Box2D world.
        /// </summary>
        protected override void InvokeController()
        {
            _walkerController.Step();
        }

        #endregion
    }
}
