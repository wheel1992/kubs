using System.Collections;
using UnityEngine;
using VRTK;

public class BlockChainController : MonoBehaviour
{
    //Attributes for movement
    private GameObject[] blockChainWaypoints;
    private int startTargetWP = 1;
    private int currentWP;
    private float rotationSpeed = 3f;
    private float moveSpeed = 3f;
    private float accuracyWP = 1f;
    private bool moveValid = false;
    private GameObject blockChainPlate;
    private BoxCollider blockChainPlateBoxCollider;
    private Vector3 direction;

    //Attributes for expansion & depression
    private bool expand = false;
    private float _currentScale = InitScale;
    private const float TargetBigScale = 2f;
    private const float TargetSmallScale = 0.1f;
    private const float InitScale = 1f;
    private const int FramesCount = 100;
    private const float AnimationTimeSeconds = 2f;
    private float _deltaTime = AnimationTimeSeconds / FramesCount;
    private float _dx = (TargetBigScale - InitScale) / FramesCount;
    private float _dxDepress = (InitScale - TargetSmallScale) / FramesCount;
    //private bool setActive = false;

    private GameObject Program_Block_Snap_Zone;

    [SerializeField] private GameObject testBlockPrefab;


    // Use this for initialization
    void Start()
    {
        currentWP = startTargetWP;
        blockChainWaypoints = new GameObject[19];
        blockChainWaypoints[0] = GameObject.Find("BlockChainMovementWPS0");
        blockChainWaypoints[1] = GameObject.Find("BlockChainMovementWPS1");
        blockChainWaypoints[2] = GameObject.Find("BlockChainMovementWPS2");
        blockChainWaypoints[3] = GameObject.Find("BlockChainMovementWPS3");
        blockChainWaypoints[4] = GameObject.Find("BlockChainMovementWPS4");
        blockChainWaypoints[5] = GameObject.Find("BlockChainMovementWPS5");
        blockChainWaypoints[6] = GameObject.Find("BlockChainMovementWPS6");
        blockChainWaypoints[7] = GameObject.Find("BlockChainMovementWPS7");
        blockChainWaypoints[8] = GameObject.Find("BlockChainMovementWPS8");
        blockChainWaypoints[9] = GameObject.Find("BlockChainMovementWPS9");
        blockChainWaypoints[10] = GameObject.Find("BlockChainMovementWPS10");
        blockChainWaypoints[11] = GameObject.Find("BlockChainMovementWPS11");
        blockChainWaypoints[12] = GameObject.Find("BlockChainMovementWPS12");
        blockChainWaypoints[13] = GameObject.Find("BlockChainMovementWPS13");
        blockChainWaypoints[14] = GameObject.Find("BlockChainMovementWPS14");
        blockChainWaypoints[15] = GameObject.Find("BlockChainMovementWPS15");
        blockChainWaypoints[16] = GameObject.Find("BlockChainMovementWPS16");
        blockChainWaypoints[17] = GameObject.Find("BlockChainMovementWPS17");
        blockChainWaypoints[18] = GameObject.Find("BlockChainMovementWPS18");
        blockChainPlate = GameObject.Find("BlockChain");
        blockChainPlateBoxCollider = GameObject.Find("BlockChainPlate").GetComponent<BoxCollider>();
        //InitBlockChainPlateBoxCollider();
        //Debug.Log("BlockChain Started");
        Program_Block_Snap_Zone = GameObject.Find("Program_Block_SnapDropZone");
        if (Program_Block_Snap_Zone != null)
        {
            var block = Instantiate(testBlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Program_Block_Snap_Zone.GetComponent<VRTK_SnapDropZone>().ForceSnap(block);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("BlockChain Update: " + moveValid + " " + currentWP);
        if (moveValid)
        {
            Debug.Log("BlockChain Move Valid Update");
            if (Vector3.Distance(blockChainWaypoints[currentWP].transform.position, transform.position) < accuracyWP)
            {
                //Handle reassignment of target way point when object reaches current way point

                if (currentWP < blockChainWaypoints.Length - 1)
                {
                    currentWP++;
                } else if(currentWP == blockChainWaypoints.Length - 1)
                {
                    currentWP = 0;
                }

                if(currentWP == 10 || currentWP == 1)
                {
                    //moveValid = false;
                    if (currentWP == 1)
                        blockChainPlateBoxCollider.enabled = true;
                    else
                    {
                        GameObject.Find("BlockChain").SetActive(false);
                    }
                    return;
                }

            }

            //rotate and move towards waypoint
            direction = blockChainWaypoints[currentWP].transform.position - transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            this.transform.Translate(0, 0, Time.deltaTime * moveSpeed);
        }
        else
        {
            // ...
        }

    }

    void InitBlockChainPlateBoxCollider()
    {
        blockChainPlateBoxCollider = gameObject.transform.Find("BlockChainPlate").GetComponent<BoxCollider>();
    }

    public bool MoveBlockChain()
    {
        if (expand)
            StartCoroutine(Expand());
        else
            StartCoroutine(Depress());

        if (blockChainPlateBoxCollider == null)
        {
            InitBlockChainPlateBoxCollider();
        }
        blockChainPlateBoxCollider.enabled = false;
        moveValid = true;
        return moveValid;
    }

    private IEnumerator Expand()
    {

        var incrementor = 0f;
        while (this.transform.localScale != (Vector3.one * TargetBigScale))
        {
            incrementor += 0.001f;
            Vector3 currentScale = Vector3.Lerp(Vector3.one * InitScale, Vector3.one * TargetBigScale, incrementor);
            transform.localScale = currentScale;
            yield return null;
        }

        yield break;

        /*
        while (true)
        {
            while (_upScale)
            {
                _currentScale += _dx;
                if (_currentScale > TargetBigScale)
                {
                    _upScale = false;
                    _currentScale = TargetBigScale;
                }
                blockChainPlate.transform.localScale = Vector3.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }

            while (!_upScale)
            {
                _currentScale -= _dx;
                if (_currentScale < InitScale)
                {
                    _upScale = true;
                    _currentScale = InitScale;
                }
                blockChainPlate.transform.localScale = Vector3.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }
        }*/
    }

    private IEnumerator Depress()
    {
        var incrementor = 0f;
        while (this.transform.localScale != (Vector3.one * TargetSmallScale))
        {
            incrementor += 0.001f;
            Vector3 currentScale = Vector3.Lerp(Vector3.one * InitScale, Vector3.one * TargetSmallScale, incrementor);
            transform.localScale = currentScale;
            yield return null;
        }

        yield break;

        /*
        var incrementor = 0f;
        while (!expand)
        {
            incrementor += Time.deltaTime;
            _currentScale -=incrementor;
            if (_currentScale < TargetBigScale)
            {
                expand = true;
                _currentScale = TargetSmallScale;
            }
            blockChainPlate.transform.localScale = Vector3.one * _currentScale;
            yield return new WaitForSeconds(_deltaTime);
            */
    }
}