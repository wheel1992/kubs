using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramBlock : MonoBehaviour, Block {

	#region Lifecycle

	void Awake() 
	{
		Category = BlockCategory.Program;
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

public enum ProgramBlockType 
{
	Forward = 0,
	RotateLeft = 1,
	RotateRight = 2,
	Jump = 3,
	ForLoop = 4,
	End = 5
}	
