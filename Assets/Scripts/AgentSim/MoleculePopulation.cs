using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculePopulation : AgentComponent 
    {
        public MoleculeReactor reactor;
        public Molecule[] molecules;
        [Tooltip( "M" )]
        public float concentration;
        public float collisionRadius;
        public float interactionRadius;
        public Dictionary<MoleculeBindingSite,BindingSitePopulation> bindingSitePopulations;

        int amount
        {
            get
            {
                return Mathf.RoundToInt( concentration * reactor.container.volume * 6.022141e23f );
            }
        }

        public virtual void Init (MoleculeConcentration moleculeConcentration, MoleculeReactor _reactor)
        {
            molecules = new Molecule[moleculeConcentration.moleculeCount];
            for (int i = 0; i < moleculeConcentration.moleculeCount; i++)
            {
                molecules[i] = moleculeConcentration.moleculeStateSet.moleculeStates[i].molecule;
            }
            concentration = moleculeConcentration.concentration;
            reactor = _reactor;
            collisionRadius = interactionRadius = moleculeConcentration.radius;

            CreateBindingSitePopulations( moleculeConcentration.moleculeStateSet.moleculeStates );

            if (moleculeConcentration.moleculeCount == 1)
            {
                SpawnMolecules( moleculeConcentration.moleculeStateSet.moleculeStates[0] );
            }
            else if (moleculeConcentration.moleculeCount > 1)
            {
                // TODO spawn complexes of molecules
            }
        }

        protected virtual void CreateBindingSitePopulations (MoleculeState[] moleculeStates)
        {
            bindingSitePopulations = new Dictionary<MoleculeBindingSite,BindingSitePopulation>();
            string initialState = "";
            BindingSitePopulation sitePopulation;
            foreach (Molecule molecule in molecules)
            {
                foreach (BindingSite site in molecule.sites)
                {
                    foreach (MoleculeState moleculeState in moleculeStates)
                    {
                        if (moleculeState.molecule == molecule && moleculeState.bindingSiteStates.ContainsKey( site.id ))
                        {
                            initialState = moleculeState.bindingSiteStates[site.id];
                        }
                    }

                    sitePopulation = gameObject.AddComponent<BindingSitePopulation>();
                    sitePopulation.Init( molecule, site, initialState, this );
                    bindingSitePopulations.Add( new MoleculeBindingSite( molecule, site.id ), sitePopulation );
                }
            }
        }

        protected virtual void SpawnMolecules (MoleculeState moleculeState)
        {
            if (moleculeState.molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s molecule prefab is null!" );
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                SpawnMolecule( moleculeState, i );
            }
        }

        public virtual void SpawnMolecule (MoleculeState moleculeState, int index)
        {
            if (moleculeState.molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s molecule prefab is null!" );
                return;
            }

            GameObject particle = Instantiate( moleculeState.molecule.visualizationPrefab, transform );
            particle.transform.position = reactor.container.GetRandomPointInBounds( 0.1f );
            particle.transform.rotation = Random.rotation;
            particle.name = molecules[0].species + "_" + index;
            particle.AddComponent<Agent>().Init( moleculeState.molecule.species, moleculeState.molecule.scale );

            MoleculeSimulator simulator;
            if (reactor.usePhysicsEngine)
            {
                simulator = particle.AddComponent<PhysicalMoleculeSimulator>();
            }
            else
            {
                simulator = particle.AddComponent<ManagedMoleculeSimulator>();
            }
            simulator.Init( moleculeState, this );
        }

        public virtual BindingSitePopulation GetBindingSitePopulation (Molecule _molecule, string _bindingSiteID)
        {
            foreach (MoleculeBindingSite site in bindingSitePopulations.Keys)
            {
                if (site.molecule == _molecule && site.bindingSiteID == _bindingSiteID)
                {
                    return bindingSitePopulations[site];
                }
            }
            return null;
        }
    }
}