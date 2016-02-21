This is an implementation of the single pole balancing problem domain that uses BOX2D for the physics simulation.
The original NEAT single pole balancer code contains its own physics code. Using BOX2D (A) provides a simple example usage
for anyone wishing to create their own domains requiring a 2D physics engine and (B) allows visual rendering of the BOX2D
world with minimal extra effort via a generic BOX2D drawing code using OpenGL.


