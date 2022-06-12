// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.IO.Models;

namespace SharpNeat.NeuralNets.IO;

/// <summary>
/// Static methods for converting between neural network types and <see cref="NetFileModel"/>.
/// </summary>
public static class NeuralNetConverter
{
    // TODO: Implement.

    public static IBlackBox<double> ToNeuralNet(
        NetFileModel model)
    {
        throw new NotImplementedException();
    }

    public static IBlackBox<double> ToAcyclicNeuralNet(
        NetFileModel model)
    {
        throw new NotImplementedException();
    }

    public static IBlackBox<double> ToCyclicNeuralNet(
        NetFileModel model)
    {
        throw new NotImplementedException();
    }
}
