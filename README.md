# BezierCurves_Demo_Raycast
This project demonstrates using 3 different types of Bezier Curves using Line Renderer. This also contains an added Raycasting functions which simulates a VR Controller's ray/spline to a point.


# Linear Bezier Curve Equation
`p= p0 + t*(p1-p0)`
2 control points : p0 and p1

# Quadratic Bezier Curve Equation
`p= (1-t)^2 *p0 + 2*(1-t) * t *p1 +t^2*p2`
3 Control points : p0,p1,and p2

# Cubic Bezier Curve Equation
`p= (1-t)^3 * p0+ 3*(1-t)^2 *t *p1 + 3*(1-t)*t^2*p2 + t^3 * p3`
 4 Control points :  p0,p1,p2,and p3

_Generic Formula_
`Summation(n/i) = C(n/i) * (1-t)^(n-i) * t^i * pi`
where 
Summation(n/i) defines sigma( sum ) 
i defines the limits
n defines the degree
C(n/i) is Combinations of n over i
t is the delta. (timestep)
pi is the control point.

# USAGE

Select the type of curve from the Enum and press play. 
_Vertical axis_, rotates the line renderer over x axis. ( For raycasting ) 
_Horizontal axis_, rotates the player gameObject.
_Raycasting_ works only when left mouse button is pressed. 
_OnRelease_, it'll teleport player gameObject to the hit position.

Functions already contain summaries of what they do. 

# Improvements(to be done):
Use mouse Drag speed instead of rotation for increasing curve.
Remove Raycasting and determine hitpoint based on speed given by the user. (Simulate something similar to controllers)
Add more scenes
