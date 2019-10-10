using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintQuaternion : MonoBehaviour 
{
	void Start () 
    {
        Debug.Log( name + " rot wxyz = (" + transform.localRotation.w + ", " + transform.localRotation.x + ", " + transform.localRotation.y + ", " + transform.localRotation.z + ")");

        //Quaternion q = new Quaternion(0.3396354f, -0.6389218f, 0.06508861f, 0.687161f); //(33.4, 266.8, 335.6)
        //Quaternion q = new Quaternion(-0.8209659f, 0.2822475f, -0.1120121f, 0.4835335f); //(313.1, 138.0, 236.9)
        //Quaternion q = new Quaternion(0.5f, 0.5f, 0.51f, -0.5f); //(-90, 0, -90)
        //print(q.eulerAngles);
	}
}
