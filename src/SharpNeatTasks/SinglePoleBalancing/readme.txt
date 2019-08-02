Single Pole Balancing in 2D.

Also known as: 
  pole-cart problem
  broom balancer
  the inverted pendulum problem
  stick balancer problem

The physics model consists of a cart on a rail with fixed length. The cart moves along the x-axis (x=0 represents the center of the rail).
A single pole is attached to the cart with a hinge that allows the pole to move left and right.

An external agent can push the cart with a fixed force either left or right.


The model parameters are:
* Rail length. Default is 4.8 m, i.e. +-2.4 m from tne center.
* Cart mass. Default is 1 kg.
* Pole mass. Default is 0.1 kg.
* Pole length. Default is 1 m.
* Fixed force. Default is +-10 Newtons, i.e. the controller controls only the direction that this force is applied.


The model state variables are:
* Cart position on rail (x-axis).
* Cart velocity.
* Polel angle.
* Polel angular velocity.

All four variables are provided as inputs to the controller; and note that velocity input in particular makes the task an easy one to 
solve with no hidden neural net nodes required.

* Physics simulation timesteps are in increments of 0.01 seconds.
* Maximum timesteps for a trial is 100,000, i.e. 1000 seconds, or about 16 minutes clock time.


The goal is for the controller to balance the pole for as long as possible by applying force to the cart.

The controller is considered to have failed if the cart runs off the ends of the track (i.e. an x position of outside of +-2.4 meters) 
and/or the pole angle is >= +-12 degrees.


The physics model is taken from:

  THE POLE BALANCING PROBLEM,
  A Benchmark Control Theory Problem
  Technical Report 7-01
  Jason Brownlee
  Centre for Intelligent Systems and Complex Processes
  Faculty of Information and Communication Technologies
  Swinburne University of Technology
  Melbourne, Victoria, Australia

  https://pdfs.semanticscholar.org/3dd6/7d8565480ddb5f3c0b4ea6be7058e77b4172.pdf


The original formulation of the problem appears to have originated in this paper:

  Neuronlike Adaptive Elements That Can Solve Difficult Learning Control Problems
  Andrew G. Barton, Richard S. Sutton, Charles W. Anderson
  IEEE Transactions on Systems, Man, and Cybernetics, VOL. SMC-13, NO. 5, September/October 1983
  http://incompleteideas.net/papers/barto-sutton-anderson-83.pdf


Colin Green
August 2nd, 2019
