using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The loading game stage. When in this game stage the level of another game stage will be loading
/// </summary>
public class GameStageLoading
{

	private List<AsyncOperation> m_loadLevelAsyncOperations = new List<AsyncOperation>();
#if DEBUG
	private float m_minimumLoadingTime = 0f;
#endif

	public void OnUpdate()
	{
#if DEBUG
		m_minimumLoadingTime += Time.deltaTime;
#endif
	}

	public void StartLoading(List<string> levelNames)
	{
		int sceneIndex = 0;
		foreach (string levelName in levelNames)
		{
			var asyncOperation = SceneManager.LoadSceneAsync(levelName, sceneIndex == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
			asyncOperation.allowSceneActivation = false;
			m_loadLevelAsyncOperations.Add(asyncOperation);
			sceneIndex++;
		}

#if DEBUG
		m_minimumLoadingTime = 0f;
#endif
	}

	public bool IsLoadingFinished()
	{
#if DEBUG
		if (m_minimumLoadingTime < 1.5f)
		{
			return false;
		}
#endif

		foreach (var loadLevelAsyncOperation in m_loadLevelAsyncOperations)
		{
			if (loadLevelAsyncOperation.progress < 0.9f)
			{
				return false;
			}
			loadLevelAsyncOperation.allowSceneActivation = true;	// This scene is loaded, activate it
		}

		foreach (var loadLevelAsyncOperation in m_loadLevelAsyncOperations)
		{
			if (!loadLevelAsyncOperation.isDone)
			{
				return false;
			}
		}

		return true;
	}

	public void ActivateLoadedLevel()
	{
		//
		// Finally activate the scene
		foreach (AsyncOperation asyncOperation in m_loadLevelAsyncOperations)
		{
			asyncOperation.allowSceneActivation = true;
		}

		//
		// Clear the array
		m_loadLevelAsyncOperations.Clear();
	}
}
