using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ClosestPointOnLine : MonoBehaviour 
{
    public Transform lineStart;
    public Transform lineEnd;

	void Start () 
    {
        Transform t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        t.position = GetNearestPointOnLine( lineStart.position, lineEnd.position, transform.position );
        t.name = name + "_axis";

        File.AppendAllText( "/Users/blairl/Desktop/mt_distances.txt", t.position.z.ToString() + ", " );
	}

    public Vector3 GetNearestPointOnLine (Vector3 _lineStart, Vector3 _lineEnd, Vector3 _point)
    {
        Vector3 lineDir = (_lineEnd - _lineStart).normalized;
        Vector3 v = _point - _lineStart;
        float d = Vector3.Dot(v, lineDir);
        return _lineStart + lineDir * d;
    }
}
