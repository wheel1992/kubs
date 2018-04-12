using Kubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UIProgramBlockHints : MonoBehaviour {

    private bool _onHover = false;
    private bool _animating = false;
    private GameObject currentGameObject;
    private float scaleFactor = 0.3f;
    private GameObject UIProgramBlockHintsPointer;
    private Character _character;
    [SerializeField] private GameObject CharacterPrefab;
    [SerializeField] private GameObject GrassPrefab;
    [SerializeField] private GameObject HolePrefab;

	// Use this for initialization
	void Start () {
        //_onHover = true;
		if(this.gameObject.GetComponent<VRTK_InteractableObject>() != null) {
            var io = gameObject.GetComponent<VRTK_InteractableObject>();
            io.InteractableObjectTouched += new InteractableObjectEventHandler(HandleOnTouch);
            io.InteractableObjectGrabbed += new InteractableObjectEventHandler(HandleOnGrab);
            //io.InteractableObjectUntouched += new InteractableObjectEventHandler(HandleUnTouch);
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
        } else
        {
            if (UIProgramBlockHintsPointer != null)
                GameObject.Destroy(UIProgramBlockHintsPointer);
        }
    }

    void OnEnable()
    {
        EventManager.StartListening(Constant.EVENT_NAME_CHARACTER_DID_START, SetCharacter);
    }

    void OnDisable()
    {
        EventManager.StopListening(Constant.EVENT_NAME_CHARACTER_DID_START, SetCharacter);
    }

    private void SetCharacter(object character)
    {
        _character = (Character)character;
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
        if(UIProgramBlockHintsPointer == null)
        {
            switch (programBlockType)
            {
                case ProgramBlockType.Forward:
                    UIProgramBlockHints = CreateMiniGameArea(2, "Program_Block_SnapDropZone_Clone_Forward");
                    UIProgramBlockHintsPointer = UIProgramBlockHints;
                    break;
                case ProgramBlockType.Jump:
                    UIProgramBlockHints = CreateMiniGameArea(3, "Program_Block_SnapDropZone_Clone_Jump");
                    UIProgramBlockHintsPointer = UIProgramBlockHints;
                    break;
                case ProgramBlockType.RotateLeft:
                    UIProgramBlockHints = CreateMiniGameArea(1, "Program_Block_SnapDropZone_Clone_RotateLeft");
                    UIProgramBlockHintsPointer = UIProgramBlockHints;
                    break;
                case ProgramBlockType.RotateRight:
                    UIProgramBlockHints = CreateMiniGameArea(1, "Program_Block_SnapDropZone_Clone_RotateRight");
                    UIProgramBlockHintsPointer = UIProgramBlockHints;
                    break;
                default:
                    break;
            }
        }
        //Instantiate Char
        _character = CreateChar(UIProgramBlockHints);
        //Make character move
        StartCoroutine(MoveCharacterAfterTwoFrames(programBlockType));
        _animating = false;
        //Destroy(GetCharacterFromGameArea(UIProgramBlockHints), 0);
        yield return new WaitForSeconds(3);

        _character.Reset();
        StartCoroutine(MoveCharacterAfterTwoFrames(programBlockType));
    }

    private IEnumerator MoveCharacterAfterTwoFrames(ProgramBlockType programBlockType)
    {
        yield return null;
        yield return null;

        switch (programBlockType)
        {
            case ProgramBlockType.Forward:
                _character.Forward();
                break;
            case ProgramBlockType.Jump:
                _character.Jump();
                break;
            case ProgramBlockType.RotateLeft:
                _character.RotateLeft();
                break;
            case ProgramBlockType.RotateRight:
                _character.RotateRight();
                break;
            default:
                break;
        }
    }

    private GameObject CreateMiniGameArea(int numTerrain, string programBlockTypeOnFieldString)
    {
        GameObject UIProgramBlockHints = Instantiate(new GameObject(), currentGameObject.transform, false);
        GameObject programBlockTypeOnField = GameObject.Find(programBlockTypeOnFieldString);
        UIProgramBlockHints.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        UIProgramBlockHints.transform.position = programBlockTypeOnField.transform.position + new Vector3(0, 1, 0);
        //Debug.Log("UIProgramBlockHints CreateMiniGameArea CurrentGameObject: " + currentGameObject.name);
        //Debug.Log("UIProgramBlockHints CreateMiniGameArea UIProgramBlockHints BeforeTransform Transform: " + UIProgramBlockHints.transform.localPosition);
        //Debug.Log("UIProgramBlockHints CreateMiniGameArea UIProgramBlockHints ProgramBlockTypeOnField Transform: " + programBlockTypeOnField.transform.position);
        //Debug.Log("UIProgramBlockHints CreateMiniGameArea UIProgramBlockHints Transform: " + UIProgramBlockHints.transform.localPosition);
        UIProgramBlockHints.name = "UIProgramBlockHints";
        for (int numBlock = 0; numBlock < numTerrain; numBlock++)
        {
            GameObject cube = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), UIProgramBlockHints.transform, false);
            cube.name = "MiniTerrain" + numBlock;
            //cube.transform.SetParent(UIProgramBlockHints.transform);
            //cube.transform.localPosition = new Vector3(0, ((0.5f+scaleFactor*2)/scaleFactor), 0);
            cube.tag = "Cubes";
        }
        //GameObject miniCharObject = Instantiate(CharacterPrefab, UIProgramBlockHints.transform);
        //miniCharObject.tag = "UICharacter";
        ArrangeGameArea(UIProgramBlockHints);
        var terrainReplacer = UIProgramBlockHints.AddComponent<TerrainReplacer>();
        SetNamedPrefabs(terrainReplacer);
        //UIProgramBlockHints.transform.localEulerAngles = new Vector3(0, 45f, 0);
        //Debug.Log("End");
        return UIProgramBlockHints;
    }

    private void ArrangeGameArea(GameObject area)
    {
        List<GameObject> cubeCollection;
        //Character miniChar;
        if (area != null)
        {
            //if (GetCharacterFromGameArea(area) != null)
            //{
            //    miniChar = GetCharacterFromGameArea(area);
            //}
            //else
            //    Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCharacterFromGameArea returns null");

            if (GetCubesFromGameArea(area).Count != 0)
            {
                cubeCollection = GetCubesFromGameArea(area);
                GameObject cubeOne;
                GameObject cubeTwo;
                GameObject cubeThree;
                switch (cubeCollection.Count)
                {
                    case 1:
                        cubeOne = cubeCollection[0];
                        cubeOne.transform.localPosition = new Vector3(0, 0, 0);
                        cubeOne.tag = "Grass";
                        //if (GetCharacterFromGameArea(area) != null)
                        //{
                        //    miniChar = GetCharacterFromGameArea(area);
                        //    miniChar.transform.localPosition = new Vector3(0, 1f, 0);
                        //}
                        //else
                        //    Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCharacterFromGameArea returns null");
                        break;
                    case 2:
                         cubeOne = cubeCollection[0];
                         cubeTwo = cubeCollection[1];
                         cubeOne.transform.localPosition = new Vector3(0, 0, -0.5f);
                         cubeOne.tag = "Grass";
                         cubeTwo.transform.localPosition = new Vector3(0, 0, 0.5f);
                         cubeTwo.tag = "Grass";
                        //if (GetCharacterFromGameArea(area) != null)
                        // {
                        //     miniChar = GetCharacterFromGameArea(area);
                        //     miniChar.transform.localPosition = new Vector3(0, 1f, -0.5f);
                        // }
                        // else
                        //     Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCharacterFromGameArea returns null");
                        break;
                    case 3:
                        cubeOne = cubeCollection[0];
                        cubeTwo = cubeCollection[1];
                        cubeThree = cubeCollection[2];
                        cubeOne.transform.localPosition = new Vector3(0, 0, -1f);
                        cubeOne.tag = "Grass";
                        cubeTwo.transform.localPosition = new Vector3(0, 0, 0);
                        cubeTwo.tag = "Hole";
                        cubeThree.transform.localPosition = new Vector3(0, 0, 1f);
                        cubeThree.tag = "Grass";
                        //if (GetCharacterFromGameArea(area) != null)
                        //{
                        //    miniChar = GetCharacterFromGameArea(area);
                        //    miniChar.transform.localPosition = new Vector3(0, 1f, -1f);
                        //}
                        //else
                        //    Debug.Log("UIProgramBlockHints.ArrangeGameArea: GetCharacterFromGameArea returns null");
                        //Debug.Log("UIProgramBlockHints.ArrangeGameArea: CubeCollection Count Larger Than 3");
                        break;
                    default:
                        break;
                }
                //Activate TerrainReplacer

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
                if (child.tag == "Cubes" || child.tag == "Grass" || child.tag == "Hole")
                {
                    cubeCollection.Add(child.gameObject);
                }
            }
        }
        return cubeCollection;
    }

    private TerrainReplacer SetNamedPrefabs(TerrainReplacer terrainReplacer)
    {
        terrainReplacer.prefabs = new TerrainReplacer.NamedPrefab[2];
        terrainReplacer.prefabs[0].name = "Grass";
        terrainReplacer.prefabs[0].prefab = GrassPrefab;
        terrainReplacer.prefabs[1].name = "Hole";
        terrainReplacer.prefabs[1].prefab = HolePrefab;
        return terrainReplacer;
    }

    private Character CreateChar(GameObject area)
    {
        Character miniChar = Instantiate(CharacterPrefab, area.transform).GetComponent<Character>();
        miniChar.name = "MiniCharacter";
        miniChar.tag = "UICharacter";
        miniChar.transform.localPosition = GetCubesFromGameArea(area)[0].transform.localPosition + new Vector3(0, 1f, 0);
        return miniChar;
    }

    //private void AssignBlockType(string blockTypeInString, GameObject cube)
    //{
    //    if(blockTypeInString.Equals("Grass") && Grass != null)
    //    {
    //        cube.tag = "Grass";
    //        cube.GetComponent<MeshRenderer>().material = Grass;
    //    } else if(blockTypeInString.Equals("Hole") && Hole != null)
    //    {
    //        cube.tag = "Hole";
    //        cube.GetComponent<MeshRenderer>().material = Hole;
    //    } else
    //    {
    //        if (Grass == null || Hole == null)
    //            Debug.Log("Material Prefab is empty");
    //        else
    //            Debug.Log("Block Type In String is wrong");
    //    }
    //}
}
