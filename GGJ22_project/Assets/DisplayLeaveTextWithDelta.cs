using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLeaveTextWithDelta : MonoBehaviour
{
	[SerializeField]
	private float m_timeBeforeDisplay = 4f;
	private float m_elpasedTime = 0f;

	// Start is called before the first frame update
	void Start()
	{
		GetComponent<TMP_Text>().alpha = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		m_elpasedTime += Time.deltaTime;
		if (m_elpasedTime >= m_timeBeforeDisplay)
		{
			GetComponent<TMP_Text>().alpha = 1f;
		}
	}
}
