# SharpNEAT - Evolution of Neural Networks
 
NEAT is NeuroEvolution of Augmenting Topologies; an evolutionary algorithm devised by [Kenneth O. Stanley](https://en.wikipedia.org/wiki/Kenneth_Stanley). 

[SharpNEAT](http://sharpneat.sourceforge.net/) is a complete implementation of NEAT written in C# and targeting .NET [Core] 7.

From the [SharpNEAT FAQ](http://sharpneat.sourceforge.net/faq.html)...

#### 1. What is SharpNEAT

In a nutshell, SharpNEAT provides an implementation of an Evolutionary Algorithm (EA) with the specific goal of evolving neural networks. The EA uses the evolutionary mechanisms of mutation, recombination, and selection to search for neural networks with behaviour that satisfies some formally defined problem. Example problems might be:

* How to control the limbs of a simple biped or quadruped to make it walk.
* How to control a rocket to maintain vertical flight.
* Finding a network that implements some desired digital logic, such as a multiplexer.

A notable point is that NEAT and SharpNEAT search both neural network structure (network nodes and connectivity) and connection weights (inter-node connection strength). This is distinct from algorithms such as back-propogation that generally attempt to discover good connection weights for a given structure.

SharpNEAT is a framework that facilitates research into evolutionary computation and specifically evolution of neural networks. The framework provides a number of example problem tasks that demonstrate how it can be used to produce a complete working EA.

This project aims to be modular, and therefore an alternative genetic coding or entire new evolutionary algorithm could be used alongside the wider framework. The provision for such modular experimentation was a major design goal of SharpNEAT, and is facilitated by abstractions made in SharpNEAT's architecture around key concepts such as genome (genetic representation and coding) and evolutionary algorithm (mutations, recombination, selection strategy).

Motivation for the development of SharpNEAT derives from a broader interest in biological evolution, and curiosity around the limits of neuro-evolution, in terms of the of problems and level of problem complexity it can produce satisfactory solutions for.
