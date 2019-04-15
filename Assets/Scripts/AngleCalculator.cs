using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleCalculator : MonoBehaviour 
{
    public Tuple[] triples;

    void Start () 
    {
        string result = "";
        for (int i = 0; i < triples.Length; i++)
        {
            if (triples[i].objs.Length != 3)
            {
                Debug.LogWarning( "Angles need 3 points, given " + triples[i].objs.Length );
                return;
            }
            Vector3 v1 = InvertZ(triples[i].objs[0].position) - InvertZ(triples[i].objs[1].position);
            Vector3 v2 = InvertZ(triples[i].objs[2].position) - InvertZ(triples[i].objs[1].position);
            triples[i].angle = Mathf.Acos( Vector3.Dot( v1.normalized, v2.normalized ) );
            result += triples[i].angle.ToString() + " ";
        }
        Debug.Log( result );
    }

    Vector3 InvertZ (Vector3 v)
    {
        return new Vector3(v.x, v.y, -v.z);
    }
}
