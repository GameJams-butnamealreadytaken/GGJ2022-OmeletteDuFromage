using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Object m_enemyPrefab;

    [SerializeField] public GameObject m_player;
    
    [SerializeField] public bool m_isBottom;
    
    [SerializeField] private float m_spawnDelay = 10;

    [SerializeField] private List<GameObject> m_spawners;

    private float m_lastSpawn = 0;

    /// <summary>
    /// The opposite enemy manager
    /// </summary>
    private EnemyManager m_oppositeEnemyManager;

    public EnemyManager OppositeEnemyManager
    {
        get => m_oppositeEnemyManager;
        set
        {
            m_oppositeEnemyManager = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( m_player && m_oppositeEnemyManager.m_player && (Time.time - m_lastSpawn) > m_spawnDelay )
        {
            GameObject spawner = m_spawners.Count <= 0 ? gameObject : m_spawners[Random.Range(0, m_spawners.Count)]; 
            
            GameObject generatedObject = Instantiate(m_enemyPrefab) as GameObject;
            generatedObject.GetComponent<NavMeshAgent>().Warp(spawner.transform.position);
            //generatedObject.transform.position = spawner.transform.position;
            generatedObject.GetComponent<EnemyController>().SetTarget(m_player);
            generatedObject.GetComponent<EnemyController>().m_manager = this;
            
            int layer = m_isBottom ? LayerMask.NameToLayer("NotAffectedByExteriorLight") : 0;
        
            foreach (Transform trans in generatedObject.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
            
            m_lastSpawn = Time.time;
        }
    }

    public void OnEnemyDied(GameObject enemy)
    {
        m_oppositeEnemyManager.ReceiveEnemy(enemy);
    }

    private void ReceiveEnemy(GameObject enemy)
    {
        
        int layer = m_isBottom ? LayerMask.NameToLayer("NotAffectedByExteriorLight") : 0;
        
        foreach (Transform trans in enemy.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layer;
        }
        
        EnemyController controller = enemy.GetComponent<EnemyController>();
        controller.m_manager = this;
        Vector3 oldPosition = enemy.transform.position;
        Vector3 oldManagerPosition = m_oppositeEnemyManager.transform.position;
        Vector3 newPosition = oldPosition + (transform.position - oldManagerPosition);
        newPosition.z -= (oldPosition.z - oldManagerPosition.z) * 2;
        enemy.GetComponent<NavMeshAgent>().Warp(newPosition);
        controller.SetTarget(m_player);
        controller.Revive();
    }
}
