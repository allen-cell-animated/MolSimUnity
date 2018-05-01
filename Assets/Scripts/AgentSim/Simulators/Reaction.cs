using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class CollisionFreeReaction : Reaction
	{
        [SerializeField] List<MoleculeComponent> components = new List<MoleculeComponent>();

        public CollisionFreeReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public bool RegisterComponent (MoleculeComponent component)
        {
            if (!components.Contains( component ))
            {
                if (definition.ComponentIsReactantInReactionCenter( component ))
                {
                    components.Add( component );
                    return true;
                }
            }
            else
            {
                Debug.LogWarning( "Trying to register " + component + " but it's already registered!" );
            }
            return false;
        }

        public void UnregisterComponent (MoleculeComponent component)
        {
            if (components.Contains( component ))
            {
                components.Remove( component );
            }
            else
            {
                Debug.LogWarning( "Trying to remove " + component + " but it's not registered!" );
            }
        }

        public virtual bool TryReact ()
        {
            if (components.Count > 0 && ShouldHappen())
            {
                components.Shuffle();
                return definition.React( reactor, components[0] );
            }
            return false;
        }
	}

    [System.Serializable]
    public class BimolecularReaction : Reaction
	{
        public BimolecularReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public virtual bool TryReactOnCollision (MoleculeComponent component1, MoleculeComponent component2)
        {
            if (definition.ReactantsEqual( component1.molecules, component2.molecules ) 
                && definition.BimolecularReactionCenterReactantsAreComponents( component1, component2 ) && ShouldHappen())
            {
                return definition.React( reactor, component1, component2 );
            }
            return false;
        }
	}

	// runtime data for a reaction used to keep rate near its theoretical value
    [System.Serializable]
    public abstract class Reaction
    {
        public Reactor reactor;
        public ReactionDef definition;
        
        [SerializeField] int attempts;
        public int events;
        [SerializeField] float observedRate;

        public Reaction (ReactionDef _reactionDef, Reactor _reactor)
        {
            reactor = _reactor;
            definition = _reactionDef;
        }

        public void CalculateObservedRate ()
        {
            observedRate = events / World.Instance.time;
        }

        bool observedRateTooHigh
        {
            get
            {
                return observedRate > 1.2f * definition.rate;
            }
        }

        bool observedRateTooLow
        {
            get
            {
                return observedRate < 0.8f * definition.rate;
            }
        }

        protected bool ShouldHappen ()
        {
            attempts++;

            bool react;
            if (observedRateTooHigh)
            {
                react = false;
            }
            else if (observedRateTooLow)
            {
                react = true;
            }
            else 
            {
                react = Random.value <= definition.rate * World.Instance.dT * (World.Instance.steps / attempts);
            }

            events = react ? events + 1 : events;

            return react;
        }

        public void Reset ()
        {
            events = attempts = 0;
            observedRate = 0;
        }

		public override string ToString()
		{
            return definition.description;
		}
	}
}