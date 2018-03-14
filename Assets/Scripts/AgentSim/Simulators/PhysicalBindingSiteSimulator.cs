using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class PhysicalBindingSiteSimulator : BindingSiteSimulator 
    {
        protected SphereCollider sphereCollider;
        protected Rigidbody body;

        public override void Init (BindingSitePopulation _population, MoleculeSimulator _molecule)
        {
            base.Init( _population, _molecule );

            AddRigidbodyCollider();
        }

        protected void AddRigidbodyCollider ()
        {
            gameObject.layer = 9;
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = population.interactionRadius;
            sphereCollider.isTrigger = true;
            body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
        }

        public override void SimulateFor (float dTime)
        {
            
        }

        void OnTriggerEnter (Collider other)
        {
            HandleCollision( other );
        }

        protected virtual void HandleCollision (Collider other)
        {
            BindingSiteSimulator otherSite = other.gameObject.GetComponent<BindingSiteSimulator>();
            if (otherSite != null)
            {
                //TODO handle collisions with physics engine
            }
        }
    }
}