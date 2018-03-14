using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBS : MonoBehaviour 
{
    public Transform otherBS;

	void Start () 
    {
        transform.parent.position = otherBS.TransformPoint( transform.InverseTransformPoint( transform.parent.position ) );
        transform.parent.rotation = transform.parent.rotation * Quaternion.Inverse( transform.rotation ) * otherBS.rotation;
	}
}
