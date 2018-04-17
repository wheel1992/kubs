using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class UIHintsArrowPointer : MonoBehaviour {

    //[SerializeField] GameObject ArrowPrefab;
    private float arrowMoveSpeed = 0.1f;
    private float xPosAbove = 1.0f;
    private bool _onShow = false;
    private float distAccuracy;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 velocity = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start()
    {
        distAccuracy = arrowMoveSpeed;
        endPos = this.gameObject.transform.position;
        startPos = endPos + new Vector3(0, xPosAbove, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.gameObject.transform.position, endPos) < distAccuracy)
            {
                this.gameObject.transform.position = startPos;
            }
        else
            {
                this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, endPos, ref velocity, Time.deltaTime * arrowMoveSpeed);
            }
    }

    //public GameObject CreateArrowPointer(Vector3 localPos)
    //{
    //    GameObject arrowPointer = Instantiate(ArrowPrefab);
    //    arrowPointer.transform.position = localPos;
    //    arrowPointer.AddComponent<UIHintsArrowPointer>();
    //    arrowPointer.name = "UIHintsArrowPointer";
    //    return arrowPointer;
    //}
}
