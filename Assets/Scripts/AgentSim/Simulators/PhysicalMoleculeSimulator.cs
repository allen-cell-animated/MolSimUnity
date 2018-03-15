using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that use the physics engine to detect and exit collisions
    public class PhysicalMoleculeSimulator : MoleculeSimulator 
	{
        protected SphereCollider sphereCollider;
        protected Rigidbody body;
        protected List<PhysicalMoleculeSimulator> collidingMolecules = new List<PhysicalMoleculeSimulator>();

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

        public override void Init (ComplexPopulation _population, MoleculeState moleculeState = null)
        {
            base.Init( _population, moleculeState );

            population.reactor.container.CreatePhysicsBounds();
            AddRigidbodyCollider();
        }

        protected void AddRigidbodyCollider ()
		{
            gameObject.layer = 9;
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = population.collisionRadius;
			body = gameObject.AddComponent<Rigidbody>();
			body.drag = 10f;
            body.useGravity = false;
		}

        void Update ()
        {
            //CheckBind();
            collidingMolecules.Clear();

            if (canMove)
            {
                AddRandomForces( World.Instance.dT );
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
                MoleculeSimulator[] others = collision.gameObject.GetComponents<MoleculeSimulator>();
                if (others != null && others.Length > 0)
                {
                    collidingMolecules.AddRange( (PhysicalMoleculeSimulator[])others );
                }
            }
        }

        public override void ToggleMotion (bool move)
        {
            canMove = move;
            body.velocity = body.angularVelocity = Vector3.zero;
        }
	}
}