using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class StageTrigger : MonoBehaviour
	{
		public bool isTriggered;

		void OnTriggerEnter(Collider other)
        {
			if (other.gameObject.name == "Character")
			{
				if (isTriggered) return;
				isTriggered = true;
				SendMessageUpwards("OnChildTriggerEnter", this);
			}
		}
	}
}
