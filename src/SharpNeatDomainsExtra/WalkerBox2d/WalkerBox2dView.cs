/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2012 Colin Green (sharpneat@gmail.com)
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
            _walkerController = new NeuralNetController(walkerIface, _box);
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
