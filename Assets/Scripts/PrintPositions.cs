using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PrintPositions : MonoBehaviour 
{
    void Start () 
    {
        string s = "";
        int t = 0;
        foreach (Transform pf in transform)
        {
            foreach (Transform tub in pf)
            {
                s += FormatPosition( tub.position );
                t++;
                foreach (Transform site in tub)
                {
                    if (site.name != "in")
                    {
                        t++;
                        s += FormatPosition( site.position );
                    }
                }
            }
        }
        File.WriteAllText( "/Users/blairl/Desktop/mt_positions.txt", s );
        Debug.Log("wrote positions to file! " + (t / 208f));
	}

    string FormatPosition (Vector3 position)
    {
        return "[" + (Mathf.Round( 100f * position.x ) / 100f) + ", " 
                   + (Mathf.Round( 100f * position.y ) / 100f) + ", " 
                   + (Mathf.Round( 100f * (position.z - 35f) ) / 100f) + "],";
    }
}
