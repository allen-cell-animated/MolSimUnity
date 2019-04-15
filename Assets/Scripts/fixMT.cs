using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fixMT : MonoBehaviour 
{
	void Start () 
    {
        foreach (Transform pf in transform)
        {
            Debug.Log(pf.name);

            foreach (Transform tub in pf)
            {
                Debug.Log(tub.name);

                foreach (Transform site in tub)
                {
                    if (site.name == "out")
                    {
                        site.gameObject.SetActive( true );
                    }
                }
            }
        }
	}
}
