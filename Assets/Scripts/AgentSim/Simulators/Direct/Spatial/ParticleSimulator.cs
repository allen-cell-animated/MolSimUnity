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

        ParticleReactor _reactor;
        ParticleReactor reactor
        {
            get
            {
                if (_reactor == null)
                {
                    _reactor = GetComponentInParent<ParticleReactor>();
                }
                return _reactor;
            }
        }

        Container _container;
        public Container container
        {
            get
            {
                if (_container == null)
                {
                    _container = GetComponentInParent<Container>();
                }
                return _container;
            }
        }

        protected float GetDisplacement (float dTime)
		{
            return Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * dTime );
		}

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( transform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, container.boundaryLayer ))
            {
                transform.position = info.point - collisionToCenter.normalized;
            }
        }

        protected virtual void SaveCollidingSimulators (ParticleSimulator[] others)
        {
            collidingSimulators.AddRange( others );
        }

        protected virtual bool CheckBind ()
        {
            reactor.reactionData.Shuffle();
            foreach (ParticleReaction reactionData in reactor.reactionData)
            {
                collidingSimulators.Shuffle();
                foreach (ParticleSimulator other in collidingSimulators)
                {
                    if (reactionData.reaction.ReactantsEqual( agent.species, other.agent.species ) && reactionData.ShouldHappen())
                    {
                        DoBind( other, reactionData.reaction );
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void DoBind (ParticleSimulator other, Reaction reaction)
        {
            // TODO
        }

        protected virtual Vector3 GetExitVector ()
        {
            int n = 0;
            Vector3 exitVector = Vector3.zero;
            foreach (ParticleSimulator other in collidingSimulators)
            {
                exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                n++;
            }
            return exitVector.normalized;
        }
	}
}