using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
	public class VRTK_Teleport : VRTK_BasicTeleport
	{
		public override bool ValidLocation(Transform target, Vector3 destinationPosition)
		{
			if (destinationPosition.y > 0.1)
			{
				return false;
			}

			return base.ValidLocation(target, destinationPosition);
		}
	}
}
