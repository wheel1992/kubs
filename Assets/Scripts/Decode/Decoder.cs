using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoder : MonoBehaviour
{
    private Character _character;

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public bool Decode(List<ProgramBlock> blockchain)
	{
        foreach (var block in blockchain)
        {
            if (block.Type == ProgramBlockType.Forward)
            {
                _character.Forward();
            }
        }

		return false;
	}
}
