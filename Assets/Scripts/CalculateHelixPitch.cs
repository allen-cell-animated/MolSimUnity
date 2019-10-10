using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateHelixPitch : MonoBehaviour 
{
    public Transform nextActin;
    public Transform nextActinAxis;
    public Transform axisPosition;

    public float pitch;

	void Start () 
    {
        Vector3 toThisAxis = (axisPosition.position - transform.position).normalized;
        Vector3 toNextAxis = (nextActinAxis.position - nextActin.position).normalized;

        float angle = Mathf.Acos( Mathf.Clamp( Vector3.Dot( toThisAxis, toNextAxis ), -1f, 1f ) ) * Mathf.Rad2Deg;
        float length = (nextActinAxis.position - axisPosition.position).magnitude;

        pitch = 360f / angle * length;
	}
}
