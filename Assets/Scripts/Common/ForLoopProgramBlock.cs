using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForLoopProgramBlock : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject GetCounterAreaGameObject()
    {
        return transform.Find("ForStartCounterArea").gameObject;
    }
    GameObject GetCounterAddGameObject()
    {
        return GetCounterAreaGameObject().transform.Find("CounterAddArea").gameObject;
    }
    GameObject GetCounterMinusGameObject()
    {
        return GetCounterAreaGameObject().transform.Find("CounterMinusArea").gameObject;
    }
    GameObject GetCounterNumberGameObject()
    {
        return GetCounterAreaGameObject().transform.Find("CounterNumberArea").gameObject;
    }
}
