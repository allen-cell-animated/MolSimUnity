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
        [HideInInspector] public Vector3 size;
        Walls walls;

        Transform _theTransform;
        public Transform theTransform
        {
            get
            {
                if (_theTransform == null)
                {
                    _theTransform = transform;
                }
                return _theTransform;
            }
        }

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
            return theTransform.position + (1f - margin) * (new Vector3( size.x * Random.value, size.y * Random.value, size.z * Random.value ) - size / 2f);
        }

        void OnDrawGizmos ()
        {
            DrawGizmo();
        }

        protected virtual void DrawGizmo ()
        {
            Gizmos.DrawWireCube( theTransform.position, size );
        }

        protected virtual void CreateBounds ()
        {
            if (walls == null)
            {
                walls = gameObject.AddComponent<Walls>();
                walls.Init( size, 100f );
            }
        }

        public virtual bool IsInBounds (Vector3 point)
        {
            return point.x < theTransform.position.x + size.x / 2f && point.x > theTransform.position.x - size.x / 2f
                && point.y < theTransform.position.y + size.y / 2f && point.y > theTransform.position.y - size.y / 2f
                && point.z < theTransform.position.z + size.z / 2f && point.z > theTransform.position.z - size.z / 2f;
        }
	}
}