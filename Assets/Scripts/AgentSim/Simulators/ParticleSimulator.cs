using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public Reactor reactor;

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

        float diffusionCoefficient;
        float collisionRadius;

        public virtual void Init (Reactor _reactor, float _diffusionCoefficient, float _collisionRadius)
        {
            reactor = _reactor;
            diffusionCoefficient = _diffusionCoefficient;
            collisionRadius = _collisionRadius;

            reactor.RegisterParticle( this );
        }

        public virtual void Move (float dTime)
        {
            int i = 0;
            bool moved = false;
            while (!moved && i < reactor.maxMoveAttempts)
            {
                moved = MoveRandomStep( dTime );
                i++;
            }
            RotateRandomly( dTime );
        }

        protected virtual bool MoveRandomStep (float dTime)
        {
            Vector3 moveStep = 2E3f * GetDisplacement( dTime ) * Random.onUnitSphere;

            if (!reactor.container.IsInBounds( theTransform.position + moveStep ))
            {
                if (reactor.periodicBoundary)
                {
                    ReflectPeriodically( reactor.container.theTransform.position - (theTransform.position + moveStep) );
                    return true;
                }
                return false;
            }

            if (reactor.WillCollide( this, theTransform.position + moveStep ))
            {
                return false;
            }

            theTransform.position += moveStep;
            return true;
        }

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( theTransform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, reactor.container.boundaryLayer ))
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
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * dTime ) );
        }

        public bool IsCollidingWith (ParticleSimulator other, Vector3 newPosition)
        {
            return other != this 
                && Vector3.Distance( newPosition, other.theTransform.position ) < collisionRadius + other.collisionRadius;
        }

		public void Destroy ()
		{
            reactor.UnregisterParticle( this );
		}

		public override string ToString ()
		{
            return "ParticleSimulator " + name;
		}
	}
}
