using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kubs
{
	public static class StagesManager
	{
		private const string CHARACTER_STAGE = "CharacterScene";
		private const string STAGE_PREFIX = "KubScene";

		public static int GetActiveStage()
		{
			var sceneName = SceneManager.GetActiveScene().name;
			sceneName = sceneName == CHARACTER_STAGE ? STAGE_PREFIX : sceneName;

			var stageString = sceneName.Substring(STAGE_PREFIX.Length);
			return stageString == "" ? 0 : int.Parse(stageString);
		}

		public static void LoadStage(int stage)
		{
			var stageString = stage == 0 ? "" : stage.ToString();
			SceneManager.LoadScene(STAGE_PREFIX + stageString);
		}

		public static void LoadStageAsync(int stage, MonoBehaviour monoBehaviour)
		{
			var stageString = stage == 0 ? "" : stage.ToString();
			monoBehaviour.StartCoroutine(LoadSceneAsync(STAGE_PREFIX + stageString));
		}

		public static void NextStage()
		{
			var stage = GetActiveStage();
			LoadStage(stage + 1);
		}

		private static IEnumerator LoadSceneAsync(string sceneName)
	    {
	        // Set the current Scene to be able to unload it later
	        Scene currentScene = SceneManager.GetActiveScene();

			// Collect game objects to move
			var gameObjectsToMove = GetGameObjectsFromScene(currentScene);

	        // The Application loads the Scene in the background at the same time as the current Scene.
	        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

	        // Wait until the last operation fully loads to return anything
	        while (!asyncLoad.isDone)
	        {
	            yield return null;
	        }

			// Move game objects
			var nextScene = SceneManager.GetSceneByName(sceneName);
			MoveGameObjectsToScene(gameObjectsToMove, nextScene);

	        // Unload the previous Scene
	        SceneManager.UnloadSceneAsync(currentScene);
	    }

		private static List<GameObject> GetGameObjectsFromScene(Scene scene)
		{
			var gameObjectsToMove = new List<GameObject>();

			// Collect non-GameArea objects in scene
			foreach (var go in scene.GetRootGameObjects())
			{
				if (go.name != "GameArea")
				{
					gameObjectsToMove.Add(go);
				}
			}

			return gameObjectsToMove;
		}

		private static void MoveGameObjectsToScene(List<GameObject> gameObjects, Scene scene)
		{
			// Destroy non-GameArea objects in scene
			foreach (var go in scene.GetRootGameObjects())
			{
				if (go.name != "GameArea")
				{
					Object.Destroy(go);
				}
			}

			// Move List<GameObject> to scene
			foreach (var go in gameObjects)
			{
				SceneManager.MoveGameObjectToScene(go, scene);
			}
		}
	}
}
