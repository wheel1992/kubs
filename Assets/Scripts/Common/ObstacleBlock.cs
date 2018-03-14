using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock : MonoBehaviour, Block  {

	#region Lifecycle

	void Awake() 
	{
		Category = BlockCategory.Obstacle;
	}

	// Use this for initialization
	void Start () {
		Start ();	
	}

	// Update is called once per frame
	void Update () {
		Update ();
	}

	#endregion

	#region Public methods



	#endregion
}

public enum ObstacleBlockType 
{
	Water = 0,
	Mud = 1,
	Beehive = 2
}