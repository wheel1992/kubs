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

		public static void LoadStage(int stage)
		{
			var stageString = stage == 0 ? "" : stage.ToString();
			SceneManager.LoadScene(STAGE_PREFIX + stageString);
		}

		public static void NextStage()
		{
			var sceneName = SceneManager.GetActiveScene().name;
			sceneName = sceneName == CHARACTER_STAGE ? STAGE_PREFIX : sceneName;

			var stageString = sceneName.Substring(STAGE_PREFIX.Length);
			var stage = stageString == "" ? 0 : int.Parse(stageString);

			LoadStage(stage + 1);
		}
	}
}
