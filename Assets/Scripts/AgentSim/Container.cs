using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Container : MonoBehaviour
	{
        public float scale;
        public float volume;
        public bool periodicBoundary = true;

        [HideInInspector] public LayerMask boundaryLayer = 1 << 8;
        Vector3 size;
        Walls walls;

        public virtual void Init (float _scale, float _volume, bool _periodicBoundary)
        {
            scale = _scale;
            volume = _volume;
            CalculateSize();
            periodicBoundary = _periodicBoundary;
            if (periodicBoundary)
            {
                CreateBounds();
            }
        }

        protected virtual void CalculateSize ()
        {
            float side = Mathf.Pow( volume * 1E-6f, 1f / 3f ) / scale;
            size = side * Vector3.one;
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

        public virtual void CreateBounds ()
        {
            if (walls == null)
            {
                walls = gameObject.AddComponent<Walls>();
                walls.Init( size, 100f );
            }
        }

        public virtual bool IsOutOfBounds (Vector3 point, out Vector3 pointToCenter)
        {
            bool inBounds = point.x < transform.position.x + size.x / 2f && point.x > transform.position.x - size.x / 2f
                         && point.y < transform.position.y + size.y / 2f && point.y > transform.position.y - size.y / 2f
                         && point.z < transform.position.z + size.z / 2f && point.z > transform.position.z - size.z / 2f;
            pointToCenter = inBounds ? Vector3.zero : transform.position - point;
            return !inBounds;
        }
	}
}