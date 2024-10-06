using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public Waypoint startWaypoint;
	public Vector3 boxSize = new Vector3(2f, 2f, 2f);
	[SerializeField] private NavMeshAgent agent;
	[SerializeField] private Transform leftEye;
	[SerializeField] private Transform rightEye;
	[SerializeField] public List<AiStateActionScore> actionScores = new List<AiStateActionScore>();
	[SerializeField] private float detectionRangeToSee = 20;
	[SerializeField] private float detectionRangeToFeel = 20;

	private AiState currentState;
	private Waypoint currentWaypoint;

	private Transform mainCharacter;
	private LayerMask characterLayerMask;
	private void Start()
	{
		if (agent == null)
			agent = GetComponent<NavMeshAgent>();
		agent.Warp(startWaypoint.GetPosition());
		currentWaypoint = startWaypoint;
		currentState = AiState.Idle;
		characterLayerMask = LayerMask.NameToLayer("Character");
		StartCoroutine(DecisionMaking());
	}
	AiAction lastAction;//Todo Remove this
	private IEnumerator DecisionMaking()
	{
		while (true)
		{
			int bestScore = 0;
			AiAction bestAction = AiAction.DoNothing;
			foreach (AiStateActionScore item in actionScores)
			{
				if (IsActionValid(item.action))
				{
					if (item.score > bestScore)
					{
						bestScore = item.score;
						bestAction = item.action;
					}
				}
			}
			if (lastAction != bestAction)
			{
				Debug.LogError(bestAction);
				lastAction = bestAction;
			}
			Debug.LogError(bestAction);
			switch (bestAction)
			{
				case AiAction.DoNothing:
					currentState = AiState.Idle;
					break;
				case AiAction.CanSeeCharacter:
					currentState = AiState.ChasingCharacter;
					break;
				case AiAction.CharacterIsInSight:
					currentState = AiState.ChasingCharacter;
					break;
				default:
					break;
			}

			ExecuteCurrentState();
			yield return null;
		}
	}

	private void ExecuteCurrentState()
	{
		switch (currentState)
		{
			case AiState.Idle:
				DoMove();
				break;
			case AiState.ChasingCharacter:
				MoveToCharacter();
				break;
		}
	}

	private bool IsActionValid(AiAction action)
	{
		switch (action)
		{
			case AiAction.DoNothing:
				return true;
			case AiAction.CanSeeCharacter:
				return CanSeeCharacter();
			case AiAction.CharacterIsInSight:
				bool isInSight = CharacterIsInSight();
				mainCharacter = (isInSight) ? mainCharacter : null;
				return isInSight;
			default:
				return false;
		}
	}

	private void DoMove()
	{
		agent.SetDestination(currentWaypoint.GetPosition());

		if (agent.remainingDistance <= agent.stoppingDistance)
		{
			if (currentWaypoint.nextWaypoint != null)
				currentWaypoint = currentWaypoint.nextWaypoint;
			else
				currentWaypoint = startWaypoint;
			agent.SetDestination(currentWaypoint.GetPosition());
		}
	}

	private bool CanSeeCharacter()
	{
		if (mainCharacter == null)
		{
			if (Physics.Raycast(leftEye.transform.position, leftEye.forward, out RaycastHit leftHit, detectionRangeToSee, characterLayerMask))
			{
				mainCharacter = leftHit.transform;
				return true;
			}
			if (Physics.Raycast(rightEye.transform.position, rightEye.forward, out RaycastHit rightHit, detectionRangeToSee, characterLayerMask))
			{
				mainCharacter = rightHit.transform;
				return true;
			}
			mainCharacter = null;
			return false;
		}
		else return true;
	}

	private bool CharacterIsInSight()
	{
		if (mainCharacter == null)
			return false;
		if (Vector3.Distance(mainCharacter.transform.position, transform.position) <= detectionRangeToSee)
		{
			if (Physics.Linecast(transform.position, mainCharacter.position, out RaycastHit hit))
			{
				if (hit.transform != mainCharacter)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private void MoveToCharacter()
	{
		if (mainCharacter != null)
			agent.SetDestination(mainCharacter.transform.position);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(leftEye.transform.position, leftEye.forward);
		Gizmos.DrawRay(rightEye.transform.position, rightEye.forward);
	}
}
