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
        [HideInInspector] public ReactionState[] reactionStates;

        ReactionWatcher[] reactionWatchers
        {
            get
            {
                return population.reactor.reactionWatchers;
            }
        }

        public void Init (ParticlePopulation _population)
        {
            population = _population;
            diffusionCoefficient = population.molecule.diffusionCoefficient;
            InitReactionStates( population.reactor.model.reactions.Length );
            DoAdditionalInit();
        }

        void InitReactionStates (int n)
        {
            reactionStates = new ReactionState[n];
            for (int i = 0; i < n; i++)
            {
                reactionStates[i] = new ReactionState( i, null );
            }
        }

        protected abstract void DoAdditionalInit ();

        protected float GetDisplacement (float dTime)
		{
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt(  diffusionCoefficient * dTime ) );
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
            reactionStates.Shuffle();
            for (int i = 0; i < reactionStates.Length; i++)
            {
                if (reactionStates[i].currentBindingPartner == null)
                {
                    collidingParticles.Shuffle();
                    foreach (ParticleSimulator other in collidingParticles)
                    {
                        if (other.reactionStates[i].currentBindingPartner == null 
                            && reactionWatchers[i].reaction.ReactantsEqual( agent.species, other.agent.species )
                            && reactionWatchers[i].ShouldHappen())
                        {
                            ApplyBind( i, other );
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected void ApplyBind (int reactionIndex, ParticleSimulator other)
        {
            reactionStates[reactionIndex].currentBindingPartner = other;
            other.reactionStates[reactionIndex].currentBindingPartner = this;

            Bind bind = reactionWatchers[reactionIndex].reaction.bind;
            ParticleSimulator child, parent;
            GetChildAndParent( bind, this, other, out child, out parent );

            child.BindTo( parent, bind );
        }

        protected void BindTo (ParticleSimulator other, Bind bind)
        {
            ToggleMotion( false );
            agent.SetParent( other.agent );
            transform.position = other.transform.TransformPoint( bind.relativePosition );
            transform.rotation = other.transform.rotation * Quaternion.Euler( bind.relativeRotation );
        }

        public void GetChildAndParent (Bind bind, ParticleSimulator particle1, ParticleSimulator particle2, out ParticleSimulator child, out ParticleSimulator parent)
        {
            bool childIs1 = bind.childSpecies == particle1.agent.species;
            child = childIs1 ? particle1 : particle2;
            parent = childIs1 ? particle2 : particle1;
        }

        protected abstract void ToggleMotion (bool move);

        protected virtual Vector3 GetExitDirection ()
        {
            int n = 0;
            Vector3 exitVector = Vector3.zero;
            foreach (ParticleSimulator other in collidingParticles)
            {
                if (!IsBoundToOther( other ))
                {
                    exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                    n++;
                }
            }
            return exitVector.normalized;
        }

        public bool IsBoundToOther (ParticleSimulator other)
        {
            foreach (ReactionState reactionState in reactionStates)
            {
                if (reactionState.currentBindingPartner == other)
                {
                    return true;
                }
            }
            return false;
        }
	}

    [System.Serializable]
    public class ReactionState
    {
        public int index;
        public ParticleSimulator currentBindingPartner;

        public ReactionState (int _index, ParticleSimulator _currentBindingPartner)
        {
            index = _index;
            currentBindingPartner = _currentBindingPartner;
        }
    }
}