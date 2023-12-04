# SharpNEAT - Evolution of Neural Networks
 
[NEAT](https://en.wikipedia.org/wiki/Neuroevolution_of_augmenting_topologies) is an [evolutionary algorithm](https://en.wikipedia.org/wiki/Evolutionary_algorithm) devised by [Kenneth O. Stanley](https://en.wikipedia.org/wiki/Kenneth_Stanley). 

SharpNEAT is a complete implementation of NEAT written in C# and targeting .NET 8.

## What is SharpNEAT?

SharpNEAT provides an implementation of an [Evolutionary Algorithm](https://en.wikipedia.org/wiki/Evolutionary_algorithm) (EA) with the specific goal of evolving a population of neural networks towards solving some goal problem task (known as as the Objective function).

The EA uses the evolutionary mechanisms of mutation, recombination, and selection, to search for a neural network that  'solves' a given problem task, with each neural net being assigned a fitness score that represents the quality of the solution it represents.

Some example problem tasks:

* How to control the limbs of a simple biped or quadruped to make it walk.
* How to control a rocket to maintain vertical flight.
* Finding a network that implements some desired digital logic, such as a multiplexer.

A notable point is that NEAT and SharpNEAT search both neural network structure (the set of network nodes and how they are connected) and connection weights. This is distinct from algorithms such as [backpropagation](https://en.wikipedia.org/wiki/Backpropagation) that attempt to find good connection weights for a given structure.

SharpNEAT is a framework, or 'kit of parts', that facilitates research into evolutionary computation and specifically evolution of neural networks. The framework provides a number of example problem tasks that demonstrate how it can be used to produce a complete working EA.

This project aims to be modular, e.g. an alternative genetic coding or entirely new evolutionary algorithm could be used alongside the other parts/classes provided by SharpNEAT. The provision for such modular experimentation was a major design goal of SharpNEAT, and is facilitated by abstractions made in SharpNEAT's architecture around key concepts such as 'genome' (genetic representation / encoding) and 'evolutionary algorithm' (mutations, recombination, selection strategy, etc.).

Motivation for the development of SharpNEAT derives from a broader interest in biological evolution, and curiosity around the limits of [neuro-evolution](https://en.wikipedia.org/wiki/Neuroevolution), in terms of the of problems and level of problem complexity it can produce satisfactory solutions for.

## More Info / Links

* [SharpNEAT YouTube channel](https://www.youtube.com/@sharpneat)
* [SharpNEAT 4: Source Code Walkthrough](https://youtu.be/pqVOAo669n0)
