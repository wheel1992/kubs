using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class UIHintsArrowPointer : MonoBehaviour
{

    //[SerializeField] GameObject ArrowPrefab;
    // private float arrowMoveSpeed = 0.0001f;
    // private float xPosAbove = 2.0f;
    // private bool _onShow = false;
    // private float distAccuracy;
    // private Vector3 startPos;
    // private Vector3 endPos;
    // private float incrementor;

    public bool ShouldFloat = true;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();
    IEnumerator _floatCoroutine;

    public void BeginFloat()
    {
        // if (_floatCoroutine != null)
        // {
        //     StopCoroutine(_floatCoroutine);
        // }
        // _floatCoroutine = StartFloat();
        // StartCoroutine(_floatCoroutine);
        ShouldFloat = true;
    }
    public void StopFloat()
    {
        ShouldFloat = false;
    }
    public void Hide()
    {
        // if (_floatCoroutine != null)
        // {
        //     StopCoroutine(_floatCoroutine);
        // }
        StopFloat();
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        //BeginFloat();
    }

    // Use this for initialization
    void Start()
    {
        // incrementor = 0.01f;
        // distAccuracy = arrowMoveSpeed;
        // endPos = this.gameObject.transform.position;
        // startPos = endPos + new Vector3(0, xPosAbove, 0);

        posOffset = transform.position;
        BeginFloat();
    }

    // Update is called once per frame
    void Update()
    {
        // incrementor += Time.deltaTime;
        // if (Vector3.Distance(this.gameObject.transform.position, endPos) < distAccuracy)
        // {
        //     this.gameObject.transform.position = startPos;
        //     incrementor = 0.01f;
        // }
        // else
        // {
        //     this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, endPos, incrementor);
        // }

        if (ShouldFloat)
        {
            // Spin object around Y-Axis
            // transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

            // Float up/down with a Sin()
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            transform.position = tempPos;
        }
    }

    private IEnumerator StartFloat()
    {
        while (true)
        {
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
