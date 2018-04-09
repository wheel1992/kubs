using Kubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UIProgramBlockHints : MonoBehaviour {

    private bool _onHover = false;
    private bool _animating = false;
    private GameObject currentGameObject;
    private float scaleFactor = 0.2f;
    //[SerializeField] private GameObject programBlockTypeOnField;

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
            //Debug.Log("UIProgramBlockHints Start Function: " + currentGameObject.name );
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(_onHover)
        {
            if(currentGameObject.CompareTag("Block_Program") && !_animating)
            {
                ProgramBlockType programBlockType = currentGameObject.GetComponent<ProgramBlock>().Type;
                //Debug.Log("UIProgramBlockHints Update Function: " + currentGameObject.name );
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
                UIProgramBlockHints = CreateMiniGameArea(2, "Program_Block_SnapDropZone_Clone_Forward");
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            case ProgramBlockType.Jump:
                UIProgramBlockHints = CreateMiniGameArea(3, "Program_Block_SnapDropZone_Clone_Jump");
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            case ProgramBlockType.RotateLeft:
                UIProgramBlockHints = CreateMiniGameArea(1, "Program_Block_SnapDropZone_Clone_RotateLeft");
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            case ProgramBlockType.RotateRight:
                UIProgramBlockHints = CreateMiniGameArea(1, "Program_Block_SnapDropZone_Clone_RotateRight");
                //ArrangeGameArea(UIProgramBlockHints);
                // Make character move
                break;
            default:
                break;
        }
        _animating = false;
        yield return new WaitForSeconds(3);
    }

    private GameObject CreateMiniGameArea(int numTerrain, string programBlockTypeOnFieldString)
    {
        GameObject UIProgramBlockHints = new GameObject();
        //Debug.Log("UIProgramBlockHints CreateMiniGameArea CurrentGameObject: " + currentGameObject.name);
        UIProgramBlockHints.transform.SetParent(currentGameObject.transform);
        Debug.Log("UIProgramBlockHints CreateMiniGameArea UIProgramBlockHints BeforeTransform Transform: " + UIProgramBlockHints.transform.localPosition);
        GameObject programBlockTypeOnField = GameObject.Find(programBlockTypeOnFieldString);
        Debug.Log("UIProgramBlockHints CreateMiniGameArea UIProgramBlockHints ProgramBlockTypeOnField Transform: " + programBlockTypeOnField.transform.position);
        Debug.Log("UIProgramBlockHints CreateMiniGameArea UIProgramBlockHints Transform: " + UIProgramBlockHints.transform.localPosition);
        UIProgramBlockHints.name = "UIProgramBlockHints";
        for (int numBlock = 0; numBlock < numTerrain; numBlock++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "MiniTerrain" + numBlock;
            cube.transform.SetParent(UIProgramBlockHints.transform);
            cube.transform.localPosition = new Vector3(0, ((0.5f+scaleFactor*2)/scaleFactor), 0);
            cube.tag = "Cubes";
        }
        // Instatiate Character along with Parent
        //Character miniChar = new Character();
        //miniChar.tag = "UICharacter";
        //miniChar.transform.SetParent(UIProgramBlockHints.transform);
        UIProgramBlockHints.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        UIProgramBlockHints.transform.position = programBlockTypeOnField.transform.position;
        return UIProgramBlockHints;
    }

    private void ArrangeGameArea(GameObject area)
    {
        List<GameObject> cubeCollection;
        Character miniChar;
        if (area != null)
        {
            if (GetCharacterFromGameArea(area) != null)
            {
                miniChar = GetCharacterFromGameArea(area);
            }
            else
                Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCharacterFromGameArea returns null");

            if (GetCubesFromGameArea(area).Count != 0)
            {
                cubeCollection = GetCubesFromGameArea(area);
                switch (cubeCollection.Count)
                {
                    case 1:
                        break;
                    case 2:
                        if (cubeCollection.Count == 2)
                        {
                            GameObject cubeOne = cubeCollection[0];
                            GameObject cubeTwo = cubeCollection[1];
                            cubeOne.transform.localPosition -= new Vector3(0, 0, scaleFactor/2);
                            cubeTwo.transform.localPosition += new Vector3(0, 0, scaleFactor / 2);
                        }
                        else
                            Debug.Log("UIProgramBlockHints.ArrangeGameArea: CubeCollection Count Larger Than 2");
                        break;
                    case 3:
                        if (cubeCollection.Count == 3)
                        {

                        }
                        else
                            Debug.Log("UIProgramBlockHints.ArrangeGameArea: CubeCollection Count Larger Than 3");
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
