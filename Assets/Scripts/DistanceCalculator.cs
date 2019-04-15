using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCalculator : MonoBehaviour 
{
    public Pair[] pairs;

	void Start () 
    {
        for (int i = 0; i < pairs.Length; i++)
        {
            if (pairs[i].objs.Length != 2)
            {
                Debug.LogWarning( "Distances need 2 points, given " + pairs[i].objs.Length );
                return;
            }
            pairs[i].distance = Vector3.Distance( pairs[i].objs[0].position, pairs[i].objs[1].position );
        }
	}
}