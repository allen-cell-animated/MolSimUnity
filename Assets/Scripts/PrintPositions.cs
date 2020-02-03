﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum BioSystem
{
    Microtubules,
    Actin
}

public class PrintPositions : MonoBehaviour 
{
    public BioSystem systemType;

    void Start () 
    {
        if (systemType == BioSystem.Microtubules)
        {
            RecordMicrotubules();
        }
        else if (systemType == BioSystem.Actin)
        {
            RecordActin();
        }
    }

    void RecordMicrotubules ()
    {
        string s = "";
        int t = 0;
        foreach (Transform pf in transform)
        {
            foreach (Transform tub in pf)
            {
                if (!tub.name.Contains("frayed"))
                {
                    s += FormatPosition( tub.position, Vector3.zero );
                    t++;
                }
            }
        }
        File.WriteAllText( "/Users/blairl/Desktop/mt_positions.txt", s );
        Debug.Log("wrote microtubule positions to file! " + t);
    }

    void RecordActin ()
    {
        string s = "";
        foreach (Transform a in transform)
        {
            s += FormatPosition( a.position, 15f * Vector3.right );
        }
        File.WriteAllText( "/Users/blairl/Desktop/actin_positions.txt", s );
        Debug.Log("wrote actin positions to file!");
    }

    string FormatPosition (Vector3 position, Vector3 shift)
    {
        return "[" + (Mathf.Round( 100f * (position.x + shift.x) ) / 100f) + ", " 
                   + (Mathf.Round( 100f * (position.y + shift.y) ) / 100f) + ", " 
                   + (Mathf.Round( 100f * -(position.z + shift.z) ) / 100f) + "],";
    }
}