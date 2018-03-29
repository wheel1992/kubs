using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class ZoneHintController : MonoBehaviour
    {
        /// <summary>
        /// This has a parent which is <see cref="ZoneController"></see>
        /// This should contain a box collider which detects
        /// a ProgramBlock when the block hovers over this hint.
        /// 
        /// This should be a transparent object which is not visible.
        /// 
        /// This should contain EventHandlers which can interact with its parent
        ///  
        /// </summary>

		#region Public methods

		#endregion

		#region Private methods
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

       	void OnTriggerExit(Collider other)
        {
        }
        void OnTriggerEnter(Collider other)
        {
        }

		#endregion

    }
}