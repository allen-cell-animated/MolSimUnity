using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtOther : MonoBehaviour {

    public Transform other;

	// Use this for initialization
	void Start () {

        transform.rotation = Quaternion.LookRotation(other.position-transform.position);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
