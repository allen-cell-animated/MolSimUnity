using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class BindingSiteSimulator : Simulator 
    {
        protected BindingSitePopulation population;
        public MoleculeSimulator molecule;
        public string state;

        public bool active
        {
            get
            {
                return population.StateIsActive( state );
            }
        }

        public virtual void Init (BindingSitePopulation _population, MoleculeSimulator _molecule)
        {
            population = _population;
            molecule = _molecule;
            state = population.initialState;
        }

        public virtual bool CanBindToOther (BindingSiteSimulator other)
        {
            foreach (ReactionWatcher reactionWatcher in population.reactionWatchers)
            {
                if (!reactionWatcher.observedRateTooHigh && reactionWatcher.ReactantsEqual( molecule, other.molecule ))
                {
                    //TODO
                }
            }
            return false;
        }
    }
}