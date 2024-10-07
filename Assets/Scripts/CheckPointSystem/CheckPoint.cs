using Model;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public CheckPointPathState checkPointPathState;


	private int characterLayer;
	private bool passed;
	private void Start()
	{
		passed = false;
		characterLayer = LayerMask.NameToLayer("Character");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!passed && other.gameObject.layer == characterLayer)
		{
			passed = true;
			EventManager.Instance.SendGlobalEvent("CharacterPassedCheckPoint", checkPointPathState);
		}
	}

	public void ResetCheckPoint()
	{
		passed = false;
	}
}
