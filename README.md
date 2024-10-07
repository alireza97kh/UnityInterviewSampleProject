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


## Code Documentation

### 1. Character
- The `Character` class controls the main character's movement based on user inputs.
- When the `CharacterPassedCheckPointsWithoutBeingSeen` event is triggered by the `CheckPointManager`, the character's score increases by the value provided in the event.

### 2. EnemyController
- The `EnemyController` class manages NPCs in the game.
- It uses two `Transform` variables—one for the left eye and one for the right eye of the NPC—to cast rays and determine if the NPC can see the character.
- The class defines NPC actions and maintains a list of action scores (modifiable in the inspector).
- When adding new actions, ensure you set the validation function in `IsActionValid` to check if the action is possible during the current frame.
- Additionally, when adding new states, create corresponding functions that execute when the NPC is in that state.
- If the NPC senses the character, it searches for the character for a specified duration (`searchForCharacterDuration`). If unsuccessful, the NPC enters a sleep state, during which it ignores the character for a defined period (`sleepAfterSearchingForCharacter`).

### 3. CheckPoint
- The `CheckPoint` class manages individual checkpoints.
- Each checkpoint can have one of two states:
  - `StartOfPath`
  - `EndOfPath`
- When the character passes a checkpoint (if not previously passed), the checkpoint sends its state via the `CharacterPassedCheckPoint` event to its manager.
- The manager can reset the passed state of checkpoints using the `ResetCheckPoint` function.

### 4. CheckPointManager
- The `CheckPointManager` class handles events from checkpoints and NPCs:
  - `CharacterPassedCheckPoint`: Triggered when the character passes any checkpoint not previously passed.
  - `EnemySeeCharacter`: Sent by NPCs when they see the character.
- When a checkpoint with the `StartOfPath` state is passed, the `characterHitStartPoint` variable is set to true.
- If the character passes the start point and is not seen by an NPC, the manager increases the character's score. Otherwise, no action is taken. After checking, the manager resets relevant variables to allow the character to pass checkpoints again.

### 5. EventManager
- The `EventManager` class facilitates communication between classes without direct dependencies.
- It provides three main functions:
  - `RegisterGlobalEvent`: Registers an event with a UnityAction for future use.
  - `RemoveGlobalEvent`: Removes an action from the dictionary.
  - `SendGlobalEvent`: Calls all registered functions associated with a specific event name.

