using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Mover : MonoBehaviour
    {
        public Reactor reactor;

        [SerializeField] float diffusionCoefficient;
        [SerializeField] float collisionRadius;

        public virtual void Init (Reactor _reactor, float _diffusionCoefficient, float _collisionRadius)
        {
            reactor = _reactor;
            diffusionCoefficient = _diffusionCoefficient;
            collisionRadius = _collisionRadius;

            reactor.RegisterMover( this );
        }

        public Vector3 GetRandomDisplacement(float dTime)
        {
            return GetDisplacement( dTime ) * Random.onUnitSphere;
        }

        public void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( transform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, reactor.container.boundaryLayer ))
            {
                transform.position = info.point - collisionToCenter.normalized;
            }
        }

        public void RotateRandomly (float dTime)
        {
            transform.rotation *= Quaternion.Euler( 20f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        private float GetDisplacement (float dTime)
        {
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * dTime ) );
        }

        public bool WillCollideWith (Mover other, Vector3 newPosition)
        {
            return other != this
                && Vector3.Distance( newPosition, other.transform.position ) < collisionRadius + other.collisionRadius;
        }

		public void Destroy ()
		{
            reactor.UnregisterMover( this );
            Destroy( gameObject );
		}
	}
}
