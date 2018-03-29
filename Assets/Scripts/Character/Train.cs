using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Train : MonoBehaviour {
    
    [SerializeField] private GameObject[] trainWaypoints;

    // Use this for initialization
    void Start()
    {
        // ...
    }

    // Update is called once per frame
    void Update()
    {
        // ...
    }
    private void MoveTrain()
    {
        if (this.transform.position.Equals(trainWaypoints))
        {

        } else if (this.transform.position.Equals(trainWaypoints))
        {

        } else Debug.Log("Train is not in position(s)");
    }
}
