using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{

	[SerializeField] private PlayerSpawnPoint m_winnerPlayerSpawnPoint;
	[SerializeField] private PlayerSpawnPoint m_loserPlayerSpawnPoint;

	public PlayerSpawnPoint WinnerPlayerSpawnPoint => m_winnerPlayerSpawnPoint;
	public PlayerSpawnPoint LoserPlayerSpawnPoint => m_loserPlayerSpawnPoint;
}
