using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimeLimiter : MonoBehaviour 
{
    public float lifetime = 1f;
    float startTime;

	void Start ()
	{
        startTime = Time.time;
	}

	void Update () 
    {
        if (Time.time - startTime > lifetime)
        {
            Destroy( gameObject );
        }
	}
}
