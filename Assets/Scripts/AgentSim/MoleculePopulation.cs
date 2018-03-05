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

            InitBindingSitePopulations();
            SpawnParticles( moleculeConcentration.moleculeState );
        }

        protected virtual void InitBindingSitePopulations ()
        {
            //foreach (
        }

        protected virtual void SpawnParticles (MoleculeState moleculeState)
        {
            if (molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s particle prefab is null!" );
                return;
            }

            moleculeState.Init(); //for prototyping in inspector without writing custom property drawer etc

            for (int i = 0; i < amount; i++)
            {
                SpawnParticle( moleculeState, i );
            }
        }

        public virtual void SpawnParticle (MoleculeState moleculeState, int index)
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