# Unity-Tools
This project aims to centralize the various tools I develop under Unity. 
If there are things you can use, use it for free (under MIT license).

### ✦ Overview
The tools are designed to speed up iteration time for a better workflow efficiency. Most of them are independent but there is usually a common base (found in the Core/ folder). 

![overview-demo](https://i.gyazo.com/c0c902c4b3d57020f6719952cb0d6609.gif)

*Finite State Machine and blackboard sample*

### ✦ Features
* **Scriptable Object architecture** (inspired by the Ryan Hipple Game Architecture with Scriptable Objects conference: https://www.youtube.com/watch?v=raQ3iHhE_Kk)
* **UE4-like Blackboard**: A class allowing to store an array of variables by entities. This allows information to be transmitted through multiple components while reducing their coupling.
* **Finite State Machine** inspector framework (FSMState, FSMStateTransition, FSMStateTransitionCondition, FSMStateModule):
Allows to design multiple states from the editor without using code (or very little). 
* **Sensors** (Raycast Utility to use raycast from asset descriptors, ...)
* **Advanced Editor Utility** (Animation, Input management, MinMaxRange, Physcis...)
* **GUI attributes** to extend the editor (such as ReadOnly or LayerMask).
* **Procedural action system** (for 2D action games): Allowing to combine various tweakable actions (Hitbox for damage, Momentum for movement, StatModifier, Combo Sequence, ...)
