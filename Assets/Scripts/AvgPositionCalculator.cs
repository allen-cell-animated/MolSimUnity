using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvgPositionCalculator : MonoBehaviour 
{
    public Vector3 averagePosition;

    void Start () 
    {
        averagePosition = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            averagePosition += transform.GetChild(i).position;
        }
        averagePosition /= transform.childCount;
        GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = averagePosition;
    }
}
