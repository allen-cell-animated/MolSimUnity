﻿using System.Collections;
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

        public bool active
        {
            get
            {
                foreach (string activeState in population.bindingSite.activeStates)
                {
                    if (state == activeState)
                    {
                        return true;
                    }
                }
                return false;
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
            population.RegisterBindingSiteSimulator( this );
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
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < population.interactionRadius + other.population.interactionRadius;
        }

        public void MoveToPopulation (ParticlePopulation particlePopulation)
        {
            population.UnregisterBindingSiteSimulator( this );
            population = particlePopulation.GetBindingSitePopulation( population.moleculeBindingSite );
            population.RegisterBindingSiteSimulator( this );
            name = moleculeSimulator.name + "_" + id;
        }
    }
}