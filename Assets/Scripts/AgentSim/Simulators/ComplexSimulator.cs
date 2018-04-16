using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ComplexSimulator : MonoBehaviour 
    {
        public Reactor reactor;
        public MoleculeSimulator[] complex;

        protected ParticleSimulator particleSimulator;

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

        string GetSpecies ()
        {
            string s = "";
            for (int i = 0; i < complex.Length; i++)
            {
                s += complex[i].molecule.species;
                if (i < complex.Length - 1)
                {
                    s += ".";
                }
            }
            return s;
        }

        public bool couldReactOnCollision;

        bool GetCouldReactOnCollision ()
        {
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                if (moleculeSimulator.couldReactOnCollision)
                {
                    return true;
                }
            }
            return false;
        }

        float interactionRadius;

        float GetInteractionRadius ()
        {
            float d, maxD = 0;
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                d = Vector3.Distance( theTransform.position, moleculeSimulator.theTransform.position ) + moleculeSimulator.interactionRadius;
                if (d > maxD)
                {
                    maxD = d;
                }
            }
            return maxD;
        }

        float GetCollisionRadius ()
        {
            float d, maxD = 0;
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                d = Vector3.Distance( theTransform.position, moleculeSimulator.theTransform.position ) + moleculeSimulator.collisionRadius;
                if (d > maxD)
                {
                    maxD = d;
                }
            }
            return maxD;
        }

        float GetDiffusionCoefficient ()
        {
            if (complex.Length == 1)
            {
                return complex[0].molecule.diffusionCoefficient;
            }
            if (complex.Length > 1)
            {
                float d = 0;
                foreach (MoleculeSimulator moleculeSimulator in complex)
                {
                    d += moleculeSimulator.molecule.diffusionCoefficient;
                }
                return d / (0.8f * Mathf.Pow( complex.Length, 2f )); //hack for now
            }
            return 0;
        }

        // complex of MoleculeSimulators is set before Init is called so reactions can be set up correctly
        public virtual void Init (Reactor _reactor)
        {
            reactor = _reactor;

            name = GetSpecies() + name;
            interactionRadius = GetInteractionRadius();
            couldReactOnCollision = GetCouldReactOnCollision();

            particleSimulator = gameObject.AddComponent<ParticleSimulator>();
            particleSimulator.Init( reactor, GetDiffusionCoefficient(), GetCollisionRadius() );

            reactor.RegisterComplex( this );
        }

        public virtual void SpawnMolecules (MoleculeInitData initData)
        {
            complex = new MoleculeSimulator[initData.complexState.moleculeStates.Length];
            for (int i = 0; i < initData.complexState.moleculeStates.Length; i++)
            {
                complex[i] = SpawnMolecule( i, initData );
            }
            ConnectBoundSites();
        }

        protected virtual MoleculeSimulator SpawnMolecule (int i, MoleculeInitData initData)
        {
            GameObject visualizationPrefab = initData.complexState.moleculeStates[i].molecule.visualizationPrefab;
            if (visualizationPrefab == null)
            {
                Debug.LogWarning( initData.complexState.moleculeStates[i].molecule.species + "'s molecule prefab is null!" );
                visualizationPrefab = Resources.Load( "DefaultMolecule" ) as GameObject;
            }

            GameObject moleculeObject = Instantiate( visualizationPrefab );

            moleculeObject.name = name + "_" + initData.complexState.moleculeStates[i].molecule.species;
            moleculeObject.transform.SetParent( theTransform );
            moleculeObject.transform.position = theTransform.TransformPoint( initData.moleculeTransforms[i].position );
            moleculeObject.transform.rotation = theTransform.rotation * Quaternion.Euler( initData.moleculeTransforms[i].rotation );

            MoleculeSimulator moleculeSimulator = moleculeObject.AddComponent<MoleculeSimulator>();
            moleculeSimulator.Init( initData.complexState.moleculeStates[i], this, initData.relevantBimolecularSimulators, initData.relevantCollisionFreeSimulators );

            return moleculeSimulator;
        }

        protected virtual void ConnectBoundSites ()
        {
            Dictionary<string,BindingSiteSimulator> boundBindingSiteSimulators = new Dictionary<string, BindingSiteSimulator>();
            string boundState;
            foreach (MoleculeSimulator moleculeSimulator in complex)
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

        public virtual void SetMolecules (MoleculeSimulator[] _complex, BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                          CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            complex = _complex;
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                moleculeSimulator.MoveToComplex( this, relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            }
        }

        public virtual void InteractWith (ComplexSimulator other)
        {
            if (IsNear( other ))
            {
                complex.Shuffle();
                foreach (MoleculeSimulator moleculeSimulator in complex)
                {
                    if (moleculeSimulator != null && moleculeSimulator.couldReactOnCollision)
                    {
                        other.complex.Shuffle();
                        foreach (MoleculeSimulator otherMoleculeSimulator in other.complex)
                        {
                            if (otherMoleculeSimulator != null && otherMoleculeSimulator.couldReactOnCollision)
                            {
                                if (moleculeSimulator.InteractWith( otherMoleculeSimulator ))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsNear (ComplexSimulator other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
        }

        public MoleculeSimulator[] GetComplexAtEndOfBond (BindingSiteSimulator bindingSiteSimulator)
        {
            // TODO trace complex
            return new MoleculeSimulator[]{bindingSiteSimulator.moleculeSimulator};
        }

        public virtual void UpdateReactions ()
        {
            BimolecularReactionSimulator[] relevantBimolecularSimulators = reactor.GetRelevantBimolecularReactionSimulators( complex );
            CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators = reactor.GetRelevantCollisionFreeReactionSimulators( complex );
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                moleculeSimulator.UpdateReactions( relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            }
            UpdateCouldReactOnCollision();
        }

        public void Remove (MoleculeSimulator moleculeSimulatorToRemove)
        {
            if (complex.Length < 2)
            {
                reactor.UnregisterComplex( this );
                particleSimulator.Destroy();
            }
            else
            {
                MoleculeSimulator[] newComplex = new MoleculeSimulator[complex.Length - 1];
                int j = 0;
                for (int i = 0; i < complex.Length; i++)
                {
                    if (complex[i] != moleculeSimulatorToRemove)
                    {
                        newComplex[j] = complex[i];
                        j++;
                    }
                }
                complex = newComplex;
            }
        }

        protected void UpdateCouldReactOnCollision ()
        {
            bool newCouldReactOnCollision = GetCouldReactOnCollision();
            if (newCouldReactOnCollision != couldReactOnCollision)
            {
                couldReactOnCollision = newCouldReactOnCollision;
                reactor.ComplexChangedCouldReactOnCollisionState( this );
            }
        }

        public override string ToString ()
        {
            return "ComplexSimulator " + name;
        }
    }
}