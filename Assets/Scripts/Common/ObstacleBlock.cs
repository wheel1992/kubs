using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock : Block  {

	public ObstacleBlockType Type { get; set; }

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