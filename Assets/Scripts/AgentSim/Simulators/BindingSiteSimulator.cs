using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSiteSimulator : MonoBehaviour 
    {
        public BindingSitePopulation population;
        public MoleculeSimulator moleculeSimulator;
        public string state;
        public BindingSiteSimulator boundSite;

        Transform _theTransform;
        public Transform theTransform
        {
            get
            {
                if (_theTransform == null)
                {
                    _theTransform = transform;
                }
                return _theTransform;
            }
        }

        public bool active;

        public void UpdateActive (bool updateMolecule = true)
        {
            bool newActive = false;

            foreach (string activeState in population.activeStates)
            {
                if (state == activeState)
                {
                    newActive = true;
                    break;
                }
            }

            if (newActive != active)
            {
                active = newActive;
                if (updateMolecule)
                {
                    moleculeSimulator.UpdateActive( active );
                }
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

        public MoleculeSimulator[] complex
        {
            get
            {
                return particleSimulator.moleculeSimulators;
            }
        }

        public Molecule molecule
        {
            get
            {
                return moleculeSimulator.moleculeState.molecule;
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
            population.RegisterBindingSiteSimulator( this );
        }

        public virtual bool ReactWith (BindingSiteSimulator other)
        {
            if (IsNear( other ))
            {
                return population.DoBimolecularReaction( this, other );
            }
            return false;
        }

        bool IsNear (BindingSiteSimulator other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < population.interactionRadius + other.population.interactionRadius;
        }

        public void MoveToPopulation (ParticlePopulation particlePopulation)
        {
            population.UnregisterBindingSiteSimulator( this );
            population = particlePopulation.GetBindingSitePopulation( population.moleculeBindingSite );
            population.RegisterBindingSiteSimulator( this );
            name = moleculeSimulator.name + "_" + id;
            UpdateActive( false );
        }

        public override string ToString()
        {
            return "BindingSiteSimulator " + name;
        }
    }
}