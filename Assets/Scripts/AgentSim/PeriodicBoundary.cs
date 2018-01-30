using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class PeriodicBoundary : MonoBehaviour 
    {
        Vector3 _toParent = Vector3.zero;
        Vector3 toParent
        {
            get
            {
                if (_toParent == Vector3.zero)
                {
                    _toParent = transform.parent.position - transform.position;
                }
                return _toParent;
            }
        }

        float _size = -1;
        float size
        {
            get
            {
                if (_size < 0)
                {
                    _size = 2f * Vector3.Magnitude( toParent );
                }
                return _size;
            }
        }

        void OnCollisionEnter (Collision collision)
        {
            if (collision.rigidbody != null)
            {
                RaycastHit info;
                if (Physics.Raycast( collision.transform.position, toParent.normalized, out info, size, 1 << 8 ))
                {
                    collision.transform.position = info.point - toParent.normalized;
                }
            }
        }
    }
}