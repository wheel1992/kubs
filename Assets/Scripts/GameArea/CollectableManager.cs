﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class CollectableManager : MonoBehaviour
	{
		public TutorialManager tutorialManager;

		void Start ()
		{
			tutorialManager.CollectChildren(transform);
		}
	}
}
