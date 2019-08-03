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
 * Coefficient of friction of cart on track. Default is 0.01.
 * Coefficient of pole hinge. Default is 0.0018.
 * Maximum controller force. Default is 10 Newtons.


The model state variables are:
 * Cart position on rail (x-axis).
 * Cart velocity.
 * Polel angle.
 * Polel angular velocity.

All four variables are provided as inputs to the controller; and note that velocity input in particular makes the task an easy one to 
solve with no hidden neural net nodes required.

* Physics simulation timesteps are in increments of 0.01 seconds.
* Maximum timesteps for a trial is 200,000, i.e. 2000 seconds, or about 33.3 minutes clock time.


The goal is for the controller to balance the pole for as long as possible by applying force to the cart.

The controller is considered to have failed if the cart runs off the ends of the track (i.e. an x position of outside of +-2.4 meters) 
and/or the pole angle is >= +-12 degrees.

Notes on Friction
-----------------
The canonical pole balancing tasks did not include modelling of friction as it was deemed an inconsequential 
component of the model with regards to it difficulty. In practice I found that if we apply no force then the pole will swing to an angle
higher than its initial angle, i.e. the total energy in the system will increase due to the approximations to the real world physics, and
in particular the use of Euler's method, which will tend to under and overestimate.

By introducing friction we can avoid the steady increase in total system energy, in particular the pole friction seems to be critical and
the default value of 0.0018 was chosen as a value that just about prevent the pole from swinging to an angle higher than its starting angle
when starting from its default startign angle of 6 degrees.

Cart friction seems less important overall, but note that a typical car will have a drag coefficient of 0.3, therefore the value of 0.01 for
the cart is very low indeed.

My initial observation is that including friction into the model seems to have made the problem task much easier to solve, and this may be
because without friction the main problem to solve is a runaway increase in energy in the system (due to physics approximations) rather
than the pole balancing task per se.


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


This paper seems to be an earlier source of the update equations, for both the single and double pole balancing tasks,
and including modelling of friction at the cart-track, and pole hinge.

  Evolving Controls for Unstable Systems
  Alexis P. Weiland
  Computer Science Department
  University of California,
  Los Angeles, CA 90024-1596
  https://books.google.co.uk/books?hl=en&lr=&id=_Z6jBQAAQBAJ&oi=fnd&pg=PA91


This even earler paper contains the same equations, but for single pole balancing task only.

  Neuronlike Adaptive Elements That Can Solve Difficult Learning Control Problems
  Andrew G. Barton, Richard S. Sutton, Charles W. Anderson
  IEEE Transactions on Systems, Man, and Cybernetics, VOL. SMC-13, NO. 5, September/October 1983
  http://incompleteideas.net/papers/barto-sutton-anderson-83.pdf


Colin Green
August 2nd, 2019
