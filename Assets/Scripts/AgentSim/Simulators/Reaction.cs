using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class CollisionFreeReaction : Reaction
	{
        [SerializeField] List<BindingSite> bindingSites = new List<BindingSite>();

        public CollisionFreeReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public bool RegisterBindingSite (BindingSite bindingSite)
        {
            if (!bindingSites.Contains( bindingSite ))
            {
                if (SiteIsRelevant( bindingSite ))
                {
                    bindingSites.Add( bindingSite );
                    return true;
                }
            }
            else
            {
                Debug.LogWarning( "Trying to register " + bindingSite + " but it's already registered!" );
            }
            return false;
        }

        public void UnregisterBindingSite (BindingSite bindingSite)
        {
            if (bindingSites.Contains( bindingSite ))
            {
                bindingSites.Remove( bindingSite );
            }
            else
            {
                Debug.LogWarning( "Trying to remove " + bindingSite + " but it's not registered!" );
            }
        }

        public virtual bool TryReact ()
        {
            if (bindingSites.Count > 0 && ShouldHappen())
            {
                bindingSites.Shuffle();
                reactionDef.React( reactor, bindingSites[0] );
                return true;
            }
            return false;
        }
	}

    [System.Serializable]
    public class BimolecularReaction : Reaction
	{
        public BimolecularReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public virtual bool TryReactOnCollision (BindingSite bindingSite1, BindingSite bindingSite2)
        {
            if (ReactantsEqual( bindingSite1.molecules, bindingSite2.molecules ) 
                && BothSitesAreRelevant( bindingSite1, bindingSite2 ) && ShouldHappen())
            {
                reactionDef.React( reactor, bindingSite1, bindingSite2 );
                return true;
            }
            return false;
        }

        bool ReactantsEqual (Molecule[] molecules1, Molecule[] molecules2)
        {
            return ((reactionDef.reactantSnapshots[0].IsSatisfiedBy( molecules1 ) && reactionDef.reactantSnapshots[1].IsSatisfiedBy( molecules2 )))
                 || (reactionDef.reactantSnapshots[0].IsSatisfiedBy( molecules2 ) && reactionDef.reactantSnapshots[1].IsSatisfiedBy( molecules1 ));
            
        }

        bool BothSitesAreRelevant (BindingSite bindingSite1, BindingSite bindingSite2)
        {
            return (reactionDef.relevantSites[0].IsSatisfiedBy( bindingSite1 ) && reactionDef.relevantSites[1].IsSatisfiedBy( bindingSite2 ))
                || (reactionDef.relevantSites[0].IsSatisfiedBy( bindingSite2 ) && reactionDef.relevantSites[1].IsSatisfiedBy( bindingSite1 ));
        }
	}

	// runtime data for a reaction used to keep rate near its theoretical value
    [System.Serializable]
    public abstract class Reaction
    {
        public Reactor reactor;
        public ReactionDef reactionDef;
        
        [SerializeField] int attempts;
        public int events;
        [SerializeField] float observedRate;

        public Reaction (ReactionDef _reactionDef, Reactor _reactor)
        {
            reactor = _reactor;
            reactionDef = _reactionDef;
        }

        public void CalculateObservedRate ()
        {
            observedRate = events / World.Instance.time;
        }

        public bool ComplexIsReactant (ComplexSnapshot complexSnapshot)
        {
            foreach (ComplexSnapshot reactantSnapshot in reactionDef.reactantSnapshots)
            {
                if (reactantSnapshot.IsSatisfiedBy( complexSnapshot ))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ComplexIsReactant (Molecule[] molecules)
        {
            foreach (ComplexSnapshot reactantSnapshot in reactionDef.reactantSnapshots)
            {
                if (reactantSnapshot.IsSatisfiedBy( molecules ))
                {
                    return true;
                }
            }
            return false;
        }

        public bool SiteIsRelevant (BindingSite bindingSite)
        {
            foreach (MoleculeBindingSite site in reactionDef.relevantSites)
            {
                if (site.IsSatisfiedBy( bindingSite ))
                {
                    return true;
                }
            }
            return false;
        }

        bool observedRateTooHigh
        {
            get
            {
                return observedRate > 1.2f * reactionDef.rate;
            }
        }

        bool observedRateTooLow
        {
            get
            {
                return observedRate < 0.8f * reactionDef.rate;
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
                react = Random.value <= reactionDef.rate * World.Instance.dT * (World.Instance.steps / attempts);
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
            return reactionDef.description;
		}
	}
}