using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnEnd : MonoBehaviour {
    public float WaitTime;
    float _startTime;
	// Use this for initialization
	void Start () {
        _startTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		if ((Time.realtimeSinceStartup - _startTime) > WaitTime)
        {
            Destroy(this.gameObject);
        }
	}
}
