using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that use the physics engine to detect and exit collisions
    public class PhysicalParticleSimulator : ParticleSimulator 
	{
        protected SphereCollider sphereCollider;
        protected Rigidbody body;

        protected float GetForceMagnitude (float dTime)
		{
            float meanForce = 5E6f * GetDisplacement( dTime );
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
		}

        protected float GetTorqueMagnitude (float dTime)
		{
            float meanForce = 5E6f * GetDisplacement( dTime );
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
        }

        public override void Init (MoleculeState moleculeState, ParticlePopulation _population)
        {
            base.Init( moleculeState, _population );

            population.reactor.container.CreatePhysicsBounds();
            AddRigidbodyCollider();
        }

        protected void AddRigidbodyCollider ()
		{
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = population.molecule.radius;
			body = gameObject.AddComponent<Rigidbody>();
			body.drag = 10f;
            body.useGravity = false;
		}

        public override void SimulateFor (float dTime)
        {
            CheckBind();
            collidingParticles.Clear();

            if (canMove)
            {
                AddRandomForces( dTime );
            }
        }

		protected virtual void AddRandomForces (float dTime)
		{
			body.velocity = body.angularVelocity = Vector3.zero;
            body.AddForce( GetForceMagnitude( dTime ) * Random.onUnitSphere );
            body.AddTorque( GetTorqueMagnitude( dTime ) * Random.onUnitSphere );
		}

        void OnCollisionEnter (Collision collision)
        {
            HandleCollision( collision );
        }

        protected virtual void HandleCollision (Collision collision)
        {
            if (1 << collision.gameObject.layer == population.reactor.container.boundaryLayer)
            {
                if (population.reactor.container.periodicBoundary)
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

        protected override void ToggleMotion (bool move)
        {
            canMove = move;
            body.velocity = body.angularVelocity = Vector3.zero;
        }
	}
}