using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticlePopulation : MonoBehaviour 
    {
        public Reactor reactor;
        public ComplexState complexState;
        public float collisionRadius;
        public float interactionRadius;
        public List<BindingSitePopulation> bindingSitePopulations = new List<BindingSitePopulation>();

        Transform _theTransform;
        public Transform theTransform
        {
            get
            {
                if (_theTransform == null)
                {
                    _theTransform = transform;
                }
                return _theTransform;
            }
        }

        int amount;
        public float concentration
        {
            get
            {
                return amount / ( reactor.container.volume * 6.022141e23f );
            }
        }

        int id = -1;
        string complexName
        {
            get
            {
                id++;
                return complexState.species + id;
            }
        }

        public virtual void Init (ComplexConcentration complexConcentration, Reactor _reactor)
        {
            reactor = _reactor;
            complexState = complexConcentration.complexState;
            collisionRadius = interactionRadius = complexState.radius;
            interactionRadius += 1f;
            amount = Mathf.RoundToInt( complexConcentration.concentration * reactor.container.volume * 6.022141e23f );

            CreateBindingSitePopulations( complexState );

            if (amount > 0 && complexConcentration.moleculeCount > 0)
            {
                SpawnComplexes( complexState );
            }
        }

        protected virtual void CreateBindingSitePopulations (ComplexState complexState)
        {
            BindingSitePopulation bindingSitePopulation;
            foreach (MoleculeState moleculeState in complexState.moleculeStates)
            {
                foreach (BindingSite bindingSite in moleculeState.molecule.bindingSites.Values)
                {
                    bindingSitePopulation = gameObject.AddComponent<BindingSitePopulation>();
                    bindingSitePopulation.Init( moleculeState, bindingSite.id, this );
                    bindingSitePopulations.Add( bindingSitePopulation );
                }
            }
        }

        protected virtual void SpawnComplexes (ComplexState complexState)
        {
            bool singleMolecule = complexState.moleculeStates.Length == 1;

            GameObject complexObject;
            ParticleSimulator particleSimulator;
            MoleculeSimulator[] moleculeSimulators = null;
            RelativeTransform[] moleculeTransforms = null;
            if (!singleMolecule) { moleculeTransforms = CalculateMoleculeTransforms( complexState ); }

            for (int i = 0; i < amount; i++)
            {
                if (singleMolecule) { complexObject = SpawnMolecule( complexState.moleculeStates[0] ); }
                else { complexObject = new GameObject(); }

                NameAndPlaceComplex( complexObject, reactor.container.GetRandomPointInBounds( 0.1f ), Random.rotation );
                particleSimulator = complexObject.AddComponent<ParticleSimulator>();

                if (singleMolecule) 
                { 
                    complexObject.name += "_" + complexState.moleculeStates[0].molecule.species;
                    moleculeSimulators = new MoleculeSimulator[]{ CreateMoleculeSimulator( complexObject, complexState.moleculeStates[0], particleSimulator ) }; 
                }
                else 
                { 
                    moleculeSimulators = SpawnMoleculesInComplex( particleSimulator, complexState, moleculeTransforms ); 
                }

                particleSimulator.Init( moleculeSimulators, this );
            }
        }

        protected virtual void NameAndPlaceComplex (GameObject complexObject, Vector3 position, Quaternion rotation)
        {
            complexObject.name = complexName;
            complexObject.transform.SetParent( transform );
            complexObject.transform.position = position;
            complexObject.transform.rotation = rotation;
        }

        protected virtual MoleculeSimulator[] SpawnMoleculesInComplex (ParticleSimulator particleSimulator, ComplexState complexState, RelativeTransform[] moleculeTransforms)
        {
            GameObject moleculeObject;
            List<MoleculeSimulator> moleculeSimulators = new List<MoleculeSimulator>();
            for (int i = 0; i < complexState.moleculeStates.Length; i++)
            {
                moleculeObject = SpawnMolecule( complexState.moleculeStates[i] );
                if (moleculeObject != null)
                {
                    NameAndPlaceMoleculeInComplex( moleculeObject, complexState.moleculeStates[i].molecule.species, particleSimulator.theTransform, moleculeTransforms[i] );
                    moleculeSimulators.Add( CreateMoleculeSimulator( moleculeObject, complexState.moleculeStates[i], particleSimulator ) );
                }
            }
            ConnectBoundSites( moleculeSimulators );
            return moleculeSimulators.ToArray();
        }

        protected virtual GameObject SpawnMolecule (MoleculeState moleculeState)
        {
            GameObject visualizationPrefab = moleculeState.molecule.visualizationPrefab;
            if (visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s molecule prefab is null!" );
                visualizationPrefab = Resources.Load( "DefaultMolecule" ) as GameObject;
            }

            GameObject molecule = Instantiate( visualizationPrefab );
            return molecule;
        }

        protected virtual void NameAndPlaceMoleculeInComplex (GameObject moleculeObject, string species, Transform complexTransform, RelativeTransform relativeTransform)
        {
            moleculeObject.name = complexTransform.name + "_" + species;
            moleculeObject.transform.SetParent( complexTransform );
            moleculeObject.transform.position = complexTransform.TransformPoint( relativeTransform.position );
            moleculeObject.transform.rotation = complexTransform.rotation * Quaternion.Euler( relativeTransform.rotation );
        }

        protected virtual MoleculeSimulator CreateMoleculeSimulator (GameObject moleculeObject, MoleculeState moleculeState, ParticleSimulator particleSimulator)
        {
            MoleculeSimulator moleculeSimulator = moleculeObject.AddComponent<MoleculeSimulator>();
            moleculeSimulator.Init( moleculeState, particleSimulator, this );
            return moleculeSimulator;
        }

        protected virtual void ConnectBoundSites (List<MoleculeSimulator> moleculeSimulators)
        {
            Dictionary<string,BindingSiteSimulator> boundBindingSiteSimulators = new Dictionary<string, BindingSiteSimulator>();
            string boundState;
            foreach (MoleculeSimulator moleculeSimulator in moleculeSimulators)
            {
                foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
                {
                    boundState = bindingSiteSimulator.state;
                    if (boundState.Contains( "!" ))
                    {
                        if (!boundBindingSiteSimulators.ContainsKey( boundState ))
                        {
                            boundBindingSiteSimulators.Add( boundState, bindingSiteSimulator );
                        }
                        else
                        {
                            boundBindingSiteSimulators[boundState].boundSite = bindingSiteSimulator;
                            bindingSiteSimulator.boundSite = boundBindingSiteSimulators[boundState];
                        }
                    }
                }
            }
        }

        public virtual ParticleSimulator CreateComplexWithMoleculeSimulators (Transform centerTransform, MoleculeSimulator[] _moleculeSimulators)
        {
            ParticleSimulator particleSimulator = null;
            if (_moleculeSimulators.Length == 1)
            {
                particleSimulator = _moleculeSimulators[0].gameObject.AddComponent<ParticleSimulator>();
                particleSimulator.name = complexName;
            }
            else if (_moleculeSimulators.Length > 1)
            {
                GameObject complexObject = new GameObject();
                NameAndPlaceComplex( complexObject, centerTransform.position, centerTransform.rotation );
                particleSimulator = complexObject.AddComponent<ParticleSimulator>();
            }

            foreach (MoleculeSimulator moleculeSimulator in _moleculeSimulators)
            {
                moleculeSimulator.MoveToComplex( particleSimulator, this );
            }
            particleSimulator.Init( _moleculeSimulators, this );

            return particleSimulator;
        }

        protected virtual RelativeTransform[] CalculateMoleculeTransforms (ComplexState complexState)
        {
            RelativeTransform[] transforms = new RelativeTransform[complexState.moleculeStates.Length];
            transforms[0] = new RelativeTransform( Vector3.zero, Vector3.zero );
            Vector3 averagePosition = Vector3.zero;

            Transform molecule1 = new GameObject( "molecule1" ).transform;
            Transform molecule2 = new GameObject( "molecule2" ).transform;
            Transform site1 = new GameObject( "site1" ).transform;
            site1.SetParent( molecule1 );
            Transform site2 = new GameObject( "site2" ).transform;
            site2.SetParent( molecule2 );
            BindingSite bindingSite;

            for (int i = 0; i < complexState.moleculeStates.Length - 1; i++)
            {
                foreach (KeyValuePair<string,string> siteState1 in complexState.moleculeStates[i].bindingSiteStates)
                {
                    if (siteState1.Value.Contains( "!" ))
                    {
                        for (int j = i + 1; j < complexState.moleculeStates.Length; j++)
                        {
                            foreach (KeyValuePair<string,string> siteState2 in complexState.moleculeStates[j].bindingSiteStates)
                            {
                                if (siteState1.Value == siteState2.Value)
                                {
                                    molecule1.position = transforms[i].position;
                                    molecule1.rotation = Quaternion.Euler( transforms[i].rotation );
                                    bindingSite = complexState.moleculeStates[i].molecule.bindingSites[siteState1.Key];
                                    bindingSite.transformOnMolecule.Apply( molecule1, site1 );

                                    molecule2.position = Vector3.zero;
                                    molecule2.rotation = Quaternion.identity;
                                    bindingSite = complexState.moleculeStates[j].molecule.bindingSites[siteState2.Key];
                                    bindingSite.transformOnMolecule.Apply( molecule2, site2 );

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

        public virtual BindingSitePopulation GetBindingSitePopulation (MoleculeBindingSite moleculeBindingSite)
        {
            foreach (BindingSitePopulation bindingSitePopulation in bindingSitePopulations)
            {
                if (bindingSitePopulation.moleculeBindingSite.Matches( moleculeBindingSite ))
                {
                    return bindingSitePopulation;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return "ParticlePopulation " + name;
        }
    }
}