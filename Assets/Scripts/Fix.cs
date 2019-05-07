using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fix : MonoBehaviour 
{
	void Start () 
    {
        //foreach (Transform pf in transform)
        //{
        foreach (Transform tub in transform)
        {
            foreach (Transform site in tub)
            {
                if (site.name.Contains("ring"))
                {
                    site.localPosition *= 1.5f;
                }
            }
        }
        //}
	}
}
