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

        protected BindingSitePopulation[] bindingSitePopulations;

        int amount
        {
            get
            {
                return Mathf.RoundToInt( concentration * reactor.container.volume * 6.022141e23f );
            }
        }

        public virtual void Init (MoleculeConcentration moleculeConcentration, MoleculeReactor _reactor)
        {
            molecule = moleculeConcentration.moleculeState.molecule;
            concentration = moleculeConcentration.concentration;
            reactor = _reactor;
            collisionRadius = interactionRadius = molecule.radius;

            InitBindingSitePopulations();
            SpawnMolecules( moleculeConcentration.moleculeState );
        }

        protected virtual void InitBindingSitePopulations ()
        {
            bindingSitePopulations = new BindingSitePopulation[molecule.sites.Length];
            for (int i = 0; i < molecule.sites.Length; i++)
            {
                bindingSitePopulations[i] = gameObject.AddComponent<BindingSitePopulation>();
                bindingSitePopulations[i].Init( molecule.sites[i] );
            }
        }

        public BindingSitePopulation GetBindingSitePopulationByID (string bindingSiteID)
        {
            foreach (BindingSitePopulation population in bindingSitePopulations)
            {
                if (population.bindingSite.id == bindingSiteID)
                {
                    return population;
                }
            }
            return null;
        }

        protected virtual void SpawnMolecules (MoleculeState moleculeState)
        {
            if (molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s particle prefab is null!" );
                return;
            }

            moleculeState.Init(); //for prototyping in inspector without writing custom property drawer etc

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