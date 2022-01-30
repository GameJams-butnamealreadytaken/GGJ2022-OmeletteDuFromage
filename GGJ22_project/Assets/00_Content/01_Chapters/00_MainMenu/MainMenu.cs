using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Handle the interactions in the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{

	[SerializeField]
	private AudioMixer m_audioMixer;

	[SerializeField] private Slider m_musicVolumeSlider;
	[SerializeField] private Slider m_fxVolumeSlider;

	private void Awake()
	{
		float musicVolume;
		m_audioMixer.GetFloat("Music Volume", out musicVolume);
		m_musicVolumeSlider.value = musicVolume;

		float fxVolume;
		m_audioMixer.GetFloat("FX Volume", out fxVolume);
		m_fxVolumeSlider.value = fxVolume;
	}

	public void Play()
	{
		GameManager.Instance.PushNextStage("Character Selection Screen");
	}

	public void Options()
	{

	}

	public void Exit()
	{
		Application.Quit();
	}

	public void SetMusicVolume(float volume)
	{
		m_audioMixer.SetFloat("Music Volume", volume);
	}


	public void SetFXVolume(float volume)
	{
		m_audioMixer.SetFloat("FX Volume", volume);
	}
}

