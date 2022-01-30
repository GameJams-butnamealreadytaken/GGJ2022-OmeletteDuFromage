using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic game stage
/// Each stage represent a level
/// </summary>
public abstract class GameStage
{

	/// <summary>
	/// Called when we enter this game stage
	/// </summary>
	public abstract void OnEnter();

	/// <summary>
	/// Called when we exit this game stage
	/// </summary>
	public abstract void OnExit();

	public abstract void OnUpdate();

}
