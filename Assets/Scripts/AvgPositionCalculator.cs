using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvgPositionCalculator : MonoBehaviour 
{
    public Transform[] transforms;
    public Vector3 averagePosition;

    void Start () 
    {
        averagePosition = Vector3.zero;
        for (int i = 0; i < transforms.Length; i++)
        {
            averagePosition += transforms[i].position;
        }
        averagePosition /= transforms.Length;
        GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = averagePosition;
    }
}
