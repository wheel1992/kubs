using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kubs
{
    public static class StagesManager
    {
        public static Vector3 loadPos { get; set; }
        public static Vector3 loadScale { get; set; }

        private const string CHARACTER_STAGE = "CharacterScene";
        private const string STAGE_PREFIX = "KubScene";
        private const string WIN_SCENE = STAGE_PREFIX + "WIN";

        private const int WIN_STAGE = 5;

        public static int GetActiveStage()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            sceneName = sceneName == CHARACTER_STAGE ? STAGE_PREFIX : sceneName;

            var stageString = sceneName.Substring(STAGE_PREFIX.Length);
            return stageString == "" ? -1 : int.Parse(stageString);
        }

        private static int? TryGetActiveStage()
        {
            try
            {
                return GetActiveStage();
            }
            catch
            {
                return null;
            }
        }

        public static void LoadStageAsync(int stage, MonoBehaviour monoBehaviour)
        {
            if (stage == TryGetActiveStage())
            {
                return;
            }

            if (stage == WIN_STAGE)
            {
                monoBehaviour.StartCoroutine(LoadSceneAsync(WIN_SCENE));
                return;
            }

            var stageString = stage.ToString();
            monoBehaviour.StartCoroutine(LoadSceneAsync(STAGE_PREFIX + stageString));
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

			ResetZones(nextScene);

            // Unload the previous Scene
            SceneManager.UnloadSceneAsync(currentScene);

            EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_DISABLE, null);
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
            loadScale = loadScale == Vector3.zero ? Vector3.one : loadScale;

            // Destroy non-GameArea objects in scene
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name == "GameArea")
                {
                    go.transform.localScale = loadScale;
                    go.transform.position = loadPos;

                    // EventManager.TriggerEvent(Constant.EVENT_NAME_GAME_AREA_DID_SCALE, loadScale); // Not working
                    go.GetComponentInChildren<Character>()._scale = loadScale.x;
                }
                else
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

        private static void ResetZones(Scene scene)
        {
            foreach (var go in GetGameObjectsFromScene(scene))
            {
				if (go.name.CompareTo("PlayableObject") == 0) {
					var zonesTransform = go.transform.Find(Constant.NAME_ZONES);
					if (zonesTransform != null) {
						zonesTransform.gameObject.GetComponent<ZoneGroupController>().Reset();
						return;
					}
				}
            }
        }
    }
}
