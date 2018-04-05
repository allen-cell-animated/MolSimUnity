using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour 
{
    public bool alwaysUpdate = true;

    void Update () 
    {
        if (alwaysUpdate)
        {
            LookAtCamera();
        }
    }

    public void LookAtCamera ()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
