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

            MoleculeInitData initData = new MoleculeInitData( complexConcentration.complexSnapshot, CalculateMoleculeTransforms( complexConcentration.complexSnapshot ),
                                                              GetRelevantBimolecularReactions( complexConcentration.complexSnapshot ),
                                                              GetRelevantCollisionFreeReactions( complexConcentration.complexSnapshot ) );

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

        protected BimolecularReaction[] GetRelevantBimolecularReactions (ComplexSnapshot complexSnapshot)
        {
            List<BimolecularReaction> reactionsList = new List<BimolecularReaction>();
            foreach (BimolecularReaction reaction in bimolecularReactions)
            {
                if (reaction.ComplexIsReactant( complexSnapshot ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        public BimolecularReaction[] GetRelevantBimolecularReactions (Molecule[] molecules)
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

        protected CollisionFreeReaction[] GetRelevantCollisionFreeReactions (ComplexSnapshot complexSnapshot)
        {
            List<CollisionFreeReaction> reactionsList = new List<CollisionFreeReaction>();
            foreach (CollisionFreeReaction reaction in collisionFreeReactions)
            {
                if (reaction.ComplexIsReactant( complexSnapshot ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        public CollisionFreeReaction[] GetRelevantCollisionFreeReactions (Molecule[] molecules)
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

        public virtual void MoveMoleculesToNewComplex (Molecule[] molecules, Transform centerTransform)
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
            foreach (Molecule molecule in molecules)
            {
                molecule.MoveToComplex( complex, relevantBimolecularReactions, relevantCollisionFreeReactions );
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