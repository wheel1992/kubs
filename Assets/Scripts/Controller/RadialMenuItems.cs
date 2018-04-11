using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kubs
{
    public class RadialMenuItems : MonoBehaviour
    {
        public void OnOptionMenuClick()
        {
			Debug.Log("RadialMenuItems OnOptionMenuClick:");
        }
		public void OnOptionSelectClick()
        {
			Debug.Log("RadialMenuItems OnOptionSelectClick:");
			Debug.Log("RadialMenuItems OnOptionSelectClick: " + transform.root.name);
        }
		public void OnOptionTeleportClick()
        {
			Debug.Log("RadialMenuItems OnOptionTeleportClick:");
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}

