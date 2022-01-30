using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// REF :
// https://gist.github.com/jagwire/0129d50778c8b4462b68


//$ Unity -quit -batchmode -executeMethod WebGLBuilder.build
class WebGLBuilder
{

	static void Build()
	{
		//
		// Build all the scenes
		List<string> scenes = new List<string>();
		for (int sceneIndexInBuildSettings = 0;
		     sceneIndexInBuildSettings < SceneManager.sceneCountInBuildSettings;
		     ++sceneIndexInBuildSettings)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndexInBuildSettings);
			scenes.Add(scenePath);
		}

		//
		//
		// BuildReport buildReport = BuildPipeline.BuildPlayer(scenes.ToArray(), "Assets/../../WebGL_Build", BuildTarget.WebGL, BuildOptions.Development);
		BuildReport buildReport = BuildPipeline.BuildPlayer(scenes.ToArray(), "Assets/../../WebGL_Build", BuildTarget.WebGL, BuildOptions.None);
		BuildSummary buildSummary = buildReport.summary;

		//
		// Check if build succeeded
		if (buildSummary.result != BuildResult.Succeeded)
		{
			Debug.LogError("Build failed!" );
			EditorApplication.Exit(-1);
		}
		else
		{
			EditorApplication.Exit(0);
			Debug.Log("Build succeeded!");
		}
	}
}