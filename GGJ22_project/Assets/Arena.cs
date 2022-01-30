using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent an arena and its configuration
/// </summary>
public class Arena : MonoBehaviour
{

	[Header("Player Spawn Points")]
	[SerializeField]
	private PlayerSpawnPoint m_arenaTopPlayerSpawnPoint;
	[SerializeField]
	private PlayerSpawnPoint m_arenaBottomPlayerSpawnPoint;

	[Header("Enemy Managers")]
	[SerializeField]
	private EnemyManager m_arenaTopEnemyManager;
	[SerializeField]
	private EnemyManager m_arenaBottomEnemyManager;

	public PlayerSpawnPoint ArenaTopPlayerSpawnPoint => m_arenaTopPlayerSpawnPoint;
	public PlayerSpawnPoint ArenaBottomPlayerSpawnPoint => m_arenaBottomPlayerSpawnPoint;

	public EnemyManager ArenaTopEnemyManager => m_arenaTopEnemyManager;
	public EnemyManager ArenaBottomEnemyManager => m_arenaBottomEnemyManager;

}
