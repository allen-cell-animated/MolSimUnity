using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Container : MonoBehaviour
	{
        public Vector3 size;
        public bool periodicBoundary = true;

        BoxWalls walls;

        public virtual bool PointIsInBounds (Vector3 point)
        {
            return point.x < transform.position.x + size.x / 2f && point.x > transform.position.x - size.x / 2f
                        && point.y < transform.position.y + size.y / 2f && point.y > transform.position.y - size.y / 2f
                        && point.z < transform.position.z + size.z / 2f && point.z > transform.position.z - size.z / 2f;
        }

        public virtual Vector3 GetRandomPointInBounds (float margin = 0)
        {
            return transform.position + (1f - margin) * (new Vector3( size.x * Random.value, size.y * Random.value, size.z * Random.value ) - size / 2f);
        }

        void OnDrawGizmos ()
        {
            DrawGizmo();
        }

        protected virtual void DrawGizmo ()
        {
            Gizmos.DrawWireCube( transform.position, size );
        }

        public virtual void CreatePhysicsBounds ()
        {
            if (walls == null)
            {
                walls = gameObject.AddComponent<BoxWalls>();
                walls.Init( size, 25f, periodicBoundary );
            }
        }
	}
}