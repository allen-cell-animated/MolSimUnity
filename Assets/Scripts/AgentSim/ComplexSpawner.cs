using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ComplexSpawner : MonoBehaviour 
    {
        public Reactor reactor;

        int id = -1;
        string nextID
        {
            get
            {
                id++;
                return id.ToString();
            }
        }

        public virtual void Init (Reactor _reactor)
        {
            reactor = _reactor;
        }

        public virtual void SpawnComplexes (ComplexConcentration complexConcentration)
        {
            int amount = Mathf.RoundToInt( complexConcentration.concentration * reactor.container.volume * 6.022141e23f );
            if (amount < 1 || complexConcentration.moleculeCount < 1)
            {
                return;
            }

            MoleculeInitData initData = new MoleculeInitData( complexConcentration.complexState, CalculateMoleculeTransforms( complexConcentration.complexState ),
                                                              reactor.GetRelevantBimolecularReactionSimulators( complexConcentration.complexState ),
                                                              reactor.GetRelevantCollisionFreeReactionSimulators( complexConcentration.complexState ) );
            ComplexSimulator complexSimulator;
            for (int i = 0; i < amount; i++)
            {
                complexSimulator = new GameObject( nextID ).AddComponent<ComplexSimulator>();
                complexSimulator.gameObject.transform.SetParent( transform );
                complexSimulator.gameObject.transform.position = reactor.container.GetRandomPointInBounds( 0.1f );
                complexSimulator.gameObject.transform.rotation = Random.rotation;

                complexSimulator.SpawnMolecules( initData );
                complexSimulator.Init( reactor );
            }
        }

        public virtual void CreateComplex (Transform centerTransform, MoleculeSimulator[] complex)
        {
            ComplexSimulator complexSimulator = new GameObject( nextID ).AddComponent<ComplexSimulator>();
            complexSimulator.gameObject.transform.SetParent( transform );
            complexSimulator.gameObject.transform.position = centerTransform.position;
            complexSimulator.gameObject.transform.rotation = centerTransform.rotation;

            BimolecularReactionSimulator[] relevantBimolecularSimulators = reactor.GetRelevantBimolecularReactionSimulators( complex );
            CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators = reactor.GetRelevantCollisionFreeReactionSimulators( complex );
            complexSimulator.SetMolecules( complex, relevantBimolecularSimulators, relevantCollisionFreeSimulators );

            complexSimulator.Init( reactor );
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
    }

    public class MoleculeInitData
    {
        public ComplexState complexState;
        public RelativeTransform[] moleculeTransforms;
        public BimolecularReactionSimulator[] relevantBimolecularSimulators;
        public CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators;

        public MoleculeInitData (ComplexState _complexState, RelativeTransform[] _moleculeTransforms,
                                 BimolecularReactionSimulator[] _relevantBimolecularSimulators, 
                                 CollisionFreeReactionSimulator[] _relevantCollisionFreeSimulators)
        {
            complexState = _complexState;
            moleculeTransforms = _moleculeTransforms;
            relevantBimolecularSimulators = _relevantBimolecularSimulators;
            relevantCollisionFreeSimulators = _relevantCollisionFreeSimulators;
        }
    }
}