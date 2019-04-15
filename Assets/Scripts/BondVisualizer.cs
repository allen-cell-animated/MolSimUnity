using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondVisualizer : MonoBehaviour 
{
    public Pair[] boundObjs;

    void Start () 
    {
        foreach (Pair bond in boundObjs)
        {
            if (bond.objs.Length >= 2)
            {
                CreateLine( bond );
                bond.CalculateDistance();
            }
        }
    }

    void CreateLine (Pair bond)
    {
        LineRenderer line = (Instantiate( Resources.Load( "Line" ) ) as GameObject).GetComponent<LineRenderer>();
        line.transform.SetParent( transform );
        line.transform.localPosition = Vector3.zero;
        line.transform.localRotation = Quaternion.identity;
        line.transform.localScale = Vector3.one;

        line.positionCount = 2;
        line.SetPositions( new Vector3[] {bond.objs[0].position, bond.objs[1].position} );
    }
}

[System.Serializable]
public class Pair
{
    public Transform[] objs;
    public float distance;

    public void CalculateDistance ()
    {
        distance = Vector3.Distance( objs[0].position, objs[1].position );
    }
}
