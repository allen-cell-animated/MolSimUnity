using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reactor : MonoBehaviour 
    {
        [HideInInspector] public Container container;
        public ModelDef modelDef;
        public List<BimolecularReaction> bimolecularReactions = new List<BimolecularReaction>();
        public List<CollisionFreeReaction> collisionFreeReactions = new List<CollisionFreeReaction>();
        [Tooltip( "How many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;

        public List<Mover> movers = new List<Mover>();
        public List<Complex> complexes = new List<Complex>();

        //[HideInInspector] public int moveFails;
        //[SerializeField] float stepsPerMoveFail;
        //[SerializeField] float percentOccupiedVolume;

        float GetPercentOccupiedVolume ()
        {
            float occupiedVolume = 0;
            Molecule[] allMolecules = GetComponentsInChildren<Molecule>();
            foreach (Molecule molecule in allMolecules)
            {
                occupiedVolume += Mathf.PI * Mathf.Pow( molecule.collisionRadius, 2f );
            }
            return 100f * occupiedVolume / (container.size.x * container.size.y * container.size.z);
        }

        float dT
        {
            get
            {
                return World.Instance.dT;
            }
        }

        int id = -1;
        string nextID
        {
            get
            {
                id++;
                return id.ToString();
            }
        }

        void Start ()
        {
            CreateReactionSimulators();
            CreateContainer();
            SpawnComplexes();
            //percentOccupiedVolume = GetPercentOccupiedVolume();
        }

        protected virtual void CreateReactionSimulators ()
        {
            modelDef.Init(); //for prototyping in inspector without writing custom property drawer etc

            foreach (ReactionDef reactionDef in modelDef.reactionDefs)
            {
                if (reactionDef.isBimolecular)
                {
                    bimolecularReactions.Add( new BimolecularReaction( reactionDef, this ) );
                }
                else
                {
                    collisionFreeReactions.Add( new CollisionFreeReaction( reactionDef, this ) );
                }
            }
        }

        protected virtual void CreateContainer ()
        {
            container = gameObject.AddComponent<Container>();
            container.Init( modelDef.scale, modelDef.containerVolume, periodicBoundary );
        }

        protected virtual void SpawnComplexes ()
        {
            foreach (ComplexConcentration complex in modelDef.complexes)
            {
                SpawnComplexInstances( complex );
            }
        }

        protected virtual void SpawnComplexInstances (ComplexConcentration complexConcentration)
        {
            int amount = Mathf.RoundToInt( complexConcentration.concentration * container.volume * 6.022141e23f );
            if (amount < 1 || complexConcentration.moleculeCount < 1)
            {
                return;
            }

            MoleculeInitData initData = new MoleculeInitData( complexConcentration.complexPattern, CalculateMoleculeTransforms( complexConcentration.complexPattern ),
                                                              GetRelevantBimolecularReactions( complexConcentration.complexPattern ),
                                                              GetRelevantCollisionFreeReactions( complexConcentration.complexPattern ) );

            Complex complex;
            for (int i = 0; i < amount; i++)
            {
                complex = new GameObject( nextID ).AddComponent<Complex>();
                complex.gameObject.transform.SetParent( transform );
                complex.gameObject.transform.position = container.GetRandomPointInBounds( 0.1f );
                complex.gameObject.transform.rotation = Random.rotation;

                complex.SpawnMolecules( initData );
                complex.Init( this );
            }
        }

        protected virtual Dictionary<string,List<RelativeTransform>> CalculateMoleculeTransforms (ComplexPattern complexPattern)
        {
            Dictionary<string,List<RelativeTransform>> transforms = new Dictionary<string,List<RelativeTransform>>();
            Vector3 averagePosition = Vector3.zero;

            //temp transforms for calculation
            Transform molecule1 = new GameObject( "molecule1" ).transform;
            Transform molecule2 = new GameObject( "molecule2" ).transform;
            Transform component1 = new GameObject( "component1" ).transform;
            Transform component2 = new GameObject( "component2" ).transform;
            component1.SetParent( molecule1 );
            component2.SetParent( molecule2 );

            //TODO
            //List<ComponentPattern> foundComponents = new List<ComponentPattern>();
            //foreach (string moleculeName1 in complexPattern.moleculePatterns.Keys)
            //{
            //    foreach (MoleculePattern moleculePattern1 in complexPattern.moleculePatterns[moleculeName1])
            //    {
            //        foreach (List<ComponentPattern> aTypeOfComponent1 in moleculePattern1.componentPatterns.Values)
            //        {
            //            for (int i = 0; i < aTypeOfComponent1.Count; i++)
            //            {
            //                if (aTypeOfComponent1[i].state.Contains( "!" ))
            //                {
            //                    foreach (string moleculeName2 in complexPattern.moleculePatterns.Keys)
            //                    {
            //                        foreach (MoleculePattern moleculePattern2 in complexPattern.moleculePatterns[moleculeName2])
            //                        {
            //                            foreach (List<ComponentPattern> aTypeOfComponent2 in moleculePattern2.componentPatterns.Values)
            //                            {
            //                                for (int j = 0; j < aTypeOfComponent2.Count; j++)
            //                                {
            //                                    if (aTypeOfComponent1[i].state == aTypeOfComponent2[j].state 
            //                                        && !foundComponents.Contains( aTypeOfComponent1[i] ) && !foundComponents.Contains( aTypeOfComponent2[j] ))
            //                                    {
            //                                        foundComponents.Add( aTypeOfComponent1[i] );
            //                                        foundComponents.Add( aTypeOfComponent2[j] );

            //                                        molecule1.position = transforms[i].position;
            //                                        molecule1.rotation = Quaternion.Euler( transforms[i].rotation );
            //                                        component = complexPattern.moleculePatterns[i].moleculeDef.componentDefs[componentPattern1.componentName][componentIndex1];
            //                                        component.transformOnMolecule.Apply( molecule1, component1 );

            //                                        molecule2.position = Vector3.zero;
            //                                        molecule2.rotation = Quaternion.identity;
            //                                        component = complexPattern.moleculePatterns[j].moleculeDef.componentDefs[componentPattern2.componentName][componentIndex2];
            //                                        component.transformOnMolecule.Apply( molecule2, component2 );

            //                                        molecule2.position = component1.TransformPoint( component2.InverseTransformPoint( molecule2.position ) );
            //                                        molecule2.rotation = molecule2.rotation * Quaternion.Inverse( component2.rotation ) * component1.rotation;

            //                                        transforms[j] = new RelativeTransform( molecule2.position, molecule2.rotation.eulerAngles );
            //                                        averagePosition += transforms[j].position;
            //                                    }
            //                                    componentIndex2++;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                componentIndex1++;
            //            }
            //        }
            //    }
            //}

            //averagePosition /= transforms.Length;
            //for (int i = 0; i < transforms.Length; i++)
            //{
            //    transforms[i].position -= averagePosition;
            //}

            Destroy( molecule1.gameObject );
            Destroy( molecule2.gameObject );

            return transforms;
        }

        protected BimolecularReaction[] GetRelevantBimolecularReactions (ComplexPattern complexPattern)
        {
            List<BimolecularReaction> reactionsList = new List<BimolecularReaction>();
            foreach (BimolecularReaction reaction in bimolecularReactions)
            {
                if (reaction.ComplexIsReactant( complexPattern ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        public BimolecularReaction[] GetRelevantBimolecularReactions (Dictionary<string,List<Molecule>> molecules)
        {
            List<BimolecularReaction> reactionsList = new List<BimolecularReaction>();
            foreach (BimolecularReaction reaction in bimolecularReactions)
            {
                if (reaction.ComplexIsReactant( molecules ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        protected CollisionFreeReaction[] GetRelevantCollisionFreeReactions (ComplexPattern complexPattern)
        {
            List<CollisionFreeReaction> reactionsList = new List<CollisionFreeReaction>();
            foreach (CollisionFreeReaction reaction in collisionFreeReactions)
            {
                if (reaction.ComplexIsReactant( complexPattern ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        public CollisionFreeReaction[] GetRelevantCollisionFreeReactions (Dictionary<string,List<Molecule>> molecules)
        {
            List<CollisionFreeReaction> reactionsList = new List<CollisionFreeReaction>();
            foreach (CollisionFreeReaction reaction in collisionFreeReactions)
            {
                if (reaction.ComplexIsReactant( molecules ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        public void RegisterMover (Mover mover)
        {
            if (!movers.Contains( mover ))
            {
                movers.Add( mover );
            }
        }

        public void UnregisterMover (Mover mover)
        {
            if (movers.Contains( mover ))
            {
                movers.Remove( mover );
            }
        }

        public void RegisterComplex (Complex complex)
        {
            if (complex.couldReactOnCollision)
            {
                if (!complexes.Contains( complex ))
                {
                    complexes.Add( complex );
                }
            }
        }

        void Update ()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown( KeyCode.X ))
            {
                if (MolSimTests.StateOfReactorIsCorrect( this ) && !MolSimTests.debug)
                {
                    Debug.Log( "Reactor passed check" );
                }
            }
            #endif

            Cleanup();

            //UnityEngine.Profiling.Profiler.BeginSample("MoveParticles");
            MoveParticles();
            //UnityEngine.Profiling.Profiler.EndSample();

            //UnityEngine.Profiling.Profiler.BeginSample("CollisionFreeReactions");
            DoCollisionFreeReactions();
            //UnityEngine.Profiling.Profiler.EndSample();

            //UnityEngine.Profiling.Profiler.BeginSample("BimolecularReactions");
            DoBimolecularReactions();
            //UnityEngine.Profiling.Profiler.EndSample();

            DestroyOldComplexes();

            //stepsPerMoveFail = World.Instance.steps / (float)moveFails;
        }

        protected virtual void MoveParticles ()
        {
            foreach (Mover mover in movers)
            {
                mover.Move( dT );
            }
        }

        public virtual bool WillCollide (Mover mover, Vector3 newPosition)
        {
            foreach (Mover otherMover in movers)
            {
                if (mover.IsCollidingWith( otherMover, newPosition ))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void DoCollisionFreeReactions ()
        {
            foreach (Reaction reaction in collisionFreeReactions)
            {
                reaction.CalculateObservedRate();
            }

            collisionFreeReactions.Shuffle();
            foreach (CollisionFreeReaction collisionFreeReaction in collisionFreeReactions)
            {
                collisionFreeReaction.TryReact();
            }
        }

        protected virtual void DoBimolecularReactions ()
        {
            foreach (Reaction reaction in bimolecularReactions)
            {
                reaction.CalculateObservedRate();
            }

            Complex complex1, complex2;
            int start = complexes.GetRandomIndex();
            for (int i = 0; i < complexes.Count; i++)
            {
                complex1 = complexes[(start + i) % complexes.Count];
                if (!complex1.readyToBeDestroyed)
                {
                    for (int j = i + 1; j < complexes.Count; j++)
                    {
                        complex2 = complexes[(start + j) % complexes.Count];
                        if (!complex2.readyToBeDestroyed)
                        {
                            complex1.InteractWith( complex2 );
                        }
                    }
                }
            }
        }

        public virtual void MoveMoleculesToNewComplex (Dictionary<string,List<Molecule>> molecules, Transform centerTransform)
        {
            //create complex
            Complex complex = new GameObject( nextID ).AddComponent<Complex>();
            complex.gameObject.transform.SetParent( transform );
            complex.gameObject.transform.position = centerTransform.position;
            complex.gameObject.transform.rotation = centerTransform.rotation;

            //move molecules
            BimolecularReaction[] relevantBimolecularReactions = GetRelevantBimolecularReactions( molecules );
            CollisionFreeReaction[] relevantCollisionFreeReactions = GetRelevantCollisionFreeReactions( molecules );
            complex.molecules = molecules;
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.MoveToComplex( complex, relevantBimolecularReactions, relevantCollisionFreeReactions );
                }
            }

            complex.Init( this );
        }

        protected void DestroyOldComplexes ()
        {
            List<Complex> complexesToDestroy = complexes.FindAll( c => c.readyToBeDestroyed );
            foreach (Complex complex in complexesToDestroy)
            {
                Destroy( complex );
            }
        }

        public void Cleanup ()
        {
            complexes.RemoveAll( c => c == null || !c.couldReactOnCollision );
        }
    }

    public class MoleculeInitData
    {
        public ComplexPattern complexPattern;
        public Dictionary<string,List<RelativeTransform>> moleculeTransforms;
        public BimolecularReaction[] relevantBimolecularReactions;
        public CollisionFreeReaction[] relevantCollisionFreeReactions;

        public MoleculeInitData (ComplexPattern _complexPattern, Dictionary<string,List<RelativeTransform>> _moleculeTransforms,
                                 BimolecularReaction[] _relevantBimolecularReactions, 
                                 CollisionFreeReaction[] _relevantCollisionFreeReactions)
        {
            complexPattern = _complexPattern;
            moleculeTransforms = _moleculeTransforms;
            relevantBimolecularReactions = _relevantBimolecularReactions;
            relevantCollisionFreeReactions = _relevantCollisionFreeReactions;
        }
    }
}