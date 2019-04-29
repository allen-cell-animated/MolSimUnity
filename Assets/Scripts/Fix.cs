using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotSeq
{zyx, zyz, zxy, zxz, yxz, yxy, yzx, yzy, xyz, xyx, xzy,xzx}

public class Fix : MonoBehaviour 
{
    public RotSeq rotationSequence;

	void Start () 
    {
        Vector3 start = new Vector3(-0.88871591f, -0.45771277f, 0.02613542f);
        Vector3 goal = Vector3.forward;

        Vector3 axis = Vector3.Cross(start, goal);
        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(start, goal));

        Quaternion q = Quaternion.AngleAxis(angle, axis);
        float[] ea = new float[3];
        quaternion2Euler(q, ea, rotationSequence);
        Vector3 result = Mathf.Rad2Deg * new Vector3(ea[0], ea[1], ea[2]);

        print(axis + " " + angle + " " + result);
	}

    ///////////////////////////////
    // Quaternion to Euler
    ///////////////////////////////

    void twoaxisrot(float r11, float r12, float r21, float r31, float r32, float[] res)
    {
        res[0] = Mathf.Atan2( r11, r12 );
        res[1] = Mathf.Acos( r21 );
        res[2] = Mathf.Atan2( r31, r32 );
    }

    void threeaxisrot(float r11, float r12, float r21, float r31, float r32, float[] res)
    {
        res[0] = Mathf.Atan2( r31, r32 );
        res[1] = Mathf.Asin( r21 );
        res[2] = Mathf.Atan2( r11, r12 );
    }

    void quaternion2Euler(Quaternion q, float[] res, RotSeq rotSeq)
    {
        switch(rotSeq){
            case RotSeq.zyx:
                threeaxisrot( 2*(q.x*q.y + q.w*q.z),
                   q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                   -2*(q.x*q.z - q.w*q.y),
                   2*(q.y*q.z + q.w*q.x),
                   q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                   res);
                break;

            case RotSeq.zyz:
                twoaxisrot( 2*(q.y*q.z - q.w*q.x),
                 2*(q.x*q.z + q.w*q.y),
                 q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                 2*(q.y*q.z + q.w*q.x),
                 -2*(q.x*q.z - q.w*q.y),
                 res);
                break;

            case RotSeq.zxy:
                threeaxisrot( -2*(q.x*q.y - q.w*q.z),
                   q.w*q.w - q.x*q.x + q.y*q.y - q.z*q.z,
                   2*(q.y*q.z + q.w*q.x),
                   -2*(q.x*q.z - q.w*q.y),
                   q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                   res);
                break;

            case RotSeq.zxz:
                twoaxisrot( 2*(q.x*q.z + q.w*q.y),
                 -2*(q.y*q.z - q.w*q.x),
                 q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                 2*(q.x*q.z - q.w*q.y),
                 2*(q.y*q.z + q.w*q.x),
                 res);
                break;

            case RotSeq.yxz:
                threeaxisrot( 2*(q.x*q.z + q.w*q.y),
                   q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                   -2*(q.y*q.z - q.w*q.x),
                   2*(q.x*q.y + q.w*q.z),
                   q.w*q.w - q.x*q.x + q.y*q.y - q.z*q.z,
                   res);
                break;

            case RotSeq.yxy:
                twoaxisrot( 2*(q.x*q.y - q.w*q.z),
                 2*(q.y*q.z + q.w*q.x),
                 q.w*q.w - q.x*q.x + q.y*q.y - q.z*q.z,
                 2*(q.x*q.y + q.w*q.z),
                 -2*(q.y*q.z - q.w*q.x),
                 res);
                break;

            case RotSeq.yzx:
                threeaxisrot( -2*(q.x*q.z - q.w*q.y),
                   q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                   2*(q.x*q.y + q.w*q.z),
                   -2*(q.y*q.z - q.w*q.x),
                   q.w*q.w - q.x*q.x + q.y*q.y - q.z*q.z,
                   res);
                break;

            case RotSeq.yzy:
                twoaxisrot( 2*(q.y*q.z + q.w*q.x),
                 -2*(q.x*q.y - q.w*q.z),
                 q.w*q.w - q.x*q.x + q.y*q.y - q.z*q.z,
                 2*(q.y*q.z - q.w*q.x),
                 2*(q.x*q.y + q.w*q.z),
                 res);
                break;

            case RotSeq.xyz:
                threeaxisrot( -2*(q.y*q.z - q.w*q.x),
                   q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                   2*(q.x*q.z + q.w*q.y),
                   -2*(q.x*q.y - q.w*q.z),
                   q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                   res);
                break;

            case RotSeq.xyx:
                twoaxisrot( 2*(q.x*q.y + q.w*q.z),
                 -2*(q.x*q.z - q.w*q.y),
                 q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                 2*(q.x*q.y - q.w*q.z),
                 2*(q.x*q.z + q.w*q.y),
                 res);
                break;

            case RotSeq.xzy:
                threeaxisrot( 2*(q.y*q.z + q.w*q.x),
                   q.w*q.w - q.x*q.x + q.y*q.y - q.z*q.z,
                   -2*(q.x*q.y - q.w*q.z),
                   2*(q.x*q.z + q.w*q.y),
                   q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                   res);
                break;

            case RotSeq.xzx:
                twoaxisrot( 2*(q.x*q.z - q.w*q.y),
                 2*(q.x*q.y + q.w*q.z),
                 q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                 2*(q.x*q.z + q.w*q.y),
                 -2*(q.x*q.y - q.w*q.z),
                 res);
                break;

            default:
                print("Unknown rotation sequence");
                break;
        }
    }
}
