using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class ZoneMovementController : MonoBehaviour
    {
        // Events
        public event UnityAction OnCompleted;

        //Attributes for movement
        private GameObject[] zonesWaypoints;
        private int startTargetWP = 1;
        private int currentWP;
        private float rotationSpeed = 50f;
        private float moveSpeed = 12f;
        private float accuracyWP = 0.1f;
        private bool moveValid;
        private GameObject zones;
        private BoxCollider blockChainPlateBoxCollider;
        private Vector3 direction;
        public bool forward { get; set; }

        //Attributes for expansion & depression
        private float _currentScale = InitScale;
        private const float TargetBigScale = 1f;
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
            moveValid = false;
            forward = true;
            currentWP = startTargetWP;
            zonesWaypoints = new GameObject[10];
            zonesWaypoints[0] = GameObject.Find("BlockChainMovementWPS0");
            zonesWaypoints[1] = GameObject.Find("BlockChainMovementWPS1");
            zonesWaypoints[2] = GameObject.Find("BlockChainMovementWPS2");
            zonesWaypoints[3] = GameObject.Find("BlockChainMovementWPS3");
            zonesWaypoints[4] = GameObject.Find("BlockChainMovementWPS4");
            zonesWaypoints[5] = GameObject.Find("BlockChainMovementWPS5");
            zonesWaypoints[6] = GameObject.Find("BlockChainMovementWPS6");
            zonesWaypoints[7] = GameObject.Find("BlockChainMovementWPS7");
            zonesWaypoints[8] = GameObject.Find("BlockChainMovementWPS8");
            zonesWaypoints[9] = GameObject.Find("BlockChainMovementWPS9");
            //zonesWaypoints[10] = GameObject.Find("BlockChainMovementWPS10");
            //zonesWaypoints[11] = GameObject.Find("BlockChainMovementWPS11");
            //zonesWaypoints[12] = GameObject.Find("BlockChainMovementWPS12");
            //zonesWaypoints[13] = GameObject.Find("BlockChainMovementWPS13");
            //zonesWaypoints[14] = GameObject.Find("BlockChainMovementWPS14");
            //zonesWaypoints[15] = GameObject.Find("BlockChainMovementWPS15");
            //zonesWaypoints[16] = GameObject.Find("BlockChainMovementWPS16");
            //zonesWaypoints[17] = GameObject.Find("BlockChainMovementWPS17");
            //zonesWaypoints[18] = GameObject.Find("BlockChainMovementWPS18");
            zones = GameObject.Find("Zones");
            //blockChainPlateBoxCollider = GameObject.Find("BlockChainPlate").GetComponent<BoxCollider>();
            //InitBlockChainPlateBoxCollider();
            //Debug.Log("BlockChain Started");
            //Program_Block_Snap_Zone = GameObject.Find("Program_Block_SnapDropZone");
            //if (Program_Block_Snap_Zone != null)
            //{
            //    var block = Instantiate(testBlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            //    Program_Block_Snap_Zone.GetComponent<VRTK_SnapDropZone>().ForceSnap(block);
            //}

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("BlockChain Update: " + moveValid + " " + currentWP);
            if (moveValid)
            {
                //Debug.Log("BlockChain Move Valid Update");
                //Debug.Log("Debug Update Distance " + Vector3.Distance(zonesWaypoints[currentWP].transform.position, transform.position));
                if (Vector3.Distance(zonesWaypoints[currentWP].transform.position, transform.position) < accuracyWP)
                {

                    if (forward)
                    {

                        //Handle reassignment of target way point when object reaches current way point

                        if (currentWP < zonesWaypoints.Length - 1)
                        {
                            currentWP++;
                            //Debug.Log("Debug Update " + currentWP + " WP");
                        }
                        else if (currentWP == zonesWaypoints.Length - 1)
                        {
                            moveValid = false;
                            forward = false;
                            //zones.SetActive(false);

                            OnCompleted();
                            return;
                        }

                    }
                    else
                    {

                        this.gameObject.transform.localPosition = new Vector3(7.75f, 0, -5.49f);
                        this.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                        SetProgramBlockTrigger(false);

                        moveValid = false;
                        currentWP = 1;
                        forward = true;
                        return;
                    }

                }

                //rotate and move towards waypoint
                direction = zonesWaypoints[currentWP].transform.position - transform.position;
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
            Debug.Log("MoveBlockChain");
            //Debug.Log(forward);
            if (!forward)
            {
                StartCoroutine(Expand());
            }
            else
            {
                SetProgramBlockTrigger(true);
                StartCoroutine(Depress());
            }


            //if (blockChainPlateBoxCollider == null)
            //{
            //    InitBlockChainPlateBoxCollider();
            //}
            //blockChainPlateBoxCollider.enabled = false;
            moveValid = true;
            return moveValid;
        }

        private IEnumerator Expand()
        {

            var incrementor = 0f;
            while (this.transform.localScale != (Vector3.one * TargetBigScale))
            {
                incrementor += 0.01f;
                Vector3 currentScale = Vector3.Lerp(Vector3.one * 0, Vector3.one * TargetBigScale, incrementor);
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
            var scalingFactor = 5; // Bigger for slower

            while (this.transform.localScale != (Vector3.one * TargetSmallScale))
            {
                incrementor += Time.deltaTime / scalingFactor;
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
        private void SetProgramBlockTrigger(bool isTrigger)
        {
            foreach (Transform child in this.transform)
            {
                foreach (Transform nestedChild in child.transform)
                {
                    if (nestedChild.tag.CompareTo("Block_Program") == 0)
                    {
                        nestedChild.GetComponent<BoxCollider>().isTrigger = isTrigger;
                    }
                }
            }
        }
    }
}
