﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Complex : MonoBehaviour 
    {
        public Reactor reactor;
        public Molecule[] molecules;
        public bool couldReactOnCollision;
        public bool readyToBeDestroyed;

        protected Mover mover;
        float interactionRadius;

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
            for (int i = 0; i < molecules.Length; i++)
            {
                if (molecules[i] == null)
                {
                    UnityEditor.EditorApplication.isPaused = true;
                }
                s += molecules[i].definition.moleculeName;
                if (i < molecules.Length - 1)
                {
                    s += ".";
                }
            }
            return s;
        }

        bool GetCouldReactOnCollision ()
        {
            foreach (Molecule molecule in molecules)
            {
                if (molecule.couldReactOnCollision)
                {
                    return true;
                }
            }
            return false;
        }

        float GetInteractionRadius ()
        {
            float d, maxD = 0;
            foreach (Molecule molecule in molecules)
            {
                d = Vector3.Distance( theTransform.position, molecule.theTransform.position ) + molecule.interactionRadius;
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
            foreach (Molecule molecule in molecules)
            {
                d = Vector3.Distance( theTransform.position, molecule.theTransform.position ) + molecule.collisionRadius;
                if (d > maxD)
                {
                    maxD = d;
                }
            }
            return maxD;
        }

        float GetDiffusionCoefficient ()
        {
            if (molecules.Length == 1)
            {
                return molecules[0].definition.diffusionCoefficient;
            }
            if (molecules.Length > 1)
            {
                float d = 0;
                foreach (Molecule molecule in molecules)
                {
                    d += molecule.definition.diffusionCoefficient;
                }
                return d / (0.8f * Mathf.Pow( molecules.Length, 2f )); //hack for now
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

            mover = gameObject.AddComponent<Mover>();
            mover.Init( reactor, GetDiffusionCoefficient(), GetCollisionRadius() );

            reactor.RegisterComplex( this );
        }

        public virtual void SpawnMolecules (MoleculeInitData initData)
        {
            molecules = new Molecule[initData.complexPattern.moleculePatterns.Length];
            for (int i = 0; i < initData.complexPattern.moleculePatterns.Length; i++)
            {
                molecules[i] = SpawnMolecule( i, initData );
            }
            ConnectBoundComponents();
        }

        protected virtual Molecule SpawnMolecule (int i, MoleculeInitData initData)
        {
            GameObject visualizationPrefab = initData.complexPattern.moleculePatterns[i].moleculeDef.visualizationPrefab;
            if (visualizationPrefab == null)
            {
                Debug.LogWarning( initData.complexPattern.moleculePatterns[i].moleculeDef.moleculeName + "'s molecule prefab is null!" );
                visualizationPrefab = Resources.Load( "DefaultMolecule" ) as GameObject;
            }

            GameObject moleculeObject = Instantiate( visualizationPrefab );

            moleculeObject.name = name + "_" + initData.complexPattern.moleculePatterns[i].moleculeDef.moleculeName;
            moleculeObject.transform.SetParent( theTransform );
            moleculeObject.transform.position = theTransform.TransformPoint( initData.moleculeTransforms[i].position );
            moleculeObject.transform.rotation = theTransform.rotation * Quaternion.Euler( initData.moleculeTransforms[i].rotation );

            Molecule molecule = moleculeObject.AddComponent<Molecule>();
            molecule.Init( initData.complexPattern.moleculePatterns[i], this, initData.relevantBimolecularReactions, initData.relevantCollisionFreeReactions );

            return molecule;
        }

        protected virtual void ConnectBoundComponents ()
        {
            Dictionary<string,MoleculeComponent> boundComponents = new Dictionary<string,MoleculeComponent>();
            string boundState;
            foreach (Molecule molecule in molecules)
            {
                foreach (List<MoleculeComponent> aTypeOfComponent in molecule.components.Values)
                {
                    foreach (MoleculeComponent component in aTypeOfComponent)
                    {
                        boundState = component.state;
                        if (boundState.Contains( "!" ))
                        {
                            if (!boundComponents.ContainsKey( boundState ))
                            {
                                boundComponents.Add( boundState, component );
                            }
                            else
                            {
                                boundComponents[boundState].boundComponent = component;
                                component.boundComponent = boundComponents[boundState];
                            }
                        }
                    }
                }
            }
        }

        public virtual void InteractWith (Complex other)
        {
            if (IsNear( other ))
            {
                molecules.Shuffle();
                foreach (Molecule molecule in molecules)
                {
                    if (molecule != null && molecule.couldReactOnCollision)
                    {
                        other.molecules.Shuffle();
                        foreach (Molecule otherMolecule in other.molecules)
                        {
                            if (otherMolecule != null && otherMolecule.couldReactOnCollision)
                            {
                                if (molecule.InteractWith( otherMolecule ))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsNear (Complex other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
        }

        public Molecule[] GetMoleculesAtEndOfBond (MoleculeComponent component)
        {
            // TODO trace complex
            return new Molecule[]{component.molecule};
        }

        public virtual void UpdateReactions ()
        {
            BimolecularReaction[] relevantBimolecularReactions = reactor.GetRelevantBimolecularReactions( molecules );
            CollisionFreeReaction[] relevantCollisionFreeReactions = reactor.GetRelevantCollisionFreeReactions( molecules );
            foreach (Molecule molecule in molecules)
            {
                molecule.UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
            }
            UpdateCouldReactOnCollision();
        }

        protected void UpdateCouldReactOnCollision ()
        {
            bool oldCouldReactOnCollision = couldReactOnCollision;
            couldReactOnCollision = GetCouldReactOnCollision();

            if (couldReactOnCollision != oldCouldReactOnCollision)
            {
                if (!oldCouldReactOnCollision)
                {
                    reactor.RegisterComplex( this );
                }
            }
        }

        public void RemoveMolecule (Molecule moleculeToRemove)
        {
            if (molecules.Length < 2)
            {
                readyToBeDestroyed = true;
                mover.Destroy();
            }
            else
            {
                Molecule[] newMolecules = new Molecule[molecules.Length - 1];
                int j = 0;
                for (int i = 0; i < molecules.Length; i++)
                {
                    if (molecules[i] != moleculeToRemove)
                    {
                        newMolecules[j] = molecules[i];
                        j++;
                    }
                }
                molecules = newMolecules;
            }
        }

        public override string ToString ()
        {
            return "Complex " + name;
        }
    }
}