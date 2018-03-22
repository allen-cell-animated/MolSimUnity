using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSiteSimulator : MonoBehaviour 
    {
        [SerializeField] protected BindingSitePopulation population;
        public MoleculeSimulator moleculeSimulator;
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

        public ParticleSimulator particleSimulator
        {
            get
            {
                return moleculeSimulator.particleSimulator;
            }
        }

        public List<MoleculeSimulator> complex
        {
            get
            {
                return particleSimulator.moleculeSimulators;
            }
        }

        public string species
        {
            get
            {
                return moleculeSimulator.species;
            }
        }

        public string id
        {
            get
            {
                return population.id;
            }
        }

        public virtual void Init (BindingSitePopulation _population, MoleculeSimulator _moleculeSimulator)
        {
            population = _population;
            moleculeSimulator = _moleculeSimulator;
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
            Debug.Log( other == null );
            return other != this 
                && Vector3.Distance( transform.position, other.transform.position ) < population.interactionRadius + other.population.interactionRadius;
        }

        public void MoveToPopulation (ParticlePopulation particlePopulation)
        {
            population.UnregisterBindingSite( this );
            population = particlePopulation.GetBindingSitePopulation( moleculeSimulator.moleculeState.molecule, id );
            population.RegisterBindingSite( this );
            name = moleculeSimulator.name + "_" + id;
        }
    }
}