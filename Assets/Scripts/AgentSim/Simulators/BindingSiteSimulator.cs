using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class BindingSiteSimulator : MonoBehaviour 
    {
        protected BindingSitePopulation population;
        public MoleculeSimulator molecule;
        public string state;
        public BindingSiteSimulator boundSite;

        public bool active
        {
            get
            {
                return population.StateIsActive( state );
            }
        }

        public MoleculeReactor reactor
        {
            get
            {
                return population.moleculePopulation.reactor;
            }
        }

        public string id
        {
            get
            {
                return population.bindingSite.id;
            }
        }

        public virtual void Init (BindingSitePopulation _population, MoleculeSimulator _molecule)
        {
            population = _population;
            molecule = _molecule;
            state = population.initialState;
        }

        public virtual bool TryToReact (BindingSiteSimulator other)
        {
            Reaction reaction = population.GetNextReaction( this, other );
            if (reaction != null)
            {
                boundSite = other;
                other.boundSite = this;

                reaction.React( this, other );

                return true;
            }
            return false;
        }
    }
}