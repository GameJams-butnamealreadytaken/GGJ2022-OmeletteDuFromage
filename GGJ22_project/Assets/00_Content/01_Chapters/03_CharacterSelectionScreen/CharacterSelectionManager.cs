using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInstance
{
	public PlayerInput m_playerInput;
	public CinemachineVirtualCamera m_playerVirtualCamera;
}

/// <summary>
/// Manager for the character selection level
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{

	public enum CharacterSelectionState
	{
		WaitingForPlayers,	//< Waiting for both players to join
		StartGameCountdown,	//< The countdown before starting the game
		StartingGame,	// The game is starting (loading screen before arriving in the game scene)
		GameRunning,	// The game is running
		GameFinished,	// The game is finished, we will load the end game
		EndGame	// Inside the end game
	}

	public enum SplitScreenType
	{
		Horizontal,
		Vertical
	}

	/// <summary>
	/// Static instance of the CharacterSelectionManager singleton
	/// </summary>
	private static CharacterSelectionManager m_instance = null;

	/// <summary>
	/// Get the singleton instance of the character selection manager
	/// </summary>
	public static CharacterSelectionManager Instance => m_instance;

	[Header("GUI")]
	[SerializeField]
	[Tooltip("The text used to display the state of the character selection screen")]
	private TMP_Text m_stateText;

	[Header("Texts")]
	
	[Header("Configuration")]
	[SerializeField]
	[Tooltip("The time before starting the game level when all players have joined")]
	private float m_startingGameDuration = 6f;

	[SerializeField] [Tooltip("The empty that holds the players")]
	private GameObject m_playersHolder;

	[SerializeField]
	[Tooltip("Select the type of split screen horizontal or vertical")]
	private SplitScreenType m_splitScreenType = SplitScreenType.Vertical;

	[Header("Music")] [SerializeField] [Tooltip("The in game music")]
	private AudioClip m_inGameAudioClip;
	[SerializeField]
	[Tooltip("The leaderboard music")]
	private AudioClip m_leaderboardGameAudioClip;

	/// <summary>
	/// The list of the joined players (joined with the Player Input Manager)
	/// </summary>
	private List<PlayerInstance> m_joinedPlayers = new List<PlayerInstance>();

	/// <summary>
	/// The current state of the character selection
	/// </summary>
	private CharacterSelectionState m_characterSelectionState = CharacterSelectionState.WaitingForPlayers;
	private float m_startingGameElapsedTime = 0f;	// The time since the starting game state ... started
	private float m_gameFinishedElapsedTime = 0f;	// The time since the game finish began

	public bool IsGameRunning => m_characterSelectionState == CharacterSelectionState.GameRunning;

	private void Awake()
	{
		//
		// Destroy new instance if there is already an existing instance (should never happen)
		if (null != m_instance && m_instance != this)
		{
			Destroy(m_instance);
		}

		//
		// Store our instance
		m_instance = this;

		//
		// Don't destroy it on load
		DontDestroyOnLoad(this);
	}

	private void Update()
	{
		//
		//
		switch (m_characterSelectionState)
		{
			case CharacterSelectionState.WaitingForPlayers:
			{
				m_stateText.text = "Waiting for players \n(press any key on gamepad or keyboard to join)";
			}break;

			case CharacterSelectionState.StartGameCountdown:
			{
				//
				// Update time and display remaining duration
				m_stateText.text = "Starting game in " + Mathf.FloorToInt(m_startingGameDuration - m_startingGameElapsedTime) + " ...";
				m_startingGameElapsedTime += Time.deltaTime;

				//
				// Check if a character has left, we go back to waiting players
				if (m_joinedPlayers.Count < 2)
				{
					m_characterSelectionState = CharacterSelectionState.WaitingForPlayers;
				}

				//
				// Check if we finished waiting
				if (m_startingGameElapsedTime >= m_startingGameDuration)
				{
					//
					// Switch to game level
					GameManager.Instance.PushNextStageWithAdditionalScenes("Game", new List<string>()
					{
						"LevelGeometry_Base",
						"02_Game_Lighting"
					},OnGameStarted);

					//
					// We now are starting the game
					m_characterSelectionState =  CharacterSelectionState.StartingGame;

					//
					// Hide the state text
					m_stateText.text = "";

				}
			}break;

			case CharacterSelectionState.StartingGame:
			{
				// Nothing special, the level is loading and game starting
			}break;

			case CharacterSelectionState.GameRunning:
			{
				if (!GetComponentInChildren<AudioSource>().isPlaying)
				{
					GetComponentInChildren<AudioSource>().Stop();
					GetComponentInChildren<AudioSource>().clip = m_inGameAudioClip;
					GetComponentInChildren<AudioSource>().Play();
				}
				// Nothing to do the game is running
			}break;

			case CharacterSelectionState.GameFinished:
			{
				m_gameFinishedElapsedTime += Time.deltaTime;

				if (m_gameFinishedElapsedTime >= 8f)
				{
					GameManager.Instance.PushNextStage("End Game", OnEndGame);
					m_characterSelectionState = CharacterSelectionState.EndGame;

					foreach (PlayerInstance playerInstance in m_joinedPlayers)
					{
						//
						// Hide win and lose screen (hide both, we don't care which one is actually displayed)
						playerInstance.m_playerInput.GetComponent<PlayerController>().HideLoseScreen();
						playerInstance.m_playerInput.GetComponent<PlayerController>().HideWinScreen();
					}

					//
					// Stop the in game music and play the leaderboard music
					GetComponentInChildren<AudioSource>().Stop();
					GetComponentInChildren<AudioSource>().clip = m_leaderboardGameAudioClip;
					GetComponentInChildren<AudioSource>().Play();
				}
			}break;

			case CharacterSelectionState.EndGame:
			{
				// ...
			}break;
		}
	}

	private void OnGameStarted()
	{
		//
		// The game started we can change state
		m_characterSelectionState = CharacterSelectionState.GameRunning;

		//
		// Deactivate the players tutorials
		m_joinedPlayers[0].m_playerInput.GetComponent<PlayerController>().HideTutorial();
		m_joinedPlayers[1].m_playerInput.GetComponent<PlayerController>().HideTutorial();

		//
		// Retrieve the arena configuration
		Arena arena = GameObject.FindObjectOfType<Arena>();
		if (null == arena)
		{
			Debug.LogError("No arena in this level!");
			return;
		}

		//
		// Teleport the players to the spawn points (as well as the cameras)
		Vector3 topPlayerOldPosition = m_joinedPlayers[0].m_playerInput.transform.position;
		Vector3 bottomPlayerOldPosition = m_joinedPlayers[1].m_playerInput.transform.position;
		m_joinedPlayers[0].m_playerVirtualCamera.OnTargetObjectWarped(m_joinedPlayers[0].m_playerInput.transform, topPlayerOldPosition - arena.ArenaTopPlayerSpawnPoint.transform.position);
		m_joinedPlayers[1].m_playerVirtualCamera.OnTargetObjectWarped(m_joinedPlayers[1].m_playerInput.transform, bottomPlayerOldPosition - arena.ArenaBottomPlayerSpawnPoint.transform.position);
		m_joinedPlayers[0].m_playerInput.transform.position = arena.ArenaTopPlayerSpawnPoint.transform.position;
		m_joinedPlayers[1].m_playerInput.transform.position = arena.ArenaBottomPlayerSpawnPoint.transform.position;

		//
		// Configure the enemy managers
		arena.ArenaTopEnemyManager.m_player = m_joinedPlayers[0].m_playerInput.gameObject;
		arena.ArenaTopEnemyManager.OppositeEnemyManager = arena.ArenaBottomEnemyManager;
		arena.ArenaTopEnemyManager.m_isBottom = false;
		arena.ArenaBottomEnemyManager.m_player = m_joinedPlayers[1].m_playerInput.gameObject;
		arena.ArenaBottomEnemyManager.OppositeEnemyManager = arena.ArenaTopEnemyManager;
		arena.ArenaBottomEnemyManager.m_isBottom = true;
	}

	private void OnEndGame()
	{
		//
		// Deactivate the cameras of the player
		m_joinedPlayers[0].m_playerVirtualCamera.gameObject.SetActive(false);
		m_joinedPlayers[1].m_playerVirtualCamera.gameObject.SetActive(false);
		m_joinedPlayers[0].m_playerInput.GetComponentInChildren<Camera>().gameObject.SetActive(false);
		m_joinedPlayers[1].m_playerInput.GetComponentInChildren<Camera>().gameObject.SetActive(false);

		//
		// Retrieve the end game system and spawn points
		EndGame endGame = GameObject.FindObjectOfType<EndGame>();
		if (m_joinedPlayers[0].m_playerInput.GetComponent<PlayerController>().HasWon)
		{
			m_joinedPlayers[0].m_playerInput.transform.position = endGame.WinnerPlayerSpawnPoint.transform.position;
			m_joinedPlayers[1].m_playerInput.transform.position = endGame.LoserPlayerSpawnPoint.transform.position;
		}
		else
		{
			m_joinedPlayers[1].m_playerInput.transform.position = endGame.WinnerPlayerSpawnPoint.transform.position;
			m_joinedPlayers[0].m_playerInput.transform.position = endGame.LoserPlayerSpawnPoint.transform.position;
		}

		//
		// Set the palyers forward
		Quaternion rotation = Quaternion.LookRotation(-Camera.main.transform.forward, gameObject.transform.up);
		m_joinedPlayers[0].m_playerInput.transform.rotation = rotation;
		m_joinedPlayers[1].m_playerInput.transform.rotation = rotation;

		//
		// Inform the players we are in leaderboard
		m_joinedPlayers[0].m_playerInput.GetComponent<PlayerController>().ActivateLeaderboard();
		m_joinedPlayers[1].m_playerInput.GetComponent<PlayerController>().ActivateLeaderboard();
	}

	public void EndLeaderboardAndGoToMainMenu()
	{
		GameManager.Instance.PushNextStage("Main Menu");

		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
		Destroy(gameObject);
	}

	public void JoinPlayer(PlayerInput joinedPlayer, CinemachineVirtualCamera playerVirtualCamera)
	{
		//
		// Notify a player joined
		Debug.Log("[Character Selection Manager] : Player joined (" + joinedPlayer.currentControlScheme + ")");

		//
		// Ensure we don't have more than 2 players (arbitrary value here, because the game is only 2 players)
		if (m_joinedPlayers.Count < 2)
		{
			m_joinedPlayers.Add(new PlayerInstance()
			{
				m_playerInput =  joinedPlayer,
				m_playerVirtualCamera = playerVirtualCamera
			});

			//
			// Add the player and the camera in the players holder
			joinedPlayer.transform.parent = m_playersHolder.transform;
			playerVirtualCamera.transform.parent = m_playersHolder.transform;

			//
			// Set the correct layer
			if (m_joinedPlayers.Count == 1)
			{
				joinedPlayer.GetComponentInChildren<Camera>().cullingMask = joinedPlayer.GetComponentInChildren<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("SplitScreen_Player2"));
				playerVirtualCamera.gameObject.layer = LayerMask.NameToLayer("SplitScreen_Player1");
			}
			else if (m_joinedPlayers.Count == 2)
			{
				joinedPlayer.GetComponentInChildren<Camera>().cullingMask = joinedPlayer.GetComponentInChildren<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("SplitScreen_Player1"));
				playerVirtualCamera.gameObject.layer = LayerMask.NameToLayer("SplitScreen_Player2");
			}
			else
			{
				Debug.LogError("Too many players added in Character Selection Manager");
			}

			//
			//
			if (m_joinedPlayers.Count >= 2)
			{
				//
				// Set the camera rects correctly
				//
				// Vertical split
				if (m_splitScreenType == SplitScreenType.Vertical)
				{
					m_joinedPlayers[0].m_playerInput.GetComponentInChildren<Camera>().rect = new Rect(0f, 0f, 0.5f, 1f);
					m_joinedPlayers[1].m_playerInput.GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0f, 1f, 1f);
				}
				//
				// Horizontal split
				else
				{
					m_joinedPlayers[0].m_playerInput.GetComponentInChildren<Camera>().rect = new Rect(0f, 0.5f, 1f, 0.5f);
					m_joinedPlayers[1].m_playerInput.GetComponentInChildren<Camera>().rect = new Rect(0f, 0f, 1f, 0.5f);
				}

				m_startingGameElapsedTime = 0f;	// Reset the elapsed time
				m_characterSelectionState = CharacterSelectionState.StartGameCountdown;	// Set the new state
			}
		}
	}

	public void NotifyPlayerLost(PlayerInput loserPlayer)
	{
		//
		// Display the screen (win or lose) of the players, and animate their victory or death
		foreach (PlayerInstance playerInstance in m_joinedPlayers)
		{
			if (playerInstance.m_playerInput == loserPlayer)
			{
				playerInstance.m_playerInput.GetComponent<PlayerController>().DisplayLoseScreen();
				// TODO: Animate death
			}
			else
			{
				playerInstance.m_playerInput.GetComponent<PlayerController>().DisplayWinScreen();
				// TODO: Animate victory
			}
		}

		//
		// Switch to game finished screen
		m_characterSelectionState = CharacterSelectionState.GameFinished;
		m_gameFinishedElapsedTime = 0f;
	}

}
