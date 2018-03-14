using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class MoleculeSimulator : Simulator 
    {
        protected MoleculePopulation population;
        public bool canMove = true;

        protected List<MoleculeSimulator> collidingMolecules = new List<MoleculeSimulator>();
        protected Dictionary<string,BindingSiteSimulator> bindingSites = new Dictionary<string,BindingSiteSimulator>();
        protected List<BindingSiteSimulator> activeBindingSites = new List<BindingSiteSimulator>();

        public string species
        {
            get
            {
                return agent.species;
            }
        }

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

        ReactionWatcher[] reactionWatchers
        {
            get
            {
                return population.reactor.reactionWatchers;
            }
        }

        public virtual void Init (MoleculePopulation _population, MoleculeState moleculeState = null)
        {
            population = _population;
            if (moleculeState != null)
            {
                CreateBindingSites( moleculeState );
            }
        }

        protected virtual void CreateBindingSites (MoleculeState moleculeState)
        {
            foreach (BindingSite site in moleculeState.molecule.sites)
            {
                CreateBindingSite( moleculeState.molecule, site.id );
            }
        }

        public virtual void CreateBindingSite (Molecule molecule, string id)
        {
            BindingSitePopulation bindingSitePopulation = population.GetBindingSitePopulation( molecule, id );

            GameObject bindingSite = new GameObject();
            bindingSite.transform.SetParent( transform );
            bindingSitePopulation.bindingSite.transformOnMolecule.Apply( transform, bindingSite.transform );
            bindingSite.name = name + "_" + bindingSitePopulation.bindingSite.id;
            bindingSite.AddComponent<Agent>().Init( agent.species + "_" + bindingSitePopulation.bindingSite.id, 0.1f * agent.scale );

            BindingSiteSimulator simulator;
            if (population.reactor.usePhysicsEngine)
            {
                simulator = bindingSite.AddComponent<PhysicalBindingSiteSimulator>();
            }
            else
            {
                simulator = bindingSite.AddComponent<ManagedBindingSiteSimulator>();
            }
            simulator.Init( bindingSitePopulation, this );
            bindingSites.Add( id, simulator );
            if (simulator.active)
            {
                activeBindingSites.Add( simulator );
            }
        }

        protected float GetDisplacement (float dTime)
		{
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( population.diffusionCoefficient * dTime ) );
		}

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( transform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, population.reactor.container.boundaryLayer ))
            {
                transform.position = info.point - collisionToCenter.normalized;
            }
        }

        protected virtual void SaveCollidingSimulators (MoleculeSimulator[] others)
        {
            collidingMolecules.AddRange( others );
        }

        public virtual void InteractWith (MoleculeSimulator other)
        {
            for (int i = 0; i < activeBindingSites.Count; i++)
            {
                for (int j = 0; j < other.activeBindingSites.Count; j++)
                {
                    if (activeBindingSites[i].TryToReact( other.activeBindingSites[j] ))
                    {
                        return;
                    }
                }
            }
        }

        public virtual bool SiteIsInState (string siteID, string state)
        {
            return bindingSites[siteID].state == state;
        }

        public virtual bool IsBoundToOther (MoleculeSimulator molecule)
        {
            foreach (BindingSiteSimulator site in bindingSites.Values)
            {
                if (site.boundSite != null && site.boundSite.molecule == molecule)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual MoleculeSimulator[] GetBoundMoleculesSet ()
        {
            List<MoleculeSimulator> boundMolecules = new List<MoleculeSimulator>();
            boundMolecules.Add( this );
            foreach (BindingSiteSimulator site in bindingSites.Values)
            {
                if (site.boundSite != null && !boundMolecules.Contains( site.boundSite.molecule ))
                {
                    boundMolecules.Add( site.boundSite.molecule );
                }
            }
            return boundMolecules.ToArray();
        }

        public abstract void ToggleMotion (bool move);

        protected virtual Vector3 GetExitDirection ()
        {
            int n = 0;
            Vector3 exitVector = Vector3.zero;
            foreach (MoleculeSimulator other in collidingMolecules)
            {
                if (!IsBoundToOther( other ))
                {
                    exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                    n++;
                }
            }
            return exitVector.normalized;
        }
	}
}