using Model;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
	public int passCheckPointScore = 100;
	public List<CheckPoint> checkPoints = new List<CheckPoint>();


	private bool characterHitStartPoint = false;
	private bool characterBeingSeen = false;



	void Start()
	{
		EventManager.Instance.RegisterGlobalEvent("CharacterPassedCheckPoint", OnCharacterPassedCheckPoint);
		EventManager.Instance.RegisterGlobalEvent("EnemySeeCharacter", EnemySeeCharacter);
	}



	private void OnDestroy()
	{
		EventManager.Instance.RemoveGlobalEvent("CharacterPassedCheckPoint", OnCharacterPassedCheckPoint);
		EventManager.Instance.RemoveGlobalEvent("EnemySeeCharacter", EnemySeeCharacter);
	}
	private void OnCharacterPassedCheckPoint(object data)
	{
		if (data != null && data is CheckPointPathState)
		{
			CheckPointPathState state = (CheckPointPathState)data;
			switch (state)
			{
				case CheckPointPathState.StartOfPath:
					characterHitStartPoint = true;
					break;
				case CheckPointPathState.EndOfPath:
					if (characterHitStartPoint && !characterBeingSeen)
						EventManager.Instance.SendGlobalEvent("CharacterPassedCheckPointsWithoutBeingSeen", passCheckPointScore);

					ResetData();


					break;
				default:
					break;
			}
		}
	}

	private void ResetData()
	{
		characterHitStartPoint = false;
		characterBeingSeen = false;
		foreach (var item in checkPoints)
			item.ResetCheckPoint();
	}

	private void EnemySeeCharacter(object arg0)
	{
		characterBeingSeen = true;
	}
}
