using Kubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UIProgramBlockHints : MonoBehaviour {

    private bool _onHover = false;
    private bool _animating = false;
    private GameObject currentGameObject;

	// Use this for initialization
	void Start () {
		if(this.gameObject.GetComponent<VRTK_InteractableObject>() != null) {
            var io = gameObject.GetComponent<VRTK_InteractableObject>();
            io.InteractableObjectTouched += new InteractableObjectEventHandler(HandleOnTouch);
            io.InteractableObjectUntouched += new InteractableObjectEventHandler(HandleUnTouch);
            //if(gameObject.CompareTag("Block_Program"))
            //{
            currentGameObject = this.gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(_onHover)
        {
            if(currentGameObject.CompareTag("Block_Program") && !_animating)
            {
                ProgramBlockType programBlockType = currentGameObject.GetComponent<ProgramBlock>().Type;
                StartCoroutine(CreateProgramBlockUIHint(programBlockType));
                _animating = true;
            }
        }
		
	}

    void HandleOnTouch(object sender, VRTK.InteractableObjectEventArgs e)
    {
        _onHover = true;
    }

    void HandleUnTouch(object sender, VRTK.InteractableObjectEventArgs e)
    {
        _onHover = false;
    }

    private IEnumerator CreateProgramBlockUIHint(ProgramBlockType programBlockType)
    {
        Character miniChar;
        switch (programBlockType)
        {
            case ProgramBlockType.Forward:
                miniChar = CreateMiniGameArea(2);
                // Make character move
                break;
            case ProgramBlockType.Jump:
                miniChar = CreateMiniGameArea(3);
                // Make character move
                break;
            case ProgramBlockType.RotateLeft:
                miniChar = CreateMiniGameArea(1);
                // Make character move
                break;
            case ProgramBlockType.RotateRight:
                miniChar = CreateMiniGameArea(1);
                // Make character move
                break;
            default:
                break;
        }
        _animating = false;
        yield return new WaitForSeconds(3);
    }

    private 

     GameObject CreateMiniGameArea(int numTerrain)
    {
        GameObject UIProgramBlockHints = new GameObject();
        UIProgramBlockHints.transform.SetParent(this.gameObject.transform);
        UIProgramBlockHints.transform.localPosition = new Vector3(0, 1, 0);
        UIProgramBlockHints.name = "UIProgramBlockHints";
        for (int numBlock = 0; numBlock < numTerrain; numBlock++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "MiniTerrain" + numBlock;
            cube.transform.SetParent(UIProgramBlockHints.transform);
            cube.tag = "Cubes";
        }
        Character miniChar = new Character();
        miniChar.transform.SetParent(UIProgramBlockHints.transform);
        return UIProgramBlockHints;
    }
}
