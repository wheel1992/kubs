using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	public BlockCategory Category { get; set; }

	public int? Value { get; set; }

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}

public enum BlockCategory
{
	Obstacle = 0,
	Program = 1,
	Terrain = 2
}
