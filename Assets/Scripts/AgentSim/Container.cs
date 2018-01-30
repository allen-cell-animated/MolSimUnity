using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Container : MonoBehaviour
	{
        public Vector3 size;
        public bool periodicBoundary = true;
        public LayerMask boundaryLayer;

        Walls walls;
        List<ManagedParticleSimulator> simulators = new List<ManagedParticleSimulator>();

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

        public void RegisterSimulator (ManagedParticleSimulator simulator)
        {
            simulators.Add( simulator );
        }

        public virtual void CreatePhysicsBounds ()
        {
            if (walls == null)
            {
                walls = gameObject.AddComponent<Walls>();
                walls.Init( size, 100f );
            }
        }

        public virtual bool IsOutOfBounds (Vector3 point, out Vector3 collisionToCenter)
        {
            bool inBounds = point.x < transform.position.x + size.x / 2f && point.x > transform.position.x - size.x / 2f
                         && point.y < transform.position.y + size.y / 2f && point.y > transform.position.y - size.y / 2f
                         && point.z < transform.position.z + size.z / 2f && point.z > transform.position.z - size.z / 2f;
            collisionToCenter = inBounds ? Vector3.zero : transform.position - point;
            return !inBounds;
        }

        public virtual bool WillCollide (ManagedParticleSimulator simulator, Vector3 newPosition, out ManagedParticleSimulator[] others)
        {
            List<ManagedParticleSimulator> othersList = new List<ManagedParticleSimulator>();
            foreach (ManagedParticleSimulator other in simulators)
            {
                if (SimulatorsAreColliding( simulator, other ))
                {
                    othersList.Add( other );
                }
            }
            others = othersList.ToArray();
            return others.Length > 0;
        }

        bool SimulatorsAreColliding (ManagedParticleSimulator simulator1, ManagedParticleSimulator simulator2)
        {
            return simulator1 != simulator2 && Vector3.Distance( simulator1.transform.position, simulator2.transform.position ) < simulator1.radius + simulator2.radius;
        }
	}
}