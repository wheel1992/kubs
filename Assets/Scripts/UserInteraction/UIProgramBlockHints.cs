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
            io.InteractableObjectGrabbed += new InteractableObjectEventHandler(HandleOnGrab);
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

    void HandleOnGrab(object sender, VRTK.InteractableObjectEventArgs e)
    {
        _onHover = false;
    }

    private IEnumerator CreateProgramBlockUIHint(ProgramBlockType programBlockType)
    {
        GameObject UIProgramBlockHints = new GameObject();
        switch (programBlockType)
        {
            case ProgramBlockType.Forward:
                UIProgramBlockHints = CreateMiniGameArea(2);
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            case ProgramBlockType.Jump:
                UIProgramBlockHints = CreateMiniGameArea(3);
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            case ProgramBlockType.RotateLeft:
                UIProgramBlockHints = CreateMiniGameArea(1);
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            case ProgramBlockType.RotateRight:
                UIProgramBlockHints = CreateMiniGameArea(1);
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            default:
                break;
        }
        _animating = false;
        yield return new WaitForSeconds(3);
    }

    private GameObject CreateMiniGameArea(int numTerrain)
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
        // Instatiate Character along with Parent
        //Character miniChar = new Character();
        //miniChar.tag = "UICharacter";
        //miniChar.transform.SetParent(UIProgramBlockHints.transform);
        return UIProgramBlockHints;
    }

    private void ArrangeGameArea(GameObject area)
    {
        if (area != null)
        {
            Character miniChar = new Character();
            if (GetCharacterFromGameArea(area) != null)
            {
                GetCharacterFromGameArea(area);
            }
            else
                Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCharacterFromGameArea returns null");

            if (GetCubesFromGameArea(area).Count != 0)
            {
                List<GameObject> cubeCollection = GetCubesFromGameArea(area);
                switch (cubeCollection.Count)
                {
                    case 1:
                        break;
                    case 2:

                        break;
                    case 3:
                        break;
                    default:
                        break;
                }

            }
            else
                Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCubesFromGameArea returns null");

        }

    }

    private Character GetCharacterFromGameArea(GameObject area)
    {
        Character miniChar = new Character();
        if (area != null)
        {
            foreach (Transform child in area.transform)
            {
                if(child.tag == "UICharacter")
                {
                    miniChar = child.gameObject.GetComponent<Character>();
                }
            }
        }
        return miniChar;
    }

    private List<GameObject> GetCubesFromGameArea(GameObject area)
    {
        List<GameObject> cubeCollection = new List<GameObject>();
        if(area != null)
        {
            foreach (Transform child in area.transform)
            {
                if (child.tag == "Cubes")
                {
                    cubeCollection.Add(child.gameObject);
                }
            }
        }
        return cubeCollection;
    }
}
