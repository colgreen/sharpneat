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
using SharpNeat.Phenomes;
using System.Diagnostics;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// A neural net based WalkerController.
    /// </summary>
    public class NeuralNetController : WalkerController
    {
        IBlackBox _box;

        #region Constructor

        /// <summary>
        /// Construct with the provided player interface and black box controller.
        /// </summary>
        public NeuralNetController(WalkerInterface iface, IBlackBox box) 
            : base(iface) 
        {
            _box = box;
        }

        #endregion

        public override void Step()
        {
        //---- Feed input signals into black box.
            // Torso state.
            _box.InputSignalArray[0] = _iface.TorsoPosition.Y;  // Torso Y pos.
            _box.InputSignalArray[1] = _iface.TorsoAngle;
            _box.InputSignalArray[2] = _iface.TorsoAnglularVelocity;
            _box.InputSignalArray[3] = _iface.TorsoVelocity.X;
            _box.InputSignalArray[4] = _iface.TorsoVelocity.Y;
            
            // Left leg state.
            _box.InputSignalArray[5] = _iface.LeftLegIFace.HipJointAngle;
            _box.InputSignalArray[6] = _iface.LeftLegIFace.HipJointVelocity;
            _box.InputSignalArray[7] = _iface.LeftLegIFace.KneeJointAngle;
            _box.InputSignalArray[8] = _iface.LeftLegIFace.KneeJointVelocity;

            // Right leg state.
            _box.InputSignalArray[9] = _iface.RightLegIFace.HipJointAngle;
            _box.InputSignalArray[10] = _iface.RightLegIFace.HipJointVelocity;
            _box.InputSignalArray[11] = _iface.RightLegIFace.KneeJointAngle;
            _box.InputSignalArray[12] = _iface.RightLegIFace.KneeJointVelocity;

        //---- Activate black box.
            _box.Activate();

        //---- Read joint torque outputs (proportion of max torque).
            // Here we assume neuron activation function with output range [0,1].
            _iface.LeftLegIFace.SetHipJointTorque((float)((_box.OutputSignalArray[0] - 0.5) * 2.0) * _iface.JointMaxTorque);
            _iface.RightLegIFace.SetHipJointTorque((float)((_box.OutputSignalArray[1] - 0.5) * 2.0) * _iface.JointMaxTorque);
            _iface.LeftLegIFace.SetKneeJointTorque((float)((_box.OutputSignalArray[2] - 0.5) * 2.0) * _iface.JointMaxTorque);
            _iface.RightLegIFace.SetKneeJointTorque((float)((_box.OutputSignalArray[3] - 0.5) * 2.0) * _iface.JointMaxTorque);
        }
    }
}
