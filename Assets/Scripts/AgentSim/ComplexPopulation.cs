using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ComplexPopulation : MonoBehaviour 
    {
        public Reactor reactor;
        public Molecule[] molecules;
        public string species;
        [Tooltip( "M" )]
        public float concentration;
        public float diffusionCoefficient;
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

        public virtual void Init (ComplexConcentration complexConcentration, Reactor _reactor)
        {
            molecules = new Molecule[complexConcentration.moleculeCount];
            for (int i = 0; i < complexConcentration.moleculeCount; i++)
            {
                molecules[i] = complexConcentration.complexState.moleculeStates[i].molecule;
            }
            species = complexConcentration.species;
            concentration = complexConcentration.concentration;
            reactor = _reactor;
            diffusionCoefficient = complexConcentration.complexState.diffusionCoefficient;
            collisionRadius = interactionRadius = complexConcentration.complexState.radius;

            CreateBindingSitePopulations( complexConcentration.complexState.moleculeStates );

            if (amount > 0)
            {
                if (complexConcentration.moleculeCount == 1)
                {
                    SpawnMolecules( complexConcentration.complexState.moleculeStates[0] );
                }
                else if (complexConcentration.moleculeCount > 1)
                {
                    SpawnMoleculeComplexes( complexConcentration.complexState.moleculeStates );
                }
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

        protected virtual void SpawnMoleculeComplexes (MoleculeState[] moleculeStates)
        {
            Debug.LogWarning( "Can't spawn complexes of molecules yet!" );
            // TODO spawn complexes of molecules
        }

        protected virtual void SpawnMoleculeComplex (MoleculeState[] moleculeStates)
        {
            
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

            GameObject particle = Instantiate( moleculeState.molecule.visualizationPrefab );
            particle.transform.SetParent( transform );
            particle.transform.position = reactor.container.GetRandomPointInBounds( 0.1f );
            particle.transform.rotation = Random.rotation;
            particle.name = molecules[0].species + "_" + index;

            MoleculeSimulator simulator;
            if (reactor.usePhysicsEngine)
            {
                simulator = particle.AddComponent<PhysicalMoleculeSimulator>();
            }
            else
            {
                simulator = particle.AddComponent<ManagedMoleculeSimulator>();
            }
            simulator.Init( this, moleculeState );
        }

        public virtual void CreateComplexFromMolecules (Transform centerTransform, MoleculeSimulator[] _molecules)
        {
            GameObject particle = new GameObject( species + "_" + transform.childCount );
            particle.transform.SetParent( transform );
            particle.transform.position = centerTransform.position;
            particle.transform.rotation = centerTransform.rotation;

            MoleculeSimulator simulator;
            if (reactor.usePhysicsEngine)
            {
                simulator = particle.AddComponent<PhysicalMoleculeSimulator>();
            }
            else
            {
                simulator = particle.AddComponent<ManagedMoleculeSimulator>();
            }
            simulator.Init( this );

            foreach (MoleculeSimulator molecule in _molecules)
            {
                molecule.MoveToComplex( simulator );
            }
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