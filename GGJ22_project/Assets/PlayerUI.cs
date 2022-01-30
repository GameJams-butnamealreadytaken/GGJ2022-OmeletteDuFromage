using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{

	public GameObject m_uiTutorialGamepad;
	public GameObject m_uiTutorialKeyboard;
	public GameObject m_uiTutorialKeyboardWASD;
	public GameObject m_uiTutorialKeyboardZQSD;

	public void ActivateTutorial(PlayerInput playerInput)
	{
		if (playerInput.currentControlScheme == "PlayerKeyboard")
		{
			m_uiTutorialKeyboard.SetActive(true);
			if (Application.systemLanguage == SystemLanguage.French)
			{
				m_uiTutorialKeyboardZQSD.SetActive(true);
			}
			else
			{
				m_uiTutorialKeyboardWASD.SetActive(true);
			}
		}
		else
		{
			m_uiTutorialGamepad.SetActive(true);
		}
	}

	public void DeactivateTutorial()
	{
		m_uiTutorialGamepad.SetActive(false);
		m_uiTutorialKeyboard.SetActive(false);
		m_uiTutorialKeyboardWASD.SetActive(false);
		m_uiTutorialKeyboardZQSD.SetActive(false);
	}
}
