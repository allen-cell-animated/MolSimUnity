using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSiteSimulator : MonoBehaviour 
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

        public Reactor reactor
        {
            get
            {
                return population.particlePopulation.reactor;
            }
        }

        public string id
        {
            get
            {
                return population.bindingSite.id;
            }
        }

        public List<MoleculeSimulator> complex
        {
            get
            {
                return molecule.particle.molecules;
            }
        }

        public virtual void Init (BindingSitePopulation _population, MoleculeSimulator _molecule)
        {
            population = _population;
            molecule = _molecule;
            state = population.initialState;
            population.RegisterBindingSite( this );
        }

        public virtual bool ReactWith (BindingSiteSimulator other)
        {
            if (IsNear( other ))
            {
                Reaction reaction = population.GetNextBimolecularReaction( this, other );
                if (reaction != null)
                {
                    reaction.React( this, other );
                    return true;
                }
            }
            return false;
        }

        bool IsNear (BindingSiteSimulator other)
        {
            return other != this 
                && Vector3.Distance( transform.position, other.transform.position ) < population.interactionRadius + other.population.interactionRadius;
        }
    }
}