using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
	public float movementSpeed = 5f;
	public float rotationSpeed = 10f;
	public float maxNavMeshSampleDistance = 1.0f;

	public int score = 0;

	[SerializeField] private NavMeshAgent agent;

	void Start()
	{
		if (agent == null)
			agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;

		EventManager.Instance.RegisterGlobalEvent("CharacterPassedCheckPointsWithoutBeingSeen", CharacterPassedCheckPoints);
	}



	void Update()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		Vector3 input = GetInput();
		if (input.sqrMagnitude > 0)
		{
			if (input.z != 0)
				MoveCharacter(input);
			RotateCharacter(input);
		}
	}

	private Vector3 GetInput()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		return new Vector3(horizontal, 0, vertical).normalized;
	}

	private void MoveCharacter(Vector3 input)
	{
		float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
		Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
		Vector3 targetPosition = transform.position + moveDirection * Time.deltaTime * movementSpeed * speedMultiplier;
		if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, maxNavMeshSampleDistance, NavMesh.AllAreas))
			agent.Move(moveDirection * Time.deltaTime * movementSpeed * speedMultiplier);
	}

	private void RotateCharacter(Vector3 input)
	{
		if (input.x != 0)
		{
			float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
			Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
		}
	}

	private void CharacterPassedCheckPoints(object data)
	{
		if (data != null && data is int)
		{
			score += (int)data;
			Debug.Log("CharacterPassedCheckPoints" + score);
		}
	}


	private void OnDestroy()
	{
		EventManager.Instance.RemoveGlobalEvent("CharacterPassedCheckPointsWithoutBeingSeen", CharacterPassedCheckPoints);
	}
}
