using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticlePopulation : AgentComponent 
    {
        public ParticleReactor reactor;
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

        public virtual void Init (Molecule _molecule, ParticleReactor _reactor)
        {
            molecule = _molecule;
            concentration = molecule.initialConcentration;
            reactor = _reactor;

            SpawnParticles();
        }

        protected virtual void SpawnParticles ()
        {
            if (molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s particle prefab is null!" );
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                SpawnParticle();
            }
        }

        public virtual void SpawnParticle ()
        {
            if (molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s particle prefab is null!" );
                return;
            }

            GameObject particle = Instantiate( molecule.visualizationPrefab, transform );
            particle.transform.position = reactor.container.GetRandomPointInBounds( 0.1f );
            particle.transform.rotation = Random.rotation;
            particle.AddComponent<Agent>().Init( molecule.species, molecule.scale );

            ParticleSimulator simulator;
            if (reactor.usePhysicsEngine)
            {
                simulator = particle.AddComponent<PhysicalParticleSimulator>();
            }
            else
            {
                simulator = particle.AddComponent<ManagedParticleSimulator>();
            }
            simulator.Init( this );
        }
    }
}