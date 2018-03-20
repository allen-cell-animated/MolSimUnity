using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
	public class CollisionFreeReactionWatcher : ReactionWatcher
	{
        List<BindingSitePopulation> populations = new List<BindingSitePopulation>();

		public CollisionFreeReactionWatcher (Reaction _reaction) : base (_reaction) { }

        public void RegisterBindingSitePopulation (BindingSitePopulation population)
        {
            if (!populations.Contains( population ) && reaction.relevantSites[0].molecule.species == population.molecule.species 
                && reaction.relevantSites[0].bindingSiteID == population.bindingSite.id)
            {
                populations.Add( population );
            }
        }

        public bool TryReact ()
        {
            if (populations.Count > 0 && shouldHappen)
            {
                populations.Shuffle();
                populations[0].DoCollisionFreeReaction( reaction );
            }
            return false;
        }
	}

    [System.Serializable]
	public class BimolecularReactionWatcher : ReactionWatcher
	{
		public BimolecularReactionWatcher (Reaction _reaction) : base (_reaction) { }

        public bool TryReactOnCollision (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2)
        {
            if (ReactantsEqual( bindingSite1.complex, bindingSite2.complex ))
            {
                return shouldHappen;
            }
            return false;
        }

        bool ReactantsEqual (List<MoleculeSimulator> complex1, List<MoleculeSimulator> complex2)
        {
            return (reaction.reactantStates.Length == 0 && complex1 == null && complex2 == null)
                || (reaction.reactantStates.Length == 1 && ((reaction.reactantStates[0].Matches( complex1 ) && complex2 == null)
                                                         || (reaction.reactantStates[0].Matches( complex2 ) && complex1 == null)))
                || (reaction.reactantStates.Length == 2 && ((reaction.reactantStates[0].Matches( complex1 ) && reaction.reactantStates[1].Matches( complex2 )))
                                                         || (reaction.reactantStates[0].Matches( complex2 ) && reaction.reactantStates[1].Matches( complex1 )));
        }
	}

	// runtime data for a reaction used to keep rate near its theoretical value
    [System.Serializable]
    public abstract class ReactionWatcher
    {
        public Reaction reaction;
        
        public int attempts;
        public int events;
        public float observedRate;

        public ReactionWatcher (Reaction _reaction)
        {
            reaction = _reaction;
        }

        public void CalculateObservedRate ()
        {
            observedRate = events / World.Instance.time;
        }

        bool observedRateTooHigh
        {
            get
            {
                return observedRate > 1.2f * reaction.rate;
            }
        }

        bool observedRateTooLow
        {
            get
            {
                return observedRate < 0.8f * reaction.rate;
            }
        }

        protected bool shouldHappen
        {
            get
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
                    react = Random.value <= reaction.rate * World.Instance.dT * (World.Instance.steps / attempts);
                }

                events = react ? events + 1 : events;

                return react;
            }
        }

        public void Reset ()
        {
            events = attempts = 0;
            observedRate = 0;
        }
    }
}