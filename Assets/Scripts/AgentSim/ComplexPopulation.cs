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
        public float diffusionCoefficient;
        public float collisionRadius;
        public float interactionRadius;
        public Dictionary<MoleculeBindingSite,BindingSitePopulation> bindingSitePopulations;

        int amount;
        float concentration
        {
            get
            {
                return amount / ( reactor.container.volume * 6.022141e23f );
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
            reactor = _reactor;
            diffusionCoefficient = complexConcentration.complexState.diffusionCoefficient;
            collisionRadius = interactionRadius = complexConcentration.complexState.radius;
            amount = Mathf.RoundToInt( complexConcentration.concentration * reactor.container.volume * 6.022141e23f );

            CreateBindingSitePopulations( complexConcentration.complexState.moleculeStates );

            if (amount > 0 && complexConcentration.moleculeCount > 0)
            {
                SpawnMoleculeComplexes( complexConcentration.complexState.moleculeStates );
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
            MoleculeSimulator complex;
            for (int i = 0; i < amount; i++)
            {
                complex = CreateComplex( reactor.container.GetRandomPointInBounds( 0.1f ), Random.rotation );
                SpawnMoleculesInComplex( complex, moleculeStates );
            }
        }

        protected virtual MoleculeSimulator CreateComplex (Vector3 position, Quaternion rotation)
        {
            GameObject obj = new GameObject( species + "_" + transform.childCount );
            obj.transform.SetParent( transform );
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            MoleculeSimulator simulator;
            if (reactor.usePhysicsEngine)
            {
                simulator = obj.AddComponent<PhysicalMoleculeSimulator>();
            }
            else
            {
                simulator = obj.AddComponent<ManagedMoleculeSimulator>();
            }
            simulator.Init( this );
            return simulator;
        }

        protected virtual void SpawnMoleculesInComplex (MoleculeSimulator complex, MoleculeState[] moleculeStates)
        {
            if (moleculeStates.Length < 2)
            {
                SpawnMolecule( complex, moleculeStates[0], complex.transform.position, complex.transform.rotation );
            }
            else 
            {
                for (int i = 0; i < moleculeStates.Length - 1; i++)
                {
                    for (int j = i + 1; j < moleculeStates.Length; j++)
                    {
                        // TODO place multiple molecules in complex
                    }
                }
            }
        }

        public virtual void SpawnMolecule (MoleculeSimulator complex, MoleculeState moleculeState, Vector3 position, Quaternion rotation)
        {
            if (moleculeState.molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s molecule prefab is null!" );
                return;
            }

            GameObject particle = Instantiate( moleculeState.molecule.visualizationPrefab );
            particle.transform.SetParent( complex.transform );
            particle.transform.position = position;
            particle.transform.rotation = rotation;
            particle.name = molecules[0].species + "_" + complex.transform.childCount;

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
            MoleculeSimulator complex = CreateComplex( centerTransform.position, centerTransform.rotation );

            foreach (MoleculeSimulator molecule in _molecules)
            {
                molecule.MoveToComplex( complex );
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