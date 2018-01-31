using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that use the physics engine to detect and exit collisions
    public class PhysicalParticleSimulator : ParticleSimulator 
	{
		protected Collider theCollider;
        protected Rigidbody body;

        float GetForceMagnitude (float dTime)
		{
            float meanForce = 5E6f * GetDisplacement( dTime );
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
		}

        float GetTorqueMagnitude (float dTime)
		{
            float meanForce = 5E6f * GetDisplacement( dTime );
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
		}

        protected override void Setup ()
        {
            container.CreatePhysicsBounds();
            AddRigidbodyCollider();
        }

		void AddRigidbodyCollider ()
		{
			theCollider = gameObject.AddComponent<SphereCollider>();
			body = gameObject.AddComponent<Rigidbody>();
			body.drag = 10f;
            body.useGravity = false;
		}

        public override void SimulateFor (float dTime)
        {
            CheckBind();
            collidingSimulators.Clear();

            AddRandomForces( dTime );
        }

		protected virtual void AddRandomForces (float dTime)
		{
			body.velocity = body.angularVelocity = Vector3.zero;
            body.AddForce( GetForceMagnitude( dTime ) * Random.onUnitSphere );
            body.AddTorque( GetTorqueMagnitude( dTime ) * Random.onUnitSphere );
		}

        void OnCollisionEnter (Collision collision)
        {
            if (1 << collision.gameObject.layer == container.boundaryLayer)
            {
                if (container.periodicBoundary)
                {
                    ReflectPeriodically( collision.gameObject.transform.parent.position - transform.position );
                }
            }
            else
            {
                ParticleSimulator[] others = collision.gameObject.GetComponents<ParticleSimulator>();
                if (others != null && others.Length > 0)
                {
                    SaveCollidingSimulators( others );
                }
            }
        }
	}
}