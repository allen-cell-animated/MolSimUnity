using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateBranchAngle : MonoBehaviour 
{
    public Transform a;
    public Transform b;
    public Transform c;
    public Transform d;
    public float angle;

	void Start () 
    {
        Vector3 v0 = (b.position - a.position).normalized;
        Vector3 v1 = (d.position - c.position).normalized;
        angle = Mathf.Acos( Mathf.Clamp( Vector3.Dot( v0, v1 ), -1f, 1f ) ) * Mathf.Rad2Deg;
	}
}
