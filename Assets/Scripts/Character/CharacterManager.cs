using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class CharacterManager : MonoBehaviour
	{
		public GameObject bearPrefab;
		public GameObject bunnyPrefab;
		public GameObject catPrefab;

		public GameObject activePrefab
		{
			get
			{
				return bearPrefab.activeSelf ? bearPrefab :
						bunnyPrefab.activeSelf ? bunnyPrefab :
						catPrefab.activeSelf ? catPrefab :
						_activePrefab;
			}
		}
		private GameObject _activePrefab;

		void Start()
		{
			// Default;
			_activePrefab = bearPrefab;
		}
	}
}
