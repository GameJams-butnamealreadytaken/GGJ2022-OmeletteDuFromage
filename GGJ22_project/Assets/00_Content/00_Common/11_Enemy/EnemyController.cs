using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public GameObject explosionFX;

    [SerializeField] private GameObject m_player;
    [SerializeField] private int m_maxHP = 2;
    [SerializeField] private int m_currentHP = 2;
    [SerializeField] private int m_causedDamages = 1;
    
    [SerializeField] private float m_explosionRange = 5;

    [SerializeField] private List<Object> m_meshTypes;
    public EnemyManager m_manager;

    [Header("Sounds")]
    [SerializeField]
    private List<AudioClip> m_appearAudioClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> m_hitAudioClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> m_explostionAudioClips = new List<AudioClip>();


    
    private GameObject m_mesh;
    private GameObject m_particleSystem;
    private Animator m_animator;
    

    private void Awake()
    {
        m_particleSystem = transform.GetChild(0).gameObject;
        
        m_mesh = Instantiate(m_meshTypes[Random.Range(0, m_meshTypes.Count)], transform) as GameObject;
        m_mesh.transform.position = Vector3.zero;
        m_mesh.GetComponent<EnemyAnimatorController>().m_controller = this;
        m_currentHP = m_maxHP;
        
        m_animator = m_mesh.GetComponent<Animator>();

        GetComponent<AudioSource>().PlayOneShot(m_appearAudioClips[Random.Range(0, m_appearAudioClips.Count)]);
    }

    void FixedUpdate()
    {
        if (m_particleSystem.activeSelf)
        {
            if (m_particleSystem.GetComponent<ParticleSystem>().isStopped)
            {
                Destroy(gameObject);
            }
        }
        else if (m_animator)
        {
            bool isWalking = m_animator.GetCurrentAnimatorStateInfo(0).IsName("MushroomWalkin");
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = isWalking ? m_player.transform.position : transform.position;

            float distance = Vector3.Distance(m_player.transform.position, transform.position);
            m_animator.SetBool("IsCloseToPlayer", distance < m_explosionRange / 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage();
        }
        //else if (other.CompareTag("Player"))
        //{
        //    m_player.GetComponent<PlayerController>().TakeDamage(m_causedDamages);
        //}
    }

    public void TakeDamage()
    {
        if (--m_currentHP <= 0 && m_animator)
        {
            m_animator.SetBool("IsDead", true);
            // TODO: Replace with dead audio clip
            GetComponent<AudioSource>().PlayOneShot(m_hitAudioClips[Random.Range(0, m_hitAudioClips.Count)]);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(m_hitAudioClips[Random.Range(0, m_hitAudioClips.Count)]);
        }
    }

    public void SetTarget(GameObject target)
    {
        m_player = target;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = m_player.transform.position; 
    }

    public void Stomped()
    {
        m_manager.OnEnemyDied(gameObject);
    }

    public void Exploded()
    {
        Destroy(m_mesh);
        explosionFX.SetActive(true);
        
        float distance = Vector3.Distance(m_player.transform.position, transform.position);
        if (distance <= m_explosionRange)
        {
            m_player.GetComponent<PlayerController>().TakeDamage(m_causedDamages);
        }

        GetComponent<AudioSource>().PlayOneShot(m_explostionAudioClips[Random.Range(0, m_explostionAudioClips.Count)]);
    }

    public void Revive()
    {
        m_animator.SetBool("IsDead", false);
        ++m_maxHP;
        m_currentHP = m_maxHP;
        GetComponent<AudioSource>().PlayOneShot(m_appearAudioClips[Random.Range(0, m_appearAudioClips.Count)]);
    }
}
