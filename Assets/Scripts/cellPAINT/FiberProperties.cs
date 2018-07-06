using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiberProperties : ScriptableObject 
{
    public float drawDistanceZ;
    public float monomerLength;
    public float hingeLimitMin;
    public float hingeLimitMax;
    public float springDamper;
    public GameObject monomerPrefab;
}
