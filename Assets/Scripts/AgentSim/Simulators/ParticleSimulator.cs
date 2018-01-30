using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class ParticleSimulator : Simulator 
    {
		[Tooltip( "([Agent's scale] meters)^2 / s" )]
		public float diffusionCoefficient;

        protected List<ParticleSimulator> collidingSimulators = new List<ParticleSimulator>();

        protected float GetDisplacement (float dTime)
		{
            return Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * dTime );
		}

        protected virtual void ReflectPeriodically (Vector3 wallToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( transform.position, wallToCenter.normalized, out info, 2f * wallToCenter.magnitude, agent.container.boundaryLayer ))
            {
                transform.position = info.point - wallToCenter.normalized;
            }
        }

        protected virtual void SaveCollidingSimulators (ParticleSimulator[] others)
        {
            collidingSimulators.AddRange( others );
        }

        protected virtual bool CheckBind ()
        {
            foreach (ParticleSimulator other in collidingSimulators)
            {
                // TODO handle bind
            }
            collidingSimulators.Clear();
            return false;
        }

        protected virtual bool CheckBind (out Vector3 exitVector)
        {
            int n = 0;
            exitVector = Vector3.zero;
            foreach (ParticleSimulator other in collidingSimulators)
            {
                // TODO handle bind
                exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                n++;
            }
            exitVector.Normalize();
            collidingSimulators.Clear();
            return false;
        }
	}
}