using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class MoleculeSimulator : Simulator 
    {
        protected Molecule molecule;
        protected MoleculePopulation population;
		protected float diffusionCoefficient;
        public bool canMove = true;

        protected List<MoleculeSimulator> collidingMolecules = new List<MoleculeSimulator>();
        protected Dictionary<string,BindingSiteSimulator> bindingSites = new Dictionary<string,BindingSiteSimulator>();
        protected List<BindingSiteSimulator> activeBindingSites = new List<BindingSiteSimulator>();

        public string species
        {
            get
            {
                return molecule.species;
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

        public virtual void Init (MoleculeState moleculeState, MoleculePopulation _population)
        {
            molecule = moleculeState.molecule;
            population = _population;
            diffusionCoefficient = molecule.diffusionCoefficient;
            CreateBindingSites( moleculeState );
        }

        protected virtual void CreateBindingSites (MoleculeState moleculeState)
        {
            foreach (BindingSite site in moleculeState.molecule.sites)
            {
                CreateBindingSite( site.id );
            }
        }

        public virtual void CreateBindingSite (string id)
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
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * dTime ) );
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

        protected virtual bool CheckBind ()
        {
            //reactionStates.Shuffle();
            //for (int i = 0; i < reactionStates.Length; i++)
            //{
            //    if (reactionStates[i].currentBindingPartner == null)
            //    {
            //        collidingParticles.Shuffle();
            //        foreach (ParticleSimulator other in collidingParticles)
            //        {
            //            if (other.reactionStates[i].currentBindingPartner == null 
            //                //&& reactionWatchers[i].reaction.ReactantsEqual( agent.species, other.agent.species )
            //                && reactionWatchers[i].ShouldHappen())
            //            {
            //                //ApplyBind( i, other );
            //                return true;
            //            }
            //        }
            //    }
            //}
            return false;
        }

        //protected void ApplyBind (int reactionIndex, ParticleSimulator other)
        //{
        //    reactionStates[reactionIndex].currentBindingPartner = other;
        //    other.reactionStates[reactionIndex].currentBindingPartner = this;

        //    Bind bind = reactionWatchers[reactionIndex].reaction.bind;
        //    ParticleSimulator child, parent;
        //    GetChildAndParent( bind, this, other, out child, out parent );

        //    child.BindTo( parent, bind );
        //}

        //public void GetChildAndParent (Bind bind, ParticleSimulator particle1, ParticleSimulator particle2, out ParticleSimulator child, out ParticleSimulator parent)
        //{
        //    bool childIs1 = bind.childSpecies == particle1.agent.species;
        //    child = childIs1 ? particle1 : particle2;
        //    parent = childIs1 ? particle2 : particle1;
        //}

        //protected void BindTo (ParticleSimulator other, Bind bind)
        //{
        //    ToggleMotion( false );
        //    agent.SetParent( other.agent );
        //    transform.position = other.transform.TransformPoint( bind.relativePosition );
        //    transform.rotation = other.transform.rotation * Quaternion.Euler( bind.relativeRotation );
        //}

        protected abstract void ToggleMotion (bool move);

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

        public bool IsBoundToOther (MoleculeSimulator other)
        {
            //foreach (ReactionState reactionState in reactionStates)
            //{
            //    if (reactionState.currentBindingPartner == other)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        public void CalculateInteractionRadius ()
        {
            //float r = 0, max = 0;
            //foreach (BindingSiteState siteState in bindingSiteStates)
            //{
            //    if (siteState.active)
            //    {
            //        r = siteState.maxExtentFromMoleculeCenter;
            //        if (r > max)
            //        {
            //            max = r;
            //        }
            //    }
            //}
            //interactionRadius = max;
        }
	}
}