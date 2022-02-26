/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Windows.App.Experiments;

public class ExperimentInfo
{
    public string Name { get; set; }
    public ExperimentFactoryInfo ExperimentFactory { get; set; }
    public string ConfigFile { get; set; }
    public string DescriptionFile { get; set; }
    public ExperimentUIFactoryInfo ExperimentUIFactory { get; set; }
}
