using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculePopulation : AgentComponent 
    {
        public MoleculeReactor reactor;
        public Molecule molecule;
        [Tooltip( "M" )]
        public float concentration;
        public float collisionRadius;
        public float interactionRadius;
        public Dictionary<string,BindingSitePopulation> bindingSitePopulations;

        int amount
        {
            get
            {
                return Mathf.RoundToInt( concentration * reactor.container.volume * 6.022141e23f );
            }
        }

        public virtual void Init (MoleculeConcentration moleculeConcentration, MoleculeReactor _reactor)
        {
            moleculeConcentration.moleculeState.Init(); //for prototyping in inspector without writing custom property drawer etc

            molecule = moleculeConcentration.moleculeState.molecule;
            concentration = moleculeConcentration.concentration;
            reactor = _reactor;
            collisionRadius = interactionRadius = molecule.radius;

            CreateBindingSitePopulations( moleculeConcentration.moleculeState );
            SpawnMolecules( moleculeConcentration.moleculeState );
        }

        protected virtual void CreateBindingSitePopulations (MoleculeState moleculeState)
        {
            bindingSitePopulations = new Dictionary<string,BindingSitePopulation>();
            string siteID;
            string initialState = "";
            BindingSitePopulation sitePopulation;
            for (int i = 0; i < molecule.sites.Length; i++)
            {
                siteID = molecule.sites[i].id;
                if (moleculeState.bindingSiteStates.ContainsKey( siteID ))
                {
                    initialState = moleculeState.bindingSiteStates[siteID];
                }

                sitePopulation = gameObject.AddComponent<BindingSitePopulation>();
                sitePopulation.Init( molecule.sites[i], initialState );
                bindingSitePopulations.Add( siteID, sitePopulation );
            }
        }

        protected virtual void SpawnMolecules (MoleculeState moleculeState)
        {
            if (molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s particle prefab is null!" );
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                SpawnMolecule( moleculeState, i );
            }
        }

        public virtual void SpawnMolecule (MoleculeState moleculeState, int index)
        {
            if (molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s particle prefab is null!" );
                return;
            }

            GameObject particle = Instantiate( molecule.visualizationPrefab, transform );
            particle.transform.position = reactor.container.GetRandomPointInBounds( 0.1f );
            particle.transform.rotation = Random.rotation;
            particle.name = molecule.species + "_" + index;
            particle.AddComponent<Agent>().Init( molecule.species, molecule.scale );

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
    }
}