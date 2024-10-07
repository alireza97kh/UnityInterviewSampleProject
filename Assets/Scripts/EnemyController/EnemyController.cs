using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public Waypoint startWaypoint;
	public LayerMask characterLayerMask;

	[SerializeField] private NavMeshAgent agent;
	[SerializeField] private Transform leftEye;
	[SerializeField] private Transform rightEye;
	[SerializeField] public List<AiStateActionScore> actionScores = new List<AiStateActionScore>();
	[SerializeField] private float detectionRangeToSee = 20;
	[SerializeField] private float detectionRangeToFeel = 20;
	//Search Variables
	[SerializeField] private float searchForCharacterRotationSpeed = 10;
	[SerializeField] private float searchForCharacterDuration = 3;
	/// <summary>
	/// This variable is for when we search for character but can't find it
	/// </summary>
	[SerializeField] private float sleepAfterSearchingForCharacter = 2;
	private bool inSleepState = false;
	private Coroutine resetSleepCoroutine;


	private Waypoint currentWaypoint;
	private AiState currentState;

	private Transform mainCharacter;

	private Vector3 characterFeelPosition;

	private int characterLayerForRayCast;
	private float timer = 9;

	[Header("Material Variables")]
	[SerializeField] private Renderer enemyRenderer;

	[SerializeField] private Color defaultColor;
	[SerializeField] private Color feelCharacterColor;
	[SerializeField] private Color seeCharacterColor;


	private void Start()
	{
		if (agent == null)
			agent = GetComponent<NavMeshAgent>();
		if (enemyRenderer == null)
			enemyRenderer = GetComponent<Renderer>();
		enemyRenderer.material.color = defaultColor;
		agent.Warp(startWaypoint.GetPosition());
		currentWaypoint = startWaypoint;
		currentState = AiState.Idle;
		characterLayerForRayCast = LayerMask.NameToLayer("Character");

		StartCoroutine(DecisionMaking());
	}
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

				case AiAction.CanFeelCharacter:
					currentState = AiState.SearchForCharacter;
					break;

				default:
					break;
			}
			ExecuteCurrentState();
			SetTimer();


			yield return null;
		}
	}

	private void SetTimer()
	{
		if (currentState == AiState.SearchForCharacter)
		{
			timer += Time.deltaTime;
			if (!inSleepState && timer > searchForCharacterDuration)
			{
				inSleepState = true;
				if (resetSleepCoroutine != null)
					StopCoroutine(resetSleepCoroutine);
				resetSleepCoroutine = StartCoroutine(ResetSleepState());
			}

		}
		else if (timer > 0)
			timer = 0;
	}
	private IEnumerator ResetSleepState()
	{
		yield return new WaitForSeconds(sleepAfterSearchingForCharacter);
		inSleepState = false;
	}
	private void ExecuteCurrentState()
	{
		agent.updateRotation = currentState != AiState.SearchForCharacter;

		switch (currentState)
		{
			case AiState.Idle:
				DoMove();
				break;
			case AiState.ChasingCharacter:
				MoveToCharacter();
				break;

			case AiState.SearchForCharacter:
				SearchForCharacter();
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
				bool canSeeCharacter = CanSeeCharacter();
				mainCharacter = (canSeeCharacter) ? mainCharacter : null;
				return canSeeCharacter;
			case AiAction.CharacterIsInSight:
				bool isInSight = CharacterIsInSight();
				mainCharacter = (isInSight) ? mainCharacter : null;
				return isInSight;
			case AiAction.CanFeelCharacter:
				return CanFeelCharacter();
			default:
				return false;
		}
	}

	private void DoMove()
	{
		agent.SetDestination(currentWaypoint.GetPosition());
		enemyRenderer.material.color = defaultColor;
		if (Vector3.Distance(currentWaypoint.GetPosition(), transform.position) <= agent.stoppingDistance)
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
			if (Physics.Raycast(leftEye.transform.position, leftEye.forward, out RaycastHit leftHit, detectionRangeToSee))
			{
				if (leftHit.transform.gameObject.layer == characterLayerForRayCast)
					mainCharacter = leftHit.transform;
			}



			if (Physics.Raycast(rightEye.transform.position, rightEye.forward, out RaycastHit rightHit, detectionRangeToSee))
			{
				if (rightHit.transform.gameObject.layer == characterLayerForRayCast)
					mainCharacter = rightHit.transform;
			}
			if (mainCharacter != null)
				EventManager.Instance.SendGlobalEvent("EnemySeeCharacter", null);

			return mainCharacter != null;
		}
		else
		{
			return Vector3.Distance(mainCharacter.transform.position, transform.position) <= detectionRangeToSee;
		}
	}

	private bool CanFeelCharacter()
	{
		if (!inSleepState && timer < searchForCharacterDuration)
		{
			Collider[] nearColliders = Physics.OverlapSphere(transform.position, detectionRangeToFeel, characterLayerMask);
			foreach (var item in nearColliders)
			{
				if (item.gameObject.layer == characterLayerForRayCast)
				{
					characterFeelPosition = item.transform.position;
					return true;
				}
			}
			characterFeelPosition = Vector3.zero;
			return false;
		}
		else
			return false;
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
		{
			enemyRenderer.material.color = seeCharacterColor;
			agent.SetDestination(mainCharacter.transform.position);
		}
	}

	private void SearchForCharacter()
	{
		agent.SetDestination(transform.position);
		enemyRenderer.material.color = feelCharacterColor;
		RotateTowardsCharacter();

	}

	private void RotateTowardsCharacter()
	{
		Vector3 directionToCharacter = (characterFeelPosition - transform.position).normalized;

		// Calculate the rotation to face the character
		Quaternion targetRotation = Quaternion.LookRotation(directionToCharacter);

		// Smoothly rotate towards the target rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, searchForCharacterRotationSpeed * Time.deltaTime);

	}
}
