using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public ComplexSimulator complexSimulator;

        Population population
        {
            get
            {
                return complexSimulator.population;
            }
        }

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

        float _collisionRadius = -1f;
        float collisionRadius
        {
            get 
            {
                if (_collisionRadius < 0)
                {
                    float d, maxD = 0;
                    foreach (MoleculeSimulator moleculeSimulator in complexSimulator.complex)
                    {
                        d = Vector3.Distance( theTransform.position, moleculeSimulator.theTransform.position ) + moleculeSimulator.collisionRadius;
                        if (d > maxD)
                        {
                            maxD = d;
                        }
                    }
                    _collisionRadius = maxD;
                }
                return _collisionRadius;
            }
        }

        public virtual void Init (ComplexSimulator _complexSimulator)
        {
            complexSimulator = _complexSimulator;
            population.reactor.RegisterParticle( this );
        }

        public virtual void Move (float dTime)
        {
            int i = 0;
            bool moved = false;
            while (!moved && i < population.reactor.maxMoveAttempts)
            {
                moved = MoveRandomStep( dTime );
                i++;
            }
            RotateRandomly( dTime );
        }

        protected virtual bool MoveRandomStep (float dTime)
        {
            Vector3 moveStep = 2E3f * GetDisplacement( dTime ) * Random.onUnitSphere;

            if (!population.reactor.container.IsInBounds( theTransform.position + moveStep ))
            {
                if (population.reactor.periodicBoundary)
                {
                    ReflectPeriodically( population.reactor.container.theTransform.position - (theTransform.position + moveStep) );
                    return true;
                }
                return false;
            }

            if (population.reactor.WillCollide( this, theTransform.position + moveStep ))
            {
                return false;
            }

            theTransform.position += moveStep;
            return true;
        }

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( theTransform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, population.reactor.container.boundaryLayer ))
            {
                theTransform.position = info.point - collisionToCenter.normalized;
            }
        }

        protected virtual void RotateRandomly (float dTime)
        {
            theTransform.rotation *= Quaternion.Euler( 4E4f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        protected float GetDisplacement (float dTime)
        {
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( population.diffusionCoefficient * dTime ) );
        }

        public bool IsCollidingWith (ParticleSimulator other, Vector3 newPosition)
        {
            return other != this 
                && Vector3.Distance( newPosition, other.theTransform.position ) < collisionRadius + other.collisionRadius;
        }

		public void Destroy ()
		{
            population.reactor.UnregisterParticle( this );
		}

		public override string ToString ()
		{
            return "ParticleSimulator " + name;
		}
	}
}
