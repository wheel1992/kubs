using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class TerrainReplacer : MonoBehaviour
	{
		public NamedPrefab[] prefabs;

		void Start()
		{
			var dictionary = Convert(prefabs);
			ReplaceChildren(transform, dictionary);
		}

		private Dictionary<string, GameObject> Convert(NamedPrefab[] namedPrefabs)
		{
			var dictionary = new Dictionary<string, GameObject>();

			foreach (var namedPrefab in namedPrefabs)
			{
				dictionary[namedPrefab.name] = namedPrefab.prefab;
			}

			return dictionary;
		}

		private void ReplaceChildren(Transform parent, Dictionary<string, GameObject> dictionary)
		{
			foreach (Transform child in parent)
			{
				GameObject prefab;

				if (dictionary.TryGetValue(child.gameObject.tag, out prefab))
				{
					Instantiate(prefab,
								child.position,
								child.rotation,
								parent);

					Destroy(child.gameObject);
				}
			}
		}

		[Serializable]
		public struct NamedPrefab
		{
			public string name;
			public GameObject prefab;
		}
	}
}
