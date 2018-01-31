using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class ParticleSimulator : Simulator 
    {
        protected ParticlePopulation population;
		protected float diffusionCoefficient;
        public bool canMove = true;

        protected List<ParticleSimulator> collidingParticles = new List<ParticleSimulator>();
        [HideInInspector] public List<ParticleSimulator> boundParticles = new List<ParticleSimulator>();

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
            collidingParticles.AddRange( others );
        }

        protected virtual bool CheckBind ()
        {
            population.reactor.reactionData.Shuffle();
            foreach (ParticleReaction reactionData in population.reactor.reactionData)
            {
                collidingParticles.Shuffle();
                foreach (ParticleSimulator other in collidingParticles)
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

        protected void DoBind (ParticleSimulator other, Reaction reaction)
        {
            boundParticles.Add( other );
            other.boundParticles.Add( this );

            Bind bind = population.reactor.model.GetBindForSpecies( agent.species, other.agent.species );

            bool thisIsChild = bind.childSpecies == agent.species;
            ParticleSimulator newChild = thisIsChild ? this : other;
            ParticleSimulator newParent = thisIsChild ? other : this;

            newChild.BindToOther( newParent, bind );
        }

        protected virtual void BindToOther (ParticleSimulator other, Bind bind)
        {
            ToggleMotion( false );
            agent.SetParent( other.agent );
            transform.position = other.transform.TransformPoint( bind.relativePosition );
            transform.rotation = other.transform.rotation * Quaternion.Euler( bind.relativeRotation );
        }

        protected abstract void ToggleMotion (bool move);

        protected virtual Vector3 GetExitDirection ()
        {
            int n = 0;
            Vector3 exitVector = Vector3.zero;
            foreach (ParticleSimulator other in collidingParticles)
            {
                if (!boundParticles.Contains( other ))
                {
                    exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                    n++;
                }
            }
            return exitVector.normalized;
        }
	}
}