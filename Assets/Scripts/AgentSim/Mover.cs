using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Mover
    {
        public Reactor reactor;
        public Vector3 position;
        public Quaternion rotation;

        public void SetWorldTransform (RelativeTransform worldTransform)
        {
            position = worldTransform.position;
            rotation = Quaternion.Euler( worldTransform.rotation );
        }

        [SerializeField] float diffusionCoefficient;
        [SerializeField] float collisionRadius;

        public Mover (Reactor _reactor, Vector3 _position, Quaternion _rotation, float _diffusionCoefficient, float _collisionRadius)
        {
            reactor = _reactor;
            position = _position;
            rotation = _rotation;
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
            if (Physics.Raycast( position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, reactor.container.boundaryLayer ))
            {
                position = info.point - collisionToCenter.normalized;
            }
        }

        public void RotateRandomly (float dTime)
        {
            rotation *= Quaternion.Euler( 2.5f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        private float GetDisplacement (float dTime)
        {
            //mean squared displacement = 6 * diffusion coefficient * dT
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( 6f * diffusionCoefficient * dTime ) );
        }

        public bool WillCollideWith (Mover other, Vector3 newPosition)
        {
            return other != this
                && Vector3.Distance( newPosition, other.position ) < collisionRadius + other.collisionRadius;
        }

		public void Destroy ()
		{
            reactor.UnregisterMover( this );
		}
	}
}
