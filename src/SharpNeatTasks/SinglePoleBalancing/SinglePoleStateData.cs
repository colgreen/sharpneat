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

namespace SharpNeat.Tasks.SinglePoleBalancing
{
    /// <summary>
    /// Model state variables for the single pole balancing task.
    /// </summary>
    public class SinglePoleStateData
    {
        /// <summary>
        /// Cart position (meters from origin).
        /// </summary>
		public double _cartPosX;
        /// <summary>
        /// Cart velocity (m/s).
        /// </summary>
		public double _cartVelocityX;
        /// <summary>
        /// Pole angle (radians). Straight up = 0.
        /// </summary>
		public double _poleAngle;
        /// <summary>
        /// Pole angular velocity (radians/sec).
        /// </summary>
		public double _poleAngularVelocity;
        /// <summary>
        /// Action applied during most recent timestep.
        /// </summary>
		public bool _action;
    }
}
