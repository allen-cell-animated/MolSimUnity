using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestPointOnLine : MonoBehaviour 
{
	void Start () 
    {
		
	}

    public Vector3 GetNearestPointOnLine (Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDir = (lineEnd - lineStart).normalized;
        Vector3 v = point - lineStart;
        float d = Vector3.Dot(v, lineDir);
        return lineStart + lineDir * d;
    }
}
