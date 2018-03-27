using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class TutorialBlock : MonoBehaviour
	{
		public int stage;

		// Scale
		private Vector3 startScale;
		private Vector3 endScale;
		private Vector3 _originalScale;

		void Start()
		{
			_originalScale = transform.localScale;
			transform.localScale = Vector3.zero;
		}

		public void Grow()
		{
			startScale = transform.localScale;
			endScale = _originalScale;

			StartCoroutine("UpdateScale");
		}

		private IEnumerator UpdateScale()
		{
			var incrementor = 0f;

			while (transform.localScale != endScale)
			{
				incrementor += Time.deltaTime;
				var currentScale = Vector3.Lerp(startScale, endScale, incrementor);
				transform.localScale = currentScale;
				yield return null;
			}

			yield break;
		}
	}
}
