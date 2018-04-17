using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Spawner : MonoBehaviour 
    {
        int id = -1;
        string nextID
        {
            get
            {
                id++;
                return id.ToString();
            }
        }

        public virtual void SpawnComplexes (ComplexConcentration complexConcentration, Reactor reactor)
        {
            int amount = Mathf.RoundToInt( complexConcentration.concentration * reactor.container.volume * 6.022141e23f );
            if (amount < 1 || complexConcentration.moleculeCount < 1)
            {
                return;
            }

            MoleculeInitData initData = new MoleculeInitData( complexConcentration.complexSnapshot, CalculateMoleculeTransforms( complexConcentration.complexSnapshot ),
                                                              reactor.GetRelevantBimolecularReactions( complexConcentration.complexSnapshot ),
                                                              reactor.GetRelevantCollisionFreeReactions( complexConcentration.complexSnapshot ) );
            
            Complex complex;
            for (int i = 0; i < amount; i++)
            {
                complex = new GameObject( nextID ).AddComponent<Complex>();
                complex.gameObject.transform.SetParent( transform );
                complex.gameObject.transform.position = reactor.container.GetRandomPointInBounds( 0.1f );
                complex.gameObject.transform.rotation = Random.rotation;

                complex.SpawnMolecules( initData );
                complex.Init( reactor );
            }
        }

        public virtual void CreateComplex (Transform centerTransform, Molecule[] molecules, Reactor reactor)
        {
            Complex complex = new GameObject( nextID ).AddComponent<Complex>();
            complex.gameObject.transform.SetParent( transform );
            complex.gameObject.transform.position = centerTransform.position;
            complex.gameObject.transform.rotation = centerTransform.rotation;

            BimolecularReaction[] relevantBimolecularReactions = reactor.GetRelevantBimolecularReactions( molecules );
            CollisionFreeReaction[] relevantCollisionFreeReactions = reactor.GetRelevantCollisionFreeReactions( molecules );
            complex.SetMolecules( molecules, relevantBimolecularReactions, relevantCollisionFreeReactions );

            complex.Init( reactor );
        }

        protected virtual RelativeTransform[] CalculateMoleculeTransforms (ComplexSnapshot complexSnapshot)
        {
            RelativeTransform[] transforms = new RelativeTransform[complexSnapshot.moleculeSnapshots.Length];
            transforms[0] = new RelativeTransform( Vector3.zero, Vector3.zero );
            Vector3 averagePosition = Vector3.zero;

            Transform molecule1 = new GameObject( "molecule1" ).transform;
            Transform molecule2 = new GameObject( "molecule2" ).transform;
            Transform site1 = new GameObject( "site1" ).transform;
            site1.SetParent( molecule1 );
            Transform site2 = new GameObject( "site2" ).transform;
            site2.SetParent( molecule2 );
            BindingSiteDef bindingSite;

            for (int i = 0; i < complexSnapshot.moleculeSnapshots.Length - 1; i++)
            {
                foreach (KeyValuePair<string,string> siteState1 in complexSnapshot.moleculeSnapshots[i].bindingSiteStates)
                {
                    if (siteState1.Value.Contains( "!" ))
                    {
                        for (int j = i + 1; j < complexSnapshot.moleculeSnapshots.Length; j++)
                        {
                            foreach (KeyValuePair<string,string> siteState2 in complexSnapshot.moleculeSnapshots[j].bindingSiteStates)
                            {
                                if (siteState1.Value == siteState2.Value)
                                {
                                    molecule1.position = transforms[i].position;
                                    molecule1.rotation = Quaternion.Euler( transforms[i].rotation );
                                    bindingSite = complexSnapshot.moleculeSnapshots[i].moleculeDef.bindingSiteDefs[siteState1.Key];
                                    bindingSite.transformOnMolecule.Apply( molecule1, site1 );

                                    molecule2.position = Vector3.zero;
                                    molecule2.rotation = Quaternion.identity;
                                    bindingSite = complexSnapshot.moleculeSnapshots[j].moleculeDef.bindingSiteDefs[siteState2.Key];
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
        public ComplexSnapshot complexSnapshot;
        public RelativeTransform[] moleculeTransforms;
        public BimolecularReaction[] relevantBimolecularReactions;
        public CollisionFreeReaction[] relevantCollisionFreeReactions;

        public MoleculeInitData (ComplexSnapshot _complexSnapshot, RelativeTransform[] _moleculeTransforms,
                                 BimolecularReaction[] _relevantBimolecularReactions, 
                                 CollisionFreeReaction[] _relevantCollisionFreeReactions)
        {
            complexSnapshot = _complexSnapshot;
            moleculeTransforms = _moleculeTransforms;
            relevantBimolecularReactions = _relevantBimolecularReactions;
            relevantCollisionFreeReactions = _relevantCollisionFreeReactions;
        }
    }
}