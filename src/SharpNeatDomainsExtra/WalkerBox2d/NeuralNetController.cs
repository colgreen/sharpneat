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
using SharpNeat.Phenomes;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// A neural net based WalkerController.
    /// </summary>
    public class NeuralNetController : WalkerController
    {
        readonly IBlackBox _box;
        int _timestep;

        #region Constructor

        /// <summary>
        /// Construct with the provided player interface and black box controller.
        /// </summary>
        public NeuralNetController(WalkerInterface iface, IBlackBox box, int simFrameRate) 
            : base(iface) 
        {
            _box = box;
        }

        #endregion

        /// <summary>
        /// Perform one simulation timestep; provide inputs to the neural net, activate it, read its outputs and assign them to the 
        /// relevant motors (leg joints).
        /// </summary>
        public override void Step()
        {
            //---- Feed input signals into black box.
            // Torso state.
            _box.InputSignalArray[0] = _iface.TorsoPosition.Y;  // Torso Y pos.
            _box.InputSignalArray[1] = _iface.TorsoAngle;
            _box.InputSignalArray[2] = _iface.TorsoVelocity.X;
            _box.InputSignalArray[3] = _iface.TorsoVelocity.Y;

            // Left leg state.
            _box.InputSignalArray[4] = _iface.LeftLegIFace.HipJointAngle;
            _box.InputSignalArray[5] = _iface.LeftLegIFace.KneeJointAngle;

            // Right leg state.
            _box.InputSignalArray[6] = _iface.RightLegIFace.HipJointAngle;
            _box.InputSignalArray[7] = _iface.RightLegIFace.KneeJointAngle;

            _box.InputSignalArray[8] = _iface.LeftLegIFace.KneeJointPosition.Y;
            _box.InputSignalArray[9] = _iface.RightLegIFace.KneeJointPosition.Y;

            _box.InputSignalArray[10] = _iface.LeftLegIFace.KneeJointPosition.X - _iface.LeftLegIFace.HipJointPosition.X;
            _box.InputSignalArray[11] = _iface.RightLegIFace.KneeJointPosition.X - _iface.RightLegIFace.HipJointPosition.X;

            _timestep++;

            //---- Activate black box.
            _box.Activate();

            //---- Read joint torque outputs (proportion of max torque).
            // Here we assume neuron activation function with output range [0,1].
            float maxTorque = _iface.JointMaxTorque;
            _iface.LeftLegIFace.SetHipJointTorque((float)((_box.OutputSignalArray[0] - 0.5) * 2.0) * maxTorque);
            _iface.RightLegIFace.SetHipJointTorque((float)((_box.OutputSignalArray[1] - 0.5) * 2.0) * maxTorque);
            _iface.LeftLegIFace.SetKneeJointTorque((float)((_box.OutputSignalArray[2] - 0.5) * 2.0) * maxTorque);
            _iface.RightLegIFace.SetKneeJointTorque((float)((_box.OutputSignalArray[3] - 0.5) * 2.0) * maxTorque);
        }
    }
}
