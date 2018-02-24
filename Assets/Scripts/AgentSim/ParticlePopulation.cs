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

        //int[] _relevantReactionIndices;
        //public int[] relevantReactionIndices
        //{
        //    get
        //    {
        //        if (_relevantReactionIndices == null)
        //        {
        //            foreach (Reaction reaction in reactor.model.reactions)
        //            {
        //                foreach (MoleculeBindingSite bindingSite in reaction.relevantSites)
        //                {
        //                    if (molecule == bindingSite.molecule)
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //        return _relevantReactionIndices;
        //    }
        //}

        public virtual void Init (MoleculeConcentration moleculeConcentration, ParticleReactor _reactor)
        {
            molecule = moleculeConcentration.moleculeState.molecule;
            concentration = moleculeConcentration.concentration;
            reactor = _reactor;

            SpawnParticles( moleculeConcentration.moleculeState );
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

            ParticleSimulator simulator;
            if (reactor.usePhysicsEngine)
            {
                simulator = particle.AddComponent<PhysicalParticleSimulator>();
            }
            else
            {
                simulator = particle.AddComponent<ManagedParticleSimulator>();
            }
            simulator.Init( moleculeState, this );
        }
    }
}