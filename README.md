# SharpNEAT - Evolution of Neural Networks

[NEAT](https://en.wikipedia.org/wiki/Neuroevolution_of_augmenting_topologies) is an [evolutionary algorithm](https://en.wikipedia.org/wiki/Evolutionary_algorithm) devised by [Kenneth O. Stanley](https://en.wikipedia.org/wiki/Kenneth_Stanley).

SharpNEAT is a full implementation of NEAT written in C# and targeting .NET 9.

## What is SharpNEAT?

SharpNEAT provides an implementation of an [Evolutionary Algorithm](https://en.wikipedia.org/wiki/Evolutionary_algorithm) (EA) that provides the ability to evolve a population of neural networks towards solving some problem task (also known as an objective function).

The EA applies evolutionary mechanisms of mutation, recombination, and selection to search for neural networks that perform well at a given problem task. Each neural network is evaluated and assigned a fitness score representing the quality of its solution.

Some example problem tasks:

* Controlling the limbs of a simple biped or quadruped to facilitate walking.
* Controlling a rocket to maintain vertical flight.
* Designing a neural network to implement digital logic, such as a multiplexer.

A distinctive feature of NEAT is its ability to evolve both the structure (the set of network nodes and how they are connected) and the connection weights of neural networks. This contrasts with approaches like [backpropagation](https://en.wikipedia.org/wiki/Backpropagation) that optimize connection weights for a fixed network structure.

SharpNEAT serves as a framework or 'kit of parts' for conducting research in evolutionary computation, particularly for evolving neural networks. It includes example tasks that demonstrate how to assemble a complete, functioning evolutionary algorithm.

## Key Design Principles

SharpNEAT is modular by design. This modularity allows for experimentation with alternative genetic encodings, or entirely different evolutionary algorithms. The architecture emphasizes key abstractions such as:

* Genome – Genetic representation/encoding.
* Evolutionary Algorithm – Mutation, recombination, and selection strategies.

This design promotes experimentation and customization, facilitating research and development in evolutionary computation.

The motivation behind SharpNEAT stems from a broader interest in biological evolution and the exploration of neuro-evolution, aiming to understand the types and complexity of problems that can be addressed through this approach.

## More Info / Links

* [SharpNEAT YouTube channel](https://www.youtube.com/@sharpneat)
* [SharpNEAT 4: Source Code Walkthrough](https://youtu.be/pqVOAo669n0)
