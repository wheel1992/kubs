using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class Menu : MonoBehaviour
	{
		public MenuItem[] stages;

		private Decoder _decoder;

		void Start()
		{
			_decoder = GameObject.FindGameObjectWithTag(Constant.TAG_LEVEL).GetComponent<Decoder>();
		}

		public void ShowMedal(int stage)
		{
			if (stage == 0)
			{
				return;
			}

			stages[stage].ShowMedal(_decoder.lastSolutionCount);
		}
	}
}
