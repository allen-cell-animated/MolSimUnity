using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class ParticleSimulator : Simulator 
    {
        protected ParticlePopulation population;
		protected float diffusionCoefficient;

        protected List<ParticleSimulator> collidingSimulators = new List<ParticleSimulator>();

        public void Init (ParticlePopulation _population)
        {
            population = _population;
            diffusionCoefficient = population.molecule.diffusionCoefficient;
            DoAdditionalInit();
        }

        protected abstract void DoAdditionalInit ();

        protected float GetDisplacement (float dTime)
		{
            return Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * dTime );
		}

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( transform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, population.reactor.container.boundaryLayer ))
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
            population.reactor.reactionData.Shuffle();
            foreach (ParticleReaction reactionData in population.reactor.reactionData)
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

        protected virtual Vector3 GetExitDirection ()
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