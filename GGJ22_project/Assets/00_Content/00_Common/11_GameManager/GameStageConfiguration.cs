using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GameStageInfos
{
	public string m_stageName;
	public string m_sceneName;
}

[CreateAssetMenu(fileName="GameStageConfiguration", menuName="GGJ22/Create Game Stage Configuration")]
public class GameStageConfiguration : ScriptableObject
{
	[Tooltip("The initial game stage when the game boots up (only when not in editor)")]
	public GameStageInfos m_initialGameStage;
	public List<GameStageInfos> m_gameStagesInfos = new List<GameStageInfos>();

}
