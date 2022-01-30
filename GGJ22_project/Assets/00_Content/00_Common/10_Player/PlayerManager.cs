using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The positions where players will spawn")]
	private GameObject[] playerSpawn;

	[SerializeField]
	[Tooltip("The prefabs to use to instantiate players")]
	private GameObject[] playerPrefabs;

	[Header("Camera")]
	[SerializeField]
	[Tooltip("Prefab of a player camera. Will be instantiated and assigned to each player to follow him correctly")]
	private GameObject m_virtualCameraPrefab;

	/// <summary>
	/// The number of players
	/// </summary>
	private int playerCounter = 0;

	
	private PlayerInputManager inputManager;

    private void Start()
    {
		inputManager = GetComponent<PlayerInputManager>();
	}

    void OnPlayerJoined(PlayerInput playerInput)
	{
		//
		// Debug text
		Debug.Log("Player joined with control scheme : " + playerInput.currentControlScheme);

		//
		// Place the player
		playerInput.gameObject.transform.position = playerSpawn[playerCounter].transform.position;
		playerCounter++;
		playerCounter %= playerPrefabs.Length;
		inputManager.playerPrefab = playerPrefabs[playerCounter];

		//
		// Rename the player
		playerInput.transform.name = "Player " + playerCounter + "(" + playerInput.currentControlScheme + ")";

		//
		// Instantiate a new camera
		GameObject playerVirtualCamera = GameObject.Instantiate(m_virtualCameraPrefab) as GameObject;
		playerVirtualCamera.name = "Virtual Camera - " + playerInput.transform.name;

		//
		// Set the camera options (mainly the follow)
		playerVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = playerInput.transform;
		playerVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = playerInput.transform;

		//
		// Notify the character selection manager
		CharacterSelectionManager.Instance.JoinPlayer(playerInput, playerVirtualCamera.GetComponent<CinemachineVirtualCamera>());
	}
}
