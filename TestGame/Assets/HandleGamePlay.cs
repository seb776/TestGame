using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleGamePlay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GenerateLevel.Instance.FromFile("Level1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
