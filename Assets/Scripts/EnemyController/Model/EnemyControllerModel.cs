namespace Model
{
	public enum AiState
	{
		Idle = 0,
		ChasingCharacter = 1
	}

	public enum AiAction
	{
		DoNothing = 0,
		CanSeeCharacter = 1,
		CharacterIsInSight = 2
	}
	[System.Serializable]
	public class AiStateActionScore
	{
		public AiAction action;
		public AiState nextState;
		public int score;
	}
}