﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeSimulator : MonoBehaviour 
    {
        public ParticleSimulator particleSimulator;
        public MoleculeState moleculeState;
        public Dictionary<string,BindingSiteSimulator> bindingSiteSimulators = new Dictionary<string,BindingSiteSimulator>();
        protected List<BindingSiteSimulator> activeBindingSiteSimulators = new List<BindingSiteSimulator>();

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
                foreach (BindingSiteSimulator bindingSiteSimulator in bindingSiteSimulators.Values)
                {
                    if (bindingSiteSimulator.active)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public string species
        {
            get
            {
                return moleculeState.molecule.species;
            }
        }

        public virtual void Init (MoleculeState _moleculeState, ParticleSimulator _particleSimulator, ParticlePopulation population)
        {
            moleculeState = _moleculeState;
            particleSimulator = _particleSimulator;
            CreateBindingSites( population );
        }

        protected virtual void CreateBindingSites (ParticlePopulation population)
        {
            foreach (string siteID in moleculeState.molecule.bindingSites.Keys)
            {
                CreateBindingSite( moleculeState.molecule, siteID, population );
            }
        }

        protected virtual void CreateBindingSite (Molecule molecule, string id, ParticlePopulation population)
        {
            BindingSitePopulation bindingSitePopulation = population.GetBindingSitePopulation( new MoleculeBindingSite( molecule, id ) );

            GameObject bindingSiteObject = new GameObject();
            bindingSiteObject.transform.SetParent( theTransform );
            bindingSitePopulation.transformOnMolecule.Apply( theTransform, bindingSiteObject.transform );
            bindingSiteObject.name = name + "_" + bindingSitePopulation.moleculeBindingSite.bindingSiteID;

            BindingSiteSimulator bindingSiteSimulator = bindingSiteObject.AddComponent<BindingSiteSimulator>();
            bindingSiteSimulator.Init( bindingSitePopulation, this );

            bindingSiteSimulators.Add( id, bindingSiteSimulator );
            if (bindingSiteSimulator.active)
            {
                activeBindingSiteSimulators.Add( bindingSiteSimulator );
            }
        }

        public virtual bool InteractWith (MoleculeSimulator other)
        {
            foreach (BindingSiteSimulator bindingSiteSimulator in activeBindingSiteSimulators)
            {
                foreach (BindingSiteSimulator otherBindingSiteSimulator in other.activeBindingSiteSimulators)
                {
                    if (bindingSiteSimulator.ReactWith( otherBindingSiteSimulator ))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveToComplex (ParticleSimulator _particleSimulator)
        {
            theTransform.SetParent( _particleSimulator.gameObject == gameObject ? _particleSimulator.population.theTransform : _particleSimulator.theTransform );

            particleSimulator.RemoveMoleculeSimulator( this );
            particleSimulator = _particleSimulator;
            name = particleSimulator.name + "_" + species;

            foreach (BindingSiteSimulator bindingSiteSimulator in bindingSiteSimulators.Values)
            {
                bindingSiteSimulator.MoveToPopulation( particleSimulator.population );
            }
        }
	}
}