using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public abstract class Reaction
    {
        [HideInInspector] public Reactor reactor;
        public ReactionDef definition;

        [SerializeField] int attempts;
        public int events;
        [SerializeField] float observedRate;

        public Reaction (ReactionDef _reactionDef, Reactor _reactor)
        {
            reactor = _reactor;
            definition = _reactionDef;

            foreach (ReactionCenter reactionCenter in definition.reactionCenters)
            {
                reactionCenter.reaction = this;
            }
        }

        public bool ComplexIsReactant (ComplexPattern complexPattern)
        {
            foreach (ReactionCenter reactionCenter in definition.reactionCenters)
            {
                if (reactionCenter.reactantComplex.Matches( complexPattern ))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ComplexIsReactant (Dictionary<string,List<Molecule>> molecules)
        {
            foreach (ReactionCenter reactionCenter in definition.reactionCenters)
            {
                if (reactionCenter.reactantComplex.Matches( molecules ))
                {
                    return true;
                }
            }
            return false;
        }

        public ReactionCenter GetReactionCenterForComponent (MoleculeComponent component)
        {
            foreach (ReactionCenter reactionCenter in definition.reactionCenters)
            {
                if (reactionCenter.reactantComplex.Matches( component.complex.molecules ) && 
                    reactionCenter.reactantMolecule.Matches( component.molecule ) && 
                    reactionCenter.reactantComponent.Matches( component ))
                {
                    return reactionCenter;
                }
            }
            return null;
        }

        public abstract bool React (MoleculeComponent[] components, ReactionCenter[] matchingReactionCenters);

        protected void SetProductColor (Dictionary<string,List<Molecule>> molecules)
        {
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.SetColorForCurrentState();
                }
            }
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

        public bool ShouldHappen ()
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
            observedRate = events / World.Instance.time;

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