using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class StageTrigger : MonoBehaviour
	{
		public bool autoSetNextStage = true;
		public int stage;

		void Start()
		{
			if (autoSetNextStage)
			{
				stage = StagesManager.GetActiveStage() + 1;
			}
		}

		void OnTriggerEnter(Collider other)
        {
			if (other.gameObject.name == "Character")
			{
				StagesManager.LoadStage(stage);
			}
		}
	}
}
