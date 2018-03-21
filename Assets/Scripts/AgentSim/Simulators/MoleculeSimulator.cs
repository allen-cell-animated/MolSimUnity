using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeSimulator : MonoBehaviour 
    {
        public string species;

        public ParticleSimulator particle;
        public Dictionary<string,BindingSiteSimulator> bindingSites = new Dictionary<string,BindingSiteSimulator>();
        protected List<BindingSiteSimulator> activeBindingSites = new List<BindingSiteSimulator>();

        public bool active
        {
            get
            {
                foreach (KeyValuePair<string,BindingSiteSimulator> bindingSite in bindingSites)
                {
                    if (bindingSite.Value.active)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public virtual void Init (MoleculeState moleculeState, ParticleSimulator _particle)
        {
            species = moleculeState.species;
            particle = _particle;
            CreateBindingSites( moleculeState );
        }

        protected virtual void CreateBindingSites (MoleculeState moleculeState)
        {
            foreach (BindingSite site in moleculeState.molecule.sites)
            {
                CreateBindingSite( moleculeState.molecule, site.id );
            }
        }

        protected virtual void CreateBindingSite (Molecule molecule, string id)
        {
            BindingSitePopulation bindingSitePopulation = GetComponentInParent<ParticlePopulation>().GetBindingSitePopulation( molecule.species, id );

            GameObject obj = new GameObject();
            obj.transform.SetParent( transform );
            bindingSitePopulation.bindingSite.transformOnMolecule.Apply( transform, obj.transform );
            obj.name = name + "_" + bindingSitePopulation.bindingSite.id;

            BindingSiteSimulator bindingSite = obj.AddComponent<BindingSiteSimulator>();
            bindingSite.Init( bindingSitePopulation, this );

            bindingSites.Add( id, bindingSite );
            if (bindingSite.active)
            {
                activeBindingSites.Add( bindingSite );
            }
        }

        public virtual bool InteractWith (MoleculeSimulator other)
        {
            foreach (BindingSiteSimulator bindingSite in activeBindingSites)
            {
                foreach (BindingSiteSimulator otherBindingSite in other.activeBindingSites)
                {
                    if (bindingSite.ReactWith( otherBindingSite ))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool SiteIsInState (string siteID, string state)
        {
            return bindingSites[siteID].state == state;
        }

        public void MoveToComplex (ParticleSimulator _particle)
        {
            if (_particle.gameObject == gameObject)
            {
                transform.SetParent( _particle.population.transform );
            }
            else
            {
                transform.SetParent( _particle.transform );
            }

            particle.RemoveMolecule( this );
            particle = _particle;
            name = particle.name + "_" + species;

            foreach (BindingSiteSimulator bindingSite in bindingSites.Values)
            {
                bindingSite.MoveToPopulation( particle.population );
            }
        }
	}
}