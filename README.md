# UnityInterviewSampleProject

The **UnityInterviewSampleProject** this is a sample project that contains npc, character and Check point system

## Features

1. **Character**:
   - you can move character using WASD.
   - character class contains movement speed, rotation speed and move character using Unity AI library called NavMesh Agent.

2. **NPC**:
   - Npc have 3 action: do nothing, can feel character, can see character and character is in sight.
   - Every action causes a state and you can set State in npc Inspector. (For example when character is in sight npc chase the character and etc)
   - Npc color change based on npc state. 
   - For Npc movement in scene there is a WayPoint system that contains Next WayPoint and Previous WayPoint for Create a new WayPoint path that go to Tools -> WayPoint Editor and set WayPoint Root a Game object that you want to be root of your way point and Select Create Waypoint and adjust it in scene.
   - Don't Forget after adjusting new WayPoint(if you did that!) attach it to Enemy Game Object in scene so the Npc moves in your created path.
   

3. **CheckPointSystem**:
   - there is 2 check point in scene (check point A, check point B).
   - Check point A is start of path and when we passed that and if npc won't see us untill we reach to check point B, Character score increase and you can change reward in CheckPointManager Object in scene.

## Notes

1. **If you added new obstacle or Object to scene or move around created obstacle**:
   - Go to Map Game object in scene.
   - In inspector in NavMeshSurface component Select Bake.
   - if you didn't Bake new Map Character and Npc can't find that obstacle or new adjustment that you've jsut set.

2. **If you want to Change Camera Controller**:
    - Virtual Camera.
    - in CinemachineVirtualCamera you can change main camera Body or Aim or etc.
    - for more information check [Cinemachine Documentation](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.3/manual/index.html)

3. 
