using UnityEngine;

public class Singleton<T> where T : class, new()
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (Application.isEditor && !Application.isPlaying)
				return null;


			if (instance == null)
			{
				instance = new T();
				(instance as Singleton<T>).OnCreateSingleton();
			}

			return instance;
		}
	}

	public void Destroy()
	{
		OnDestroy();
		instance = null;
	}
	private void OnDestroy()
	{
		OnDestroySingleton();
	}

	protected virtual void OnCreateSingleton()
	{
	}

	protected virtual void OnDestroySingleton()
	{
	}
}