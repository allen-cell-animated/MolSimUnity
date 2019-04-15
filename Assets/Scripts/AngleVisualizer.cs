using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleVisualizer : MonoBehaviour 
{
    public Tuple[] angleObjs;

    void Start () 
    {
        foreach (Tuple angle in angleObjs)
        {
            if (angle.objs.Length >= 3)
            {
                CreateArc( angle );
                angle.CalculateAngle();
            }
        }
    }

    void CreateArc (Tuple angle)
    {
        LineRenderer line = (Instantiate( Resources.Load( "Line" ) ) as GameObject).GetComponent<LineRenderer>();
        line.transform.SetParent( transform );
        line.transform.localPosition = Vector3.zero;
        line.transform.localRotation = Quaternion.identity;
        line.transform.localScale = Vector3.one;

        float arcRadius = 0.3f;
        float angleInc = 10f * Mathf.Deg2Rad;
        Vector3 v1 = angle.GetVector( 0, false );
        Vector3 v2 = angle.GetVector( 1, false );
        Vector3 axis = Vector3.Cross( v1, v2 );
        float theta = angle.GetAngle( v1, v2 );
        int n = Mathf.CeilToInt( theta / angleInc );
        Vector3[] positions = new Vector3[n+1];
        for (int i = 0; i <= n; i++)
        {
            positions[i] = angle.objs[1].position + arcRadius * (Quaternion.AngleAxis( Mathf.Rad2Deg * (i < n ? i * angleInc : theta), axis ) * v1);
        }

        line.positionCount = positions.Length;
        line.SetPositions( positions );
    }

}

[System.Serializable]
public class Tuple
{
    public Transform[] objs;
    public float angle;

    public void CalculateAngle ()
    {
        Vector3 v1 = GetVector( 0, true );
        Vector3 v2 = GetVector( 1, true );
        angle = GetAngle( v1, v2 );
    }

    public float GetAngle (Vector3 v1, Vector3 v2)
    {
        return Mathf.Acos( Mathf.Clamp( Vector3.Dot( v1, v2 ), -1f, 1f ) );
    }

    public Vector3 GetVector (int n, bool invertZ)
    {
        return invertZ ? (InvertZ(objs[n == 0 ? 0 : 2].position) - InvertZ(objs[1].position)).normalized 
                       : (objs[n == 0 ? 0 : 2].position - objs[1].position).normalized;
    }

    Vector3 InvertZ (Vector3 v)
    {
        return new Vector3(v.x, v.y, -v.z);
    }
}
