using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public EnemyController m_controller;
    public bool m_stomped = false;
    public bool m_kaboomed = false;
    public bool m_stompedDone = false;
    public bool m_kaboomedDone = false;
    
    
    // Update is called once per frame
    private void Update()
    {
        if (m_stomped)
        {
            if (!m_stompedDone)
            {
                OnStompedEnded();
                m_stompedDone = true;
            }
        }
        else
        {
            m_stompedDone = false;
        }
        if (m_kaboomed)
        {
            if (!m_kaboomedDone)
            {
                OnKaboomedEnded();
                m_kaboomedDone = false;
            }
        }
        else
        {
            m_kaboomedDone = false;
        }
    }
    
    public void OnStompedEnded()
    {
        m_controller.Stomped();
    }
    
    public void OnKaboomedEnded()
    {
        m_controller.Exploded();
    }
}
