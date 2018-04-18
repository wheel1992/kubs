using Kubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UIProgramBlockHints : MonoBehaviour
{
    public delegate void ProgramBlockGrabEventHandler(ProgramBlockType programBlockType);
    public event ProgramBlockGrabEventHandler ProgramBlockGrab;

    private Transform transformHint;
    private bool _onHover = false;
    private bool _animating = false;
    private bool _changeJump = false;
    private GameObject currentGameObject;
    private float scaleFactor = 0.3f;
    private GameObject UIProgramBlockHintsPointer;
    private GameObject rightControllerTooltip;
    private GameObject gripTooltip;
    private TutorialManager _tutorialManager
    {
        get
        {
            if (__tutorialManager == null)
            {
                var go = GameObject.Find("TutorialManager");
                if (go != null)
                {
                    __tutorialManager = go.GetComponent<TutorialManager>();
                }
            }

            return __tutorialManager;
        }
    }
    private TutorialManager __tutorialManager;
    private Character _character;
    private IEnumerator _moveCoroutine;

    [SerializeField] private GameObject CharacterPrefab;
    [SerializeField] private GameObject GrassPrefab;
    [SerializeField] private GameObject HolePrefab;

    // Use this for initialization
    void Start()
    {
        // _onHover = true;
        if (this.gameObject.GetComponent<VRTK_InteractableObject>() != null)
        {
            var io = gameObject.GetComponent<VRTK_InteractableObject>();
            io.InteractableObjectTouched += new InteractableObjectEventHandler(HandleOnTouch);
            io.InteractableObjectGrabbed += new InteractableObjectEventHandler(HandleOnGrab);
            io.InteractableObjectUntouched += new InteractableObjectEventHandler(HandleUnTouch);
            io.InteractableObjectUngrabbed += new InteractableObjectEventHandler(HandleUnGrab);
            io.InteractableObjectSnappedToDropZone += new InteractableObjectEventHandler(HandleOnSnap);
            //if(gameObject.CompareTag("Block_Program"))
            //{
            currentGameObject = this.gameObject;
            //Debug.Log("UIProgramBlockHints Start Function: " + currentGameObject.name );
        }
        rightControllerTooltip = GetRightVRTKControllerTooltips();
        gripTooltip = GetRightVRTKGripTooltips();
    }

    // Update is called once per frame
    void Update()
    {
        if (_onHover)
        {
            if (currentGameObject.CompareTag("Block_Program"))
            {
                ProgramBlockType programBlockType = currentGameObject.GetComponent<ProgramBlock>().Type;
                //Debug.Log("UIProgramBlockHints Update Function: " + currentGameObject.name );
                if (UIProgramBlockHintsPointer != null)
                {
                    UIProgramBlockHintsPointer.SetActive(true);
                    if (_character == null) CreateChar(UIProgramBlockHintsPointer);
                    else if (!_animating)
                    {
                        //_character = this.transform.Find("MiniCharacter").gameObject.GetComponent<Character>();
                        _animating = true;
                        //_character.Reset();
                        _character.gameObject.SetActive(true);

                        _moveCoroutine = MoveCharacterAfterOneSecond(programBlockType);
                        StartCoroutine(_moveCoroutine);
                    }
                }
                else UIProgramBlockHintsPointer = CreateProgramBlockUIHint(programBlockType);
            }
        }
        else
        {
            transformHint = this.transform.Find("UIProgramBlockHints");
            if (transformHint != null)
            {
                if (_character != null)
                {
                    _character.Reset();
                }

                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                }

                UIProgramBlockHintsPointer = transformHint.gameObject;
                UIProgramBlockHintsPointer.SetActive(false);
            }
        }
    }

    // void OnEnable()
    // {
    //     EventManager.StartListening(Constant.EVENT_NAME_CHARACTER_DID_START, SetCharacter);
    // }

    // void OnDisable()
    // {
    //     EventManager.StopListening(Constant.EVENT_NAME_CHARACTER_DID_START, SetCharacter);
    // }

    // private void SetCharacter(object character)
    // {
    //     _character = (Character)character;
    // }

    void HandleOnTouch(object sender, VRTK.InteractableObjectEventArgs e)
    {
        if (GetProgramBlock().IsInSnapDropZoneClone())
        {
            _onHover = true;
        }

        // if (GetProgramBlock().Type == ProgramBlockType.Forward && _tutorialManager.onShowTutorialArrow)
        // {
        //     // if (GameObject.Find("UIHintsArrowPointer") != null)
        //     // {
        //     //     Debug.Log(_tutorialManager.arrowPointer.name);
        //     //     _tutorialManager.arrowPointer.SetActive(false);
        //     // }
        // }

        // TEST
        Debug.Log("HandleOnTouch");

        //if (_tutorialManager == null) { return; }
        if (_tutorialManager != null && _tutorialManager.onShowTutorialArrow)
        {
            var activeStage = _tutorialManager.GetCurrentActiveStage();
            // Either current tutorial stage = 1 & programBlock is Forward
            // Or current tutorial stage = 4 & programBlock is ForLoopStart
            if ((activeStage == 1 && GetProgramBlock().Type == ProgramBlockType.Forward) ||
                (activeStage == 4 && GetProgramBlock().Type == ProgramBlockType.ForLoopStart))
            {
                _tutorialManager.HideArrowPointer();
                ShowGripTooltip();
            }
        }
    }

    void HandleUnTouch(object sender, VRTK.InteractableObjectEventArgs e)
    {
        _onHover = false;


        // if (gameObject.GetComponent<ProgramBlock>().Type == ProgramBlockType.Forward && _tutorialManager.onShowTutorialArrow)
        // {
        //     if (_tutorialManager.arrowPointer != null)
        //     {
        //         _tutorialManager.arrowPointer.SetActive(true);
        //     }
        // }

        // TEST
        Debug.Log("HandleUnTouch");
        if (_tutorialManager == null) { return; }
        if (_tutorialManager.onShowTutorialArrow)
        {
            var activeStage = _tutorialManager.GetCurrentActiveStage();
            // Either current tutorial stage = 1 & programBlock is Forward
            // Or current tutorial stage = 4 & programBlock is ForLoopStart
            if ((activeStage == 1 && GetProgramBlock().Type == ProgramBlockType.Forward) ||
                (activeStage == 4 && GetProgramBlock().Type == ProgramBlockType.ForLoopStart))
            {
                _tutorialManager.ShowArrowPointer();
                ShowGripTooltip();
            }
        }
        // if (__tutorialManager != null)
        // {
        //     if (GetProgramBlock().Type == ProgramBlockType.Forward && _tutorialManager.onShowTutorialArrow)
        //     {
        //         _tutorialmanager.ShowArrowPointer();
        //         HideGripTooltip();
        //     }
        // }
    }

    void HandleOnGrab(object sender, VRTK.InteractableObjectEventArgs e)
    {
        _onHover = false;

        if (_tutorialManager == null) { return; }
        if (_tutorialManager.onShowTutorialArrow)
        {
            var activeStage = _tutorialManager.GetCurrentActiveStage();
            if ((activeStage == 1 && GetProgramBlock().Type == ProgramBlockType.Forward) ||
                (activeStage == 4 && GetProgramBlock().Type == ProgramBlockType.ForLoopStart)) {
                _tutorialManager.SetArrowPointerPositionToZone();
                HideGripTooltip();
            }
        }
        // if (GetProgramBlock().Type == ProgramBlockType.Forward && _tutorialManager.onShowTutorialArrow)
        // {
        //     Debug.Log("HandleOnGrab");
        //     _tutorialManager.SetArrowPointerPositionToZone();
        //     HideGripTooltip();
        // }
    }

    void HandleUnGrab(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("HandleUnGrab");
        var programBlock = GetProgramBlock();
        if (programBlock == null) { return; }

        if (_tutorialManager == null) { return; }
        if (_tutorialManager.onShowTutorialArrow)
        {
            var activeStage = _tutorialManager.GetCurrentActiveStage();
            if (activeStage == 1 && programBlock.Type == ProgramBlockType.Forward)
            {
                _tutorialManager.SetArrowPointerPositionToSnapCloneForward();
            }
            else if (activeStage == 4 && programBlock.Type == ProgramBlockType.ForLoopStart)
            {
                _tutorialManager.SetArrowPointerPositionToSnapCloneForStartEnd();
            }
            HideGripTooltip();
        }
        // if (GetProgramBlock().Type == ProgramBlockType.Forward && _tutorialManager.onShowTutorialArrow)
        // {
        //     // _tutorialManager.ShowArrowPointer();
        //     _tutorialManager.SetArrowPointerPositionToSnapCloneForward();
        //     HideGripTooltip();
        // }
    }

    void HandleOnSnap(object sender, InteractableObjectEventArgs e)
    {
        Debug.Log("HandleOnSnap");
        var programBlock = gameObject.GetComponent<ProgramBlock>();
        if (programBlock == null) { return; }
        if (programBlock.IsInSnapDropZoneClone()) { return; }

        if (_tutorialManager == null) { return; }
        if (_tutorialManager.onShowTutorialArrow)
        {
            var activeStage = _tutorialManager.GetCurrentActiveStage();
            if (activeStage == 1 && programBlock.Type == ProgramBlockType.Forward)
            {
                // Move arrow pointer to ButtonStart
                _tutorialManager.SetArrowPointerPositionToButtonStart();
            }
            else if (activeStage == 4 && programBlock.Type == ProgramBlockType.ForLoopStart)
            {
                // Remove arrow pointer
                _tutorialManager.DestroyArrowPointer();
            }
            HideGripTooltip();
        }
        // if (_tutorialManager.onShowTutorialArrow &&  // Currently arrow is showing
        //     programBlock.Type == ProgramBlockType.Forward && // Snapped block is Forward
        //     !programBlock.IsInSnapDropZoneClone()) // Snap not in SnapDropZoneClone
        // {
        //     //_tutorialManager.onShowTutorialArrow = false;
        //     // _tutorialManager.DestroyArrowPointer();
        //     _tutorialManager.SetArrowPointerPositionToButtonStart();
        //     HideGripTooltip();
        // }
    }
    private void HideGripTooltip()
    {
        if (gripTooltip == null)
        {
            gripTooltip = GetRightVRTKGripTooltips();
        }
        rightControllerTooltip.SetActive(false);
        gripTooltip.SetActive(false);
    }
    private void ShowGripTooltip()
    {
        if (gripTooltip == null)
        {
            gripTooltip = GetRightVRTKGripTooltips();
        }
        rightControllerTooltip.SetActive(true);
        gripTooltip.SetActive(true);
    }
    private GameObject CreateProgramBlockUIHint(ProgramBlockType programBlockType)
    {
        GameObject UIProgramBlockHints = null;
        if (UIProgramBlockHintsPointer == null)
        {
            switch (programBlockType)
            {
                case ProgramBlockType.Forward:
                    UIProgramBlockHints = CreateMiniGameArea(2, "Program_Block_SnapDropZone_Clone_Forward");
                    break;
                case ProgramBlockType.Jump:
                    UIProgramBlockHints = CreateMiniGameArea(3, "Program_Block_SnapDropZone_Clone_Jump");
                    break;
                case ProgramBlockType.RotateLeft:
                    UIProgramBlockHints = CreateMiniGameArea(1, "Program_Block_SnapDropZone_Clone_RotateLeft");
                    break;
                case ProgramBlockType.RotateRight:
                    UIProgramBlockHints = CreateMiniGameArea(1, "Program_Block_SnapDropZone_Clone_RotateRight");
                    break;
                default:
                    break;
            }
        }
        //Instantiate Char
        // _character = CreateChar(UIProgramBlockHints);
        //Make character move
        // StartCoroutine(MoveCharacterAfterOneSecond(programBlockType));

        //Destroy(GetCharacterFromGameArea(UIProgramBlockHints), 0);
        // yield return new WaitForSeconds(3);

        // _character.Reset();
        // StartCoroutine(MoveCharacterAfterOneSecond(programBlockType));
        UIProgramBlockHints.transform.localEulerAngles = new Vector3(0, 80, 0); // new Vector3(0, 45, 0);
        return UIProgramBlockHints;
        // _animating = false;
    }

    private IEnumerator MoveCharacterAfterOneSecond(ProgramBlockType programBlockType)
    {
        yield return new WaitForSecondsRealtime(1);
        switch (programBlockType)
        {
            case ProgramBlockType.Forward:
                _character.Forward();
                break;
            case ProgramBlockType.Jump:
                _character.Jump();
                _changeJump = !_changeJump;
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
        //Start here
        //_character.gameObject.SetActive(false);
        //_character.Reset();
        //yield return new WaitForSeconds(3);
        //_animating = false;
        //Debug.Log("Wait For 3 Seconds");
        yield return null;
    }

    private GameObject CreateMiniGameArea(int numTerrain, string programBlockTypeOnFieldString)
    {
        GameObject UIProgramBlockHints = new GameObject();
        UIProgramBlockHints.transform.SetParent(currentGameObject.transform);
        UIProgramBlockHints.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        UIProgramBlockHints.transform.position = currentGameObject.transform.position + new Vector3(0, 1, 0);
        UIProgramBlockHints.name = "UIProgramBlockHints";
        UIProgramBlockHintsPointer = UIProgramBlockHints;
        for (int numBlock = 0; numBlock < numTerrain; numBlock++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "MiniTerrain" + numBlock;
            cube.transform.SetParent(UIProgramBlockHints.transform);
            cube.transform.localPosition = new Vector3(0, ((0.5f + scaleFactor * 2) / scaleFactor), 0);
            cube.tag = "Cubes";
        }
        //GameObject miniCharObject = Instantiate(CharacterPrefab, UIProgramBlockHints.transform);
        //miniCharObject.tag = "UICharacter";
        ArrangeGameArea(UIProgramBlockHintsPointer);
        var terrainReplacer = UIProgramBlockHintsPointer.AddComponent<TerrainReplacer>();
        SetNamedPrefabs(terrainReplacer);
        //UIProgramBlockHints.transform.localEulerAngles = new Vector3(0, 45f, 0);
        //Debug.Log("End");
        return UIProgramBlockHintsPointer;
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
                        cubeOne.tag = "Grass";
                        // cubeTwo.tag = "Hole";
                        cubeThree.tag = "Grass";

                        if (_changeJump)
                        {
                            cubeTwo.tag = "Grass";

                            cubeOne.transform.localPosition = new Vector3(0, 0, -0.5f);
                            cubeTwo.transform.localPosition = new Vector3(0, 0, 0.5f);
                            cubeThree.transform.localPosition = new Vector3(0, 1f, 0.5f);

                            var characterPosition = _character.transform.localPosition;
                            characterPosition.z = -0.5f;
                            _character.transform.localPosition = characterPosition;
                        }
                        else
                        {
                            cubeTwo.tag = "Hole";

                            cubeOne.transform.localPosition = new Vector3(0, 0, -1f);
                            cubeTwo.transform.localPosition = new Vector3(0, 0, 0);
                            cubeThree.transform.localPosition = new Vector3(0, 0, 1f);
                        }
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
                if (child.tag == "UICharacter")
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
        if (area != null)
        {
            foreach (Transform child in area.transform)
            {
                if (child.tag == "Cubes" || child.tag == "Grass" || child.tag == "Hole" || child.name.Contains("Grass") || child.name.Contains("Hole"))
                {
                    cubeCollection.Add(child.gameObject);
                }
            }
        }
        return cubeCollection;
    }
    private ProgramBlock GetProgramBlock()
    {
        return gameObject.GetComponent<ProgramBlock>();
    }
    private GameObject GetRightVRTKGripTooltips()
    {
        var rightCtrlTooltip = GetRightVRTKControllerTooltips();
        if (rightCtrlTooltip == null) { return null; }
        var tooltip = rightCtrlTooltip.transform.Find("GripTooltip");
        if (tooltip == null) { return null; }
        return tooltip.gameObject;
    }
    private GameObject GetRightVRTKControllerTooltips() {
        var rightCtrl = GameObject.Find("RightController");
        if (rightCtrl == null) { return null; }
        var tooltip = rightCtrl.transform.Find("ControllerTooltips");
        return tooltip.gameObject;
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
        _character = Instantiate(CharacterPrefab, area.transform).GetComponent<Character>();
        _character.gameObject.SetActive(false);
        _character.name = "MiniCharacter";
        _character.tag = "UICharacter";
        _character.transform.localPosition = GetCubesFromGameArea(area)[0].transform.localPosition + new Vector3(0, 1, 0);
        _character.gameObject.SetActive(true);
        _character.OnReset += new Character.CharacterEventHandler(HandleCharReset);
        _character.showPopup = false;
        //new WaitForSecondsRealtime(2);
        return _character;
    }

    private void HandleCharReset()
    {
        //Debug.Log("HandleCharReset()");
        // new WaitForSecondsRealtime(2f);
        // Debug.Log("After 2 seconds");

        if (gameObject.name.Contains("Jump") && transform.Find("UIProgramBlockHints") != null)
        {
            ArrangeGameArea(transform.Find("UIProgramBlockHints").gameObject);
        }

        _animating = false;
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
