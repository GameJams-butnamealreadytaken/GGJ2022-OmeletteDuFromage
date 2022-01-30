using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public enum GameState
	{
		Running,	//< We are running the current gamestate
		Loading		//< We are in loading state
	}

	[SerializeField]
	private Canvas m_loadingScreenCanvas;

	private GameStageConfiguration m_gameStageConfiguration;
	private GameStageInfos m_currentGameStage;
	private GameStageInfos m_nextGameStage;
	private GameStageLoading m_loadingGameStage;
	private GameState m_gameState = GameState.Running;

	/// <summary>
	/// A unity event that is fired when a game stage has been activated. Useful to know when our new scene is active
	/// </summary>
	private UnityAction m_nextStageActivatedCallback = null;

	/// <summary>
	/// Static instance of the GameManager singleton
	/// </summary>
	private static GameManager m_instance = null;

	/// <summary>
	/// Get the singleton instance of the game manager
	/// </summary>
	public static GameManager Instance => m_instance;

	private void Awake()
	{
		//
		// Destroy new instance if there is already an existing instance (should never happen)
		if (null != m_instance && m_instance != this)
		{
			Destroy(gameObject);
		}

		//
		// Store our instance
		m_instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		//
		// Don't destroy this manager
		DontDestroyOnLoad(this);

		//
		// Load the GameStagesConfiguration
		m_gameStageConfiguration = Resources.Load<GameStageConfiguration>("GameStagesConfiguration");

		//
		// Hide the loading screen canvas (and don't destroy it)
		DontDestroyOnLoad(m_loadingScreenCanvas);
		m_loadingScreenCanvas.gameObject.SetActive(false);

		//
		// Create the base game states
		m_loadingGameStage = new GameStageLoading();

		//
		// If we are not in editor, we skip to the initial stage
// #if !UNITY_EDITOR
		StartLoading(new List<string>()
				{
					m_gameStageConfiguration.m_initialGameStage.m_sceneName
				});
// #else
// 		// TODO: Retrieve the correct current game stage depending on loaded level and set it
// 		// m_gameStageConfiguration.m_initialGameStage =
// #endif
	}

	// Update is called once per frame
	void Update()
	{
		if (m_gameState == GameState.Loading)
		{
			//
			// Update loading game stage
			m_loadingGameStage.OnUpdate();

			//
			// Check if loading is finished
			if (m_loadingGameStage.IsLoadingFinished())
			{
				//
				//
				m_loadingGameStage.ActivateLoadedLevel();

				//
				// We now are running the current game state, we also deactivate the loading canvas
				m_gameState = GameState.Running;
				m_loadingScreenCanvas.gameObject.SetActive(false);
				
				//
				// The next game stage is now the current
				 m_currentGameStage = m_nextGameStage;

				//
				// Call the unity event to inform that the stage is activated
				if (null != m_nextStageActivatedCallback)
				{
					m_nextStageActivatedCallback.Invoke();
					m_nextStageActivatedCallback = null;	// Reset this to avoid wrongly firing events
				}
			}
		}
	}

	private void OnGUI()
	{
		//
		//
		// foreach (GameStageInfos gameStageInfos in m_gameStageConfiguration.m_gameStagesInfos)
		// {
		// 	if (GUILayout.Button(gameStageInfos.m_stageName))
		// 	{
		// 		PushNextStage(gameStageInfos.m_stageName);
		// 	}
		// }
	}


	public void PushNextStage(string gameStageName, UnityAction stageActivatedCallback = null)
	{
		bool gameStageFound = false;
		m_nextStageActivatedCallback = stageActivatedCallback;

		foreach (GameStageInfos gameStageInfos in m_gameStageConfiguration.m_gameStagesInfos)
		{
			if (gameStageInfos.m_stageName == gameStageName)
			{
				gameStageFound = true;
				m_nextGameStage = gameStageInfos;
				StartLoading(new List<string>()
				{
					gameStageInfos.m_sceneName
				});
				break;
			}
		}

		if (!gameStageFound)
		{
			Debug.LogError("Trying to push unknown game stage : " + gameStageName);
		}
	}

	public void PushNextStageWithAdditionalScenes(string gameStageName, List<string> additionalScenes, UnityAction stageActivatedCallback)
	{
		m_nextStageActivatedCallback = stageActivatedCallback;

		foreach (GameStageInfos gameStageInfos in m_gameStageConfiguration.m_gameStagesInfos)
		{
			if (gameStageInfos.m_stageName == gameStageName)
			{
				m_nextGameStage = gameStageInfos;
				List<string> scenesToLoad = new List<string>();
				scenesToLoad.Add(gameStageInfos.m_sceneName);
				scenesToLoad.AddRange(additionalScenes);
				StartLoading(scenesToLoad);
				break;
			}
		}
	}

	private void StartLoading(List<string> sceneNames)
	{
		m_gameState = GameState.Loading;
		m_loadingGameStage.StartLoading(sceneNames);
		m_loadingScreenCanvas.gameObject.SetActive(true);
	}
	
}
