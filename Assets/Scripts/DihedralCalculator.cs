using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DihedralCalculator : MonoBehaviour 
{
    public Tuple[] dihedrals;

    void Start () 
    {
        for (int i = 0; i < dihedrals.Length; i++)
        {
            if (dihedrals[i].objs.Length != 4)
            {
                Debug.LogWarning( "Dihedrals need 4 points, given " + dihedrals[i].objs.Length );
                return;
            }
            Vector3 v1 = dihedrals[i].objs[0].position - dihedrals[i].objs[1].position;
            Vector3 v2 = dihedrals[i].objs[3].position - dihedrals[i].objs[2].position;
            dihedrals[i].angle = Mathf.Acos( Vector3.Dot( v1.normalized, v2.normalized ) ) * (dihedrals[i].units == AngleUnits.degrees ? 180f / Mathf.PI : 1f);
        }
    }
}
