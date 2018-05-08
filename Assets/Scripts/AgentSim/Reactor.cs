using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reactor : MonoBehaviour 
    {
        [HideInInspector] public Container container;
        public ModelDef modelDef;
        public List<BindReaction> bindReactions = new List<BindReaction>();
        public List<CollisionFreeReaction> collisionFreeReactions = new List<CollisionFreeReaction>();

        //for debugging in editor (since CollisionFreeReaction is abstract and isn't serialized)
        [SerializeField] List<ReleaseReaction> releaseReactions = new List<ReleaseReaction>();
        [SerializeField] List<StateChangeReaction> stateChangeReactions = new List<StateChangeReaction>();
        [SerializeField] List<CreateReaction> createReactions = new List<CreateReaction>();
        [SerializeField] List<DestroyReaction> destroyReactions = new List<DestroyReaction>();

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
            CreateReactions();
            CreateContainer();
            SpawnComplexes();
            //percentOccupiedVolume = GetPercentOccupiedVolume();
        }

        protected virtual void CreateReactions ()
        {
            modelDef.Init(); //for prototyping in inspector without writing custom property drawer etc

            foreach (ReactionDef reactionDef in modelDef.reactionDefs)
            {
                switch (reactionDef.type)
                {
                    case ReactionType.Bind:
                        bindReactions.Add( new BindReaction( reactionDef, this ) );
                        break;

                    case ReactionType.Release:
                        ReleaseReaction rr = new ReleaseReaction( reactionDef, this );
                        collisionFreeReactions.Add( rr );
                        releaseReactions.Add( rr );
                        break;

                    case ReactionType.StateChange:
                        StateChangeReaction scr = new StateChangeReaction( reactionDef, this );
                        collisionFreeReactions.Add( scr );
                        stateChangeReactions.Add( scr );
                        break;

                    case ReactionType.Create:
                        CreateReaction cr = new CreateReaction( reactionDef, this );
                        collisionFreeReactions.Add( cr );
                        createReactions.Add( cr );
                        break;

                    case ReactionType.Destroy:
                        DestroyReaction dr = new DestroyReaction( reactionDef, this );
                        collisionFreeReactions.Add( dr );
                        destroyReactions.Add( dr );
                        break;
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
                SpawnSpecies( complex );
            }
        }

        protected virtual void SpawnSpecies (ComplexConcentration complexConcentration)
        {
            int amount = Mathf.RoundToInt( complexConcentration.concentration * container.volume * 6.022141e23f );
            if (amount < 1 || complexConcentration.moleculeCount < 1)
            {
                return;
            }

            MoleculeInitData initData = new MoleculeInitData( complexConcentration.complexPattern, CalculateMoleculeTransforms( complexConcentration.complexPattern ),
                                                              GetRelevantBindReactions( complexConcentration.complexPattern ),
                                                              GetRelevantCollisionFreeReactions( complexConcentration.complexPattern ) );
            
            Complex complex;
            for (int i = 0; i < amount; i++)
            {
                complex = new GameObject( nextID ).AddComponent<Complex>();
                complex.gameObject.transform.SetParent( transform );
                complex.gameObject.transform.position = container.GetRandomPointInBounds( 0.1f );
                complex.gameObject.transform.rotation = Random.rotation;
                complex.reactor = this;

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

            List<ComponentPattern> foundComponents = new List<ComponentPattern>();
            ComponentPattern componentPattern1, componentPattern2;
            ComponentDef componentDef;
            int n = 0;
            foreach (string moleculeName1 in complexPattern.moleculePatterns.Keys)
            {
                if (!transforms.ContainsKey( moleculeName1 ))
                {
                    transforms.Add( moleculeName1, new List<RelativeTransform>() );
                }
                if (transforms.Keys.Count == 1)
                {
                    transforms[moleculeName1].Add( new RelativeTransform( Vector3.zero, Vector3.zero ) );
                    n++;
                }
                for (int m1 = 0; m1 < complexPattern.moleculePatterns[moleculeName1].Count; m1++)
                {
                    foreach (string componentName1 in complexPattern.moleculePatterns[moleculeName1][m1].componentPatterns.Keys)
                    {
                        for (int c1 = 0; c1 < complexPattern.moleculePatterns[moleculeName1][m1].componentPatterns[componentName1].Count; c1++)
                        {
                            componentPattern1 = complexPattern.moleculePatterns[moleculeName1][m1].componentPatterns[componentName1][c1];
                            if (!foundComponents.Contains( componentPattern1 ))
                            {
                                if (complexPattern.moleculePatterns[moleculeName1][m1].componentPatterns[componentName1][c1].bound)
                                {
                                    foreach (string moleculeName2 in complexPattern.moleculePatterns.Keys)
                                    {
                                        for (int m2 = 0; m2 < complexPattern.moleculePatterns[moleculeName2].Count; m2++)
                                        {
                                            if (!(moleculeName2 == moleculeName1 && m2 == m1))
                                            {
                                                foreach (string componentName2 in complexPattern.moleculePatterns[moleculeName2][m2].componentPatterns.Keys)
                                                {
                                                    for (int c2 = 0; c2 < complexPattern.moleculePatterns[moleculeName2][m2].componentPatterns[componentName2].Count; c2++)
                                                    {
                                                        componentPattern2 = complexPattern.moleculePatterns[moleculeName2][m2].componentPatterns[componentName2][c2];
                                                        if (componentPattern2.bound && !foundComponents.Contains( componentPattern2 ))
                                                        {
                                                            if (componentPattern1.bondName == componentPattern2.bondName)
                                                            {
                                                                foundComponents.Add( componentPattern1 );
                                                                foundComponents.Add( componentPattern2 );

                                                                molecule1.position = transforms[moleculeName1][m1].position;
                                                                molecule1.rotation = Quaternion.Euler( transforms[moleculeName1][m1].rotation );
                                                                componentDef = complexPattern.moleculePatterns[moleculeName1][m1].moleculeDef.componentDefs[componentName1][c1];
                                                                componentDef.transformOnMolecule.Apply( molecule1, component1 );

                                                                molecule2.position = Vector3.zero;
                                                                molecule2.rotation = Quaternion.identity;
                                                                componentDef = complexPattern.moleculePatterns[moleculeName2][m2].moleculeDef.componentDefs[componentName2][c2];
                                                                componentDef.transformOnMolecule.Apply( molecule2, component2 );

                                                                molecule2.position = component1.TransformPoint( component2.InverseTransformPoint( molecule2.position ) );
                                                                molecule2.rotation = molecule2.rotation * Quaternion.Inverse( component2.rotation ) * component1.rotation;

                                                                if (!transforms.ContainsKey( moleculeName2 ))
                                                                {
                                                                    transforms.Add( moleculeName2, new List<RelativeTransform>() );
                                                                    n++;
                                                                }
                                                                transforms[moleculeName2].Add( new RelativeTransform( molecule2.position, molecule2.rotation.eulerAngles ) );
                                                                averagePosition += transforms[moleculeName2][m2].position;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            averagePosition /= n;
            foreach (string moleculeName in transforms.Keys)
            {
                foreach (RelativeTransform t in transforms[moleculeName])
                {
                    t.position -= averagePosition;
                }
            }

            Destroy( molecule1.gameObject );
            Destroy( molecule2.gameObject );

            return transforms;
        }

        protected BindReaction[] GetRelevantBindReactions (ComplexPattern complexPattern)
        {
            List<BindReaction> reactionsList = new List<BindReaction>();
            foreach (BindReaction reaction in bindReactions)
            {
                if (reaction.ComplexIsReactant( complexPattern ))
                {
                    reactionsList.Add( reaction );
                }
            }
            return reactionsList.ToArray();
        }

        public BindReaction[] GetRelevantBindReactions (Dictionary<string,List<Molecule>> molecules)
        {
            List<BindReaction> reactionsList = new List<BindReaction>();
            foreach (BindReaction reaction in bindReactions)
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

            UnityEngine.Profiling.Profiler.BeginSample("MoveParticles");
            MoveParticles();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("CollisionFreeReactions");
            DoCollisionFreeReactions();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("BimolecularReactions");
            DoBindReactions();
            UnityEngine.Profiling.Profiler.EndSample();

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

        protected virtual void DoBindReactions ()
        {
            foreach (Reaction reaction in bindReactions)
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

        public virtual Complex MoveMoleculesToNewComplex (Dictionary<string,List<Molecule>> molecules, Transform centerTransform)
        {
            //create complex
            Complex complex = new GameObject( nextID ).AddComponent<Complex>();
            complex.gameObject.transform.SetParent( transform );
            complex.gameObject.transform.position = centerTransform.position;
            complex.gameObject.transform.rotation = centerTransform.rotation;
            complex.reactor = this;

            //move molecules
            complex.molecules = molecules;
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.MoveToComplex( complex );
                }
            }
            complex.Init( this );
            return complex;
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
        public BindReaction[] relevantBindReactions;
        public CollisionFreeReaction[] relevantCollisionFreeReactions;

        public MoleculeInitData (ComplexPattern _complexPattern, Dictionary<string,List<RelativeTransform>> _moleculeTransforms,
                                 BindReaction[] _relevantBindReactions, 
                                 CollisionFreeReaction[] _relevantCollisionFreeReactions)
        {
            complexPattern = _complexPattern;
            moleculeTransforms = _moleculeTransforms;
            relevantBindReactions = _relevantBindReactions;
            relevantCollisionFreeReactions = _relevantCollisionFreeReactions;
        }
    }
}