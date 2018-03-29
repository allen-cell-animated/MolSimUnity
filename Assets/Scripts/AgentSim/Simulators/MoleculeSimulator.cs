using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeSimulator : MonoBehaviour 
    {
        public ParticleSimulator particleSimulator;
        public MoleculeState moleculeState;
        public Dictionary<string,BindingSiteSimulator> bindingSiteSimulators = new Dictionary<string,BindingSiteSimulator>();

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

        public void UpdateActive (bool aBindingSiteIsActive = false)
        {
            bool newActive = aBindingSiteIsActive ? true : GetActive();

            if (newActive != active)
            {
                active = newActive;
                particleSimulator.UpdateActive( active );
            }
        }

        bool GetActive ()
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

            foreach (BindingSiteSimulator bindingSiteSimulator in bindingSiteSimulators.Values)
            {
                bindingSiteSimulator.UpdateActive( false );
            }
            active = GetActive();
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
        }

        public virtual bool InteractWith (MoleculeSimulator other)
        {
            foreach (BindingSiteSimulator bindingSiteSimulator in bindingSiteSimulators.Values)
            {
                if (bindingSiteSimulator.active)
                {
                    foreach (BindingSiteSimulator otherBindingSiteSimulator in other.bindingSiteSimulators.Values)
                    {
                        if (otherBindingSiteSimulator.active && bindingSiteSimulator.ReactWith( otherBindingSiteSimulator ))
                        {
                            return true;
                        }
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

        public override string ToString()
        {
            return "MoleculeSimulator " + name;
        }
	}
}