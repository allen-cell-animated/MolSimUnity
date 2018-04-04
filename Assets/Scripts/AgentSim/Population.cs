using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Population : MonoBehaviour 
    {
        public Reactor reactor;
        public float diffusionCoefficient;

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

        string species;
        int id = -1;
        string complexName
        {
            get
            {
                id++;
                return species + id;
            }
        }

        public virtual void Init (ComplexConcentration complexConcentration, Reactor _reactor)
        {
            ComplexState complexState = complexConcentration.complexState;
            reactor = _reactor;
            species = complexState.species;
            diffusionCoefficient = complexState.diffusionCoefficient;
            amount = Mathf.RoundToInt( complexConcentration.concentration * reactor.container.volume * 6.022141e23f );

            if (amount > 0 && complexConcentration.moleculeCount > 0)
            {
                SpawnComplexes( complexState );
            }
        }

        protected virtual void SpawnComplexes (ComplexState complexState)
        {
            bool singleMolecule = complexState.moleculeStates.Length == 1;

            GameObject complexObject;
            ParticleSimulator particleSimulator;
            MoleculeSimulator[] complex = null;
            RelativeTransform[] moleculeTransforms = null;
            BimolecularReactionSimulator[] relevantBimolecularSimulators = reactor.GetRelevantBimolecularReactionSimulators( complexState );
            CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators = reactor.GetRelevantCollisionFreeReactionSimulators( complexState );
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
                    complex = new MoleculeSimulator[]{ CreateMoleculeSimulator( complexObject, complexState.moleculeStates[0], particleSimulator, 
                                                                                relevantBimolecularSimulators, relevantCollisionFreeSimulators ) }; 
                }
                else 
                { 
                    complex = SpawnMoleculesInComplex( particleSimulator, complexState, moleculeTransforms, relevantBimolecularSimulators, relevantCollisionFreeSimulators ); 
                }

                particleSimulator.Init( complex, this );
            }
        }

        protected virtual void NameAndPlaceComplex (GameObject complexObject, Vector3 position, Quaternion rotation)
        {
            complexObject.name = complexName;
            complexObject.transform.SetParent( transform );
            complexObject.transform.position = position;
            complexObject.transform.rotation = rotation;
        }

        protected virtual MoleculeSimulator[] SpawnMoleculesInComplex (ParticleSimulator particleSimulator, ComplexState complexState, RelativeTransform[] moleculeTransforms,
                                                                       BimolecularReactionSimulator[] relevantBimolecularSimulators, CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            GameObject moleculeObject;
            List<MoleculeSimulator> complexList = new List<MoleculeSimulator>();
            for (int i = 0; i < complexState.moleculeStates.Length; i++)
            {
                moleculeObject = SpawnMolecule( complexState.moleculeStates[i] );
                if (moleculeObject != null)
                {
                    NameAndPlaceMoleculeInComplex( moleculeObject, complexState.moleculeStates[i].molecule.species, particleSimulator.theTransform, moleculeTransforms[i] );
                    complexList.Add( CreateMoleculeSimulator( moleculeObject, complexState.moleculeStates[i], particleSimulator, relevantBimolecularSimulators, relevantCollisionFreeSimulators ) );
                }
            }
            ConnectBoundSites( complexList );
            return complexList.ToArray();
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

        protected virtual MoleculeSimulator CreateMoleculeSimulator (GameObject moleculeObject, MoleculeState moleculeState, ParticleSimulator particleSimulator, 
                                                                     BimolecularReactionSimulator[] relevantBimolecularSimulators, CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            MoleculeSimulator moleculeSimulator = moleculeObject.AddComponent<MoleculeSimulator>();
            moleculeSimulator.Init( moleculeState, particleSimulator, relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            return moleculeSimulator;
        }

        protected virtual void ConnectBoundSites (List<MoleculeSimulator> complexList)
        {
            Dictionary<string,BindingSiteSimulator> boundBindingSiteSimulators = new Dictionary<string, BindingSiteSimulator>();
            string boundState;
            foreach (MoleculeSimulator moleculeSimulator in complexList)
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

        public virtual ParticleSimulator CreateComplex (Transform centerTransform, MoleculeSimulator[] complex)
        {
            bool singleMolecule = complex.Length == 1;

            ParticleSimulator particleSimulator = null;
            if (singleMolecule)
            {
                particleSimulator = complex[0].gameObject.AddComponent<ParticleSimulator>();
                particleSimulator.name = complexName;
            }
            else if (complex.Length > 1)
            {
                GameObject complexObject = new GameObject();
                NameAndPlaceComplex( complexObject, centerTransform.position, centerTransform.rotation );
                particleSimulator = complexObject.AddComponent<ParticleSimulator>();
            }

            BimolecularReactionSimulator[] relevantBimolecularSimulators = reactor.GetRelevantBimolecularReactionSimulators( complex );
            CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators = reactor.GetRelevantCollisionFreeReactionSimulators( complex );
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                moleculeSimulator.MoveToComplex( particleSimulator, relevantBimolecularSimulators, relevantCollisionFreeSimulators, 
                                                 singleMolecule ? theTransform : particleSimulator.theTransform );
            }
            particleSimulator.Init( complex, this );

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

        public string ComplexToString (MoleculeSimulator[] complex)
        {
            string s = "";
            int i = 0;
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                s += moleculeSimulator.species + ":";
                int j = 0;
                foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
                {
                    s += "[" + bindingSiteSimulator.id + "]=" + bindingSiteSimulator.state;
                    if (j < moleculeSimulator.bindingSiteSimulators.Count - 1)
                    {
                        s += ",";
                    }
                    j++;
                }

                if (i < complex.Length - 1)
                {
                    s += " | ";
                }
                i++;
            }
            return s;
        }

        public override string ToString ()
        {
            return "Population " + name;
        }
    }
}