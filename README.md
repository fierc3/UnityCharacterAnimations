# UnityCharacterAnimations

## Goal of this Prototype
The goal of this prototype is to show the different approaches possible regarding animations in Unity and the possibllity it gives to create dynamic character.

## Keyframe vs Procedural animations
To understand this project fully the understanding between the difference of keyframe vs procedural animations need to be known:

### Keyframe Animations

 This type of animation involves manually setting keyframes for the position, rotation, and scale of a 3D object, 
 and Unity then interpolates the movement between those keyframes.

![](https://help.autodesk.com/cloudhelp/2022/ENU/Maya-Animation/images/GUID-98B5004E-3275-471B-8CFA-461D7298C694.gif)

 
### Procedural Animations

 Keyframe animation requires a lot of manual work, but it allows for precise control over the movement of an object.On the other hand, procedural animation involves using code to generate animations on the fly. This can include animations that are driven by physics simulations, procedural modeling, or other algorithms. 
 Procedural animation is more flexible than keyframe animation because it can adapt to changes in the environment or user input in real-time. 

![](https://1.bp.blogspot.com/-BT9RbvrEYpc/YaANGeLKdMI/AAAAAAAADY8/mdgswjhL-dIcikEPnOPdRn2Dm39l_QN_gCLcBGAsYHQ/s641/ProceduralWalker2.gif)

### Character Animations
This prototype attempts to show how these 2 animations types can work together in the _Showcase_ Scene.
![image](https://user-images.githubusercontent.com/16936182/232013113-9f184ea2-5e8c-45d3-b723-2de8ceca377c.png)

## Using Unity to its advantage
Implementing all this from scratch would be a HUGE effort, which is why were using the tools provided by Unity.

### Mecanim
Mecanim is a state machine-based animation system in Unity, which allows developers to create complex animation sequences for characters and objects in games. 
It provides a way to define animation transitions and blend animations smoothly, allowing for more natural-looking animations. 
Mecanim also provides features such as inverse kinematics, animation events, and parameter-driven animations, which allow for greater control over the animations. 

More: https://docs.unity3d.com/462/Documentation/Manual/MecanimAnimationSystem.html

![image](https://user-images.githubusercontent.com/16936182/232015536-a5b5f550-be1b-4faa-a120-c3cf343b6f38.png)


### Unity Animation Rigging
Unity's Animation Rigging package allows you to create custom animations by manipulating the position, rotation, and scale of joints in real-time. 
This approach is useful for creating complex animations that are difficult or impossible to achieve with traditional keyframe animation

More: https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.0/manual/index.html

![image](https://user-images.githubusercontent.com/16936182/232015825-0e25696b-afa9-4a29-ac4f-713a6141dc21.png)


## Character Features
Each feature displayed in the _Showcase_ should be a learning tool, here a quick explaination to each one:

- Idle / Running
  - Simple Example how to switch states between 2 animation clips and show how the transition (interpolation) can be influenced
- Look At
  - Shows the power of using a rig with Animation Rigging constraints and using code to dictate those constraints. A target is dynamically changed via code and the constraints adjust accordingly
- Ragdoll
  - Simple implementation of a ragdoll system which changes to collider depending on state
- Get up (Unfinished)
  - Combines procedural with keyframe animations to create a standing up animations. Also shows the workflow that can be used to add Mixamo animations for existing  character
- Cape (not yet started)
  - Example showing how cloth physics can be combined with Animation Rigging constraints.
- Spell Casting
  - Example showing the use of motion capture data turned keyframe animation in combination with animation layers

