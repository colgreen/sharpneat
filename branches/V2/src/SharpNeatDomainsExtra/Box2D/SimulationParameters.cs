/* ***************************************************************************
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
using Box2DX.Common;

namespace SharpNeat.DomainsExtra.Box2D
{
    /// <summary>
    /// Represents some generic / high level Box2d simulation parameters.
    /// </summary>
    public class SimulationParameters
    {
        /// <summary>
        /// Bottom left corner of physics rectangle (the area which for which physics is simulated/calculated)
        /// </summary>
        public Vec2 _lowerBoundPhysics = new Vec2(-11f, -0.5f);
        /// <summary>
        /// Upper right corner of physics rectangle (the area which for which physics is simulated/calculated)
        /// </summary>
        public Vec2 _upperBoundPhysics = new Vec2(11f, 10f);
        /// <summary>
        /// Bottom left corner of view rectangle (default rectangle for visual rendering of the world; typically
        /// the same or slightly smaller than the physics rectangle).
        /// </summary>
        public Vec2 _lowerBoundView = new Vec2(-11f, -0.5f);
        /// <summary>
        /// Bottom left corner of view rectangle (default rectangle for visual rendering of the world; typically
        /// the same or slightly smaller than the physics rectangle).
        /// </summary>
        public Vec2 _upperBoundView = new Vec2(11f, 10f);
        /// <summary>
        /// World gravity, e.g. -9.8. (+ve Y is up).
        /// </summary>
        public float _gravity = -9.8f;
        /// <summary>
        /// Update rate in Hertz. Inverse of the simulation timestep increment, e.g. 60Hz = 16ms timestep.
        /// </summary>
        public int _frameRate = 60;
        /// <summary>
        /// See _frameRate.
        /// </summary>
        public float _timeStep = 1f / 60f;
        /// <summary>
        /// Box2d velocity constraint solver iterations per loop.
        /// </summary>
        public int _velocityIters = 10;
        /// <summary>
        /// Box2d position constraint solver iterations per loop.
        /// </summary>
        public int _positionIters = 8;
        /// <summary>
        /// Allow physics calcs to use values from previous timestep.
        /// </summary>
        public bool _warmStarting = true;
        /// <summary>
        /// Enable additional collision detection for high speed objects (that might not ever contact each other at a given timestep due to speed).
        /// </summary>
        public bool _continuousPhysics = true;
        /// <summary>
        /// Default friction.
        /// </summary>
        public float _defaultFriction = 2f;
        /// <summary>
        /// Default restitution.
        /// </summary>
        public float _defaultRestitution = 0.5f;
        /// <summary>
        /// Default density.
        /// </summary>
        public float _defaultDensity = 1.0f;
    }
}
