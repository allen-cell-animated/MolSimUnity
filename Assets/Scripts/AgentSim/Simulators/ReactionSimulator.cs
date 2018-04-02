using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class CollisionFreeReactionSimulator : ReactionSimulator
	{
        [SerializeField] List<BindingSitePopulation> populations = new List<BindingSitePopulation>();

        public CollisionFreeReactionSimulator (Reaction _reaction) : base (_reaction) { }

        public void RegisterBindingSitePopulation (BindingSitePopulation bindingSitePopulation, ComplexState complexState)
        {
            if (!populations.Contains( bindingSitePopulation ) && ComplexIsReactant( complexState ) 
                && bindingSitePopulation.moleculeBindingSite.Matches( reaction.relevantSites[0] ))
            {
                populations.Add( bindingSitePopulation );
            }
        }

        bool ComplexIsReactant (ComplexState complexState)
        {
            foreach (ComplexState reactant in reaction.reactantStates)
            {
                if (reactant.Equals( complexState ))
                {
                    return true;
                }
            }
            return false;
        }

        public void UnregisterBindingSitePopulation (BindingSitePopulation bindingSitePopulation)
        {
            if (populations.Contains( bindingSitePopulation ))
            {
                populations.Remove( bindingSitePopulation );
            }
        }

        public bool TryReact ()
        {
            if (populations.Count > 0 && shouldHappen)
            {
                populations.Shuffle();
                return populations[0].DoCollisionFreeReaction( reaction );
            }
            return false;
        }
	}

    [System.Serializable]
    public class BimolecularReactionSimulator : ReactionSimulator
	{
        public BimolecularReactionSimulator (Reaction _reaction) : base (_reaction) { }

        public bool TryReactOnCollision (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2)
        {
            if (ReactantsEqual( bindingSiteSimulator1.complex, bindingSiteSimulator2.complex ) && shouldHappen)
            {
                reaction.React( bindingSiteSimulator1, bindingSiteSimulator2 );
                return true;
            }
            return false;
        }

        bool ReactantsEqual (MoleculeSimulator[] complex1, MoleculeSimulator[] complex2)
        {
            return (reaction.reactantStates.Length == 0 && complex1 == null && complex2 == null)
                || (reaction.reactantStates.Length == 1 && ((reaction.reactantStates[0].IsSatisfiedBy( complex1 ) && complex2 == null)
                                                         || (reaction.reactantStates[0].IsSatisfiedBy( complex2 ) && complex1 == null)))
                || (reaction.reactantStates.Length == 2 && ((reaction.reactantStates[0].IsSatisfiedBy( complex1 ) && reaction.reactantStates[1].IsSatisfiedBy( complex2 )))
                                                         || (reaction.reactantStates[0].IsSatisfiedBy( complex2 ) && reaction.reactantStates[1].IsSatisfiedBy( complex1 )));
        }
	}

	// runtime data for a reaction used to keep rate near its theoretical value
    [System.Serializable]
    public abstract class ReactionSimulator
    {
        public Reaction reaction;
        
        public int attempts;
        public int events;
        public float observedRate;

        public ReactionSimulator (Reaction _reaction)
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