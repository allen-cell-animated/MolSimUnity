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
            RelativeTransform[] transforms = GetMoleculeTransforms( moleculeStates );
            MoleculeSimulator complex;
            for (int i = 0; i < amount; i++)
            {
                complex = CreateComplex( reactor.container.GetRandomPointInBounds( 0.1f ), Random.rotation );
                SpawnMoleculesInComplex( complex, moleculeStates, transforms );
            }
        }

        protected virtual RelativeTransform[] GetMoleculeTransforms (MoleculeState[] moleculeStates)
        {
            RelativeTransform[] transforms = new RelativeTransform[moleculeStates.Length];
            transforms[0] = new RelativeTransform( Vector3.zero, Vector3.zero );
            Vector3 averagePosition = Vector3.zero;

            Transform molecule1 = new GameObject( "molecule1" ).transform;
            Transform molecule2 = new GameObject( "molecule2" ).transform;
            Transform site1 = new GameObject( "site1" ).transform;
            site1.SetParent( molecule1 );
            Transform site2 = new GameObject( "site2" ).transform;
            site2.SetParent( molecule2 );
            BindingSite bs1, bs2;

            for (int i = 0; i < moleculeStates.Length - 1; i++)
            {
                foreach (KeyValuePair<string,string> siteState1 in moleculeStates[i].bindingSiteStates)
                {
                    if (siteState1.Value.Contains( "!" ))
                    {
                        for (int j = i + 1; j < moleculeStates.Length; j++)
                        {
                            foreach (KeyValuePair<string,string> siteState2 in moleculeStates[j].bindingSiteStates)
                            {
                                if (siteState1.Value == siteState2.Value)
                                {
                                    molecule1.position = transforms[i].position;
                                    molecule1.rotation = Quaternion.Euler( transforms[i].rotation );
                                    bs1 = moleculeStates[i].molecule.GetSiteByID( siteState1.Key );
                                    bs1.transformOnMolecule.Apply( molecule1, site1 );

                                    molecule2.position = Vector3.zero;
                                    molecule2.rotation = Quaternion.identity;
                                    bs2 = moleculeStates[j].molecule.GetSiteByID( siteState2.Key );
                                    bs2.transformOnMolecule.Apply( molecule2, site2 );

                                    molecule2.position = site1.TransformPoint( site2.InverseTransformPoint( molecule2.position ) );
                                    molecule2.rotation = molecule2.rotation * Quaternion.Inverse( site2.rotation ) * site1.rotation;
                                    
                                    transforms[j] = new RelativeTransform( molecule2.position, molecule2.rotation.eulerAngles );
                                    averagePosition += transforms[j].position;
                                }
                            }
                        }
                    }
                }
            }
            averagePosition /= transforms.Length;
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].position -= averagePosition;
            }
            Destroy( molecule1.gameObject );
            Destroy( molecule2.gameObject );
            return transforms;
        }

        protected virtual MoleculeSimulator CreateComplex (Vector3 position, Quaternion rotation)
        {
            GameObject obj = new GameObject( species + "Complex_" + transform.childCount );
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
                reactor.container.RegisterMolecule( simulator as ManagedMoleculeSimulator );
            }
            simulator.Init( this );
            return simulator;
        }

        protected virtual void SpawnMoleculesInComplex (MoleculeSimulator complex, MoleculeState[] moleculeStates, RelativeTransform[] moleculeTransforms)
        {
            for (int i = 0; i < moleculeStates.Length; i++)
            {
                SpawnMolecule( complex, moleculeStates[i], moleculeTransforms[i] );
            }
        }

        public virtual void SpawnMolecule (MoleculeSimulator complex, MoleculeState moleculeState, RelativeTransform relativeTransform)
        {
            if (moleculeState.molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s molecule prefab is null!" );
                return;
            }

            GameObject particle = Instantiate( moleculeState.molecule.visualizationPrefab );
            particle.name = complex.name + "_" + molecules[0].species;
            particle.transform.SetParent( complex.transform );
            particle.transform.position = complex.transform.TransformPoint( relativeTransform.position );
            particle.transform.rotation = complex.transform.rotation * Quaternion.Euler( relativeTransform.rotation );

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
            simulator.ToggleMotion( false );
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