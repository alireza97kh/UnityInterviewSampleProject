# UnityInterviewSampleProject

The **UnityInterviewSampleProject** is a sample project that contains NPCs, a character, and a checkpoint system.

## Features

1. **Character**:
   - Control the character using WASD.
   - The `Character` class contains movement speed, rotation speed, and utilizes Unity's AI library called NavMesh Agent.

2. **NPCs**:
   - NPCs have three actions:
     - **Do Nothing**: Idle state.
     - **Can Feel Character**: NPC senses the character's presence.
     - **Character Is in Sight**: NPC actively sees the character.
   - Each action corresponds to a specific state, which you can set in the NPC Inspector.
   - NPC color changes based on its state.
   - For NPC movement in the scene, there is a WayPoint system:
     - Create a new WayPoint path by going to `Tools -> WayPoint Editor`.
     - Set the WayPoint Root as a GameObject (usually an empty GameObject) that will serve as the root of your waypoint hierarchy.
     - Select "Create Waypoint" and adjust its position in the scene.
     - Don't forget to attach the waypoints to the Enemy GameObject in the scene so that the NPC follows the created path.

3. **Checkpoint System**:
   - There are two checkpoints in the scene: Checkpoint A and Checkpoint B.
   - Checkpoint A marks the start of the path.
   - If the character reaches Checkpoint B without being seen by an NPC, the character's score increases.
   - You can customize the reward in the CheckPointManager object in the scene.

## Notes

1. **Adding New Obstacles or Objects to the Scene**:
   - Go to the Map GameObject in the scene.
   - In the Inspector, find the NavMeshSurface component and select "Bake."
   - If you've made adjustments or added new obstacles, baking the NavMesh ensures that both the character and NPCs can navigate around them.

2. **Changing the Camera Controller**:
   - The project currently uses a Virtual Camera.
   - In the CinemachineVirtualCamera, you can modify the main camera's body, aim, and other settings.
   - For more information, refer to the Cinemachine Documentation.
