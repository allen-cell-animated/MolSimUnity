using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Complex : MonoBehaviour 
    {
        public Reactor reactor;
        public Dictionary<string,List<Molecule>> molecules = new Dictionary<string,List<Molecule>>();
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
            int i = 0;
            foreach (string moleculeName in molecules.Keys)
            {
                s += moleculeName + molecules[moleculeName].Count;
                if (i < molecules.Keys.Count - 1)
                {
                    s += ".";
                }
                i++;
            }
            return s;
        }

        bool GetCouldReactOnCollision ()
        {
            foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
            {
                foreach (Molecule molecule in aTypeOfMolecule)
                {
                    if (molecule.couldReactOnCollision)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        float GetInteractionRadius ()
        {
            float d, maxD = 0;
            foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
            {
                foreach (Molecule molecule in aTypeOfMolecule)
                {
                    d = Vector3.Distance( theTransform.position, molecule.theTransform.position ) + molecule.interactionRadius;
                    if (d > maxD)
                    {
                        maxD = d;
                    }
                }
            }
            return maxD;
        }

        float GetCollisionRadius ()
        {
            float d, maxD = 0;
            foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
            {
                foreach (Molecule molecule in aTypeOfMolecule)
                {
                    d = Vector3.Distance( theTransform.position, molecule.theTransform.position ) + molecule.collisionRadius;
                    if (d > maxD)
                    {
                        maxD = d;
                    }
                }
            }
            return maxD;
        }

        float GetDiffusionCoefficient ()
        {
            int n = GetNumberOfMolecules();
            if (n == 1)
            {
                foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
                {
                    if (aTypeOfMolecule.Count > 0)
                    {
                        return aTypeOfMolecule[0].definition.diffusionCoefficient;
                    }
                }
            }
            if (n > 1)
            {
                float d = 0;
                foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
                {
                    foreach (Molecule molecule in aTypeOfMolecule)
                    {
                        d += molecule.definition.diffusionCoefficient;
                    }
                }
                return d / (0.8f * Mathf.Pow( n, 2f )); //hack for now
            }
            return 0;
        }

        // complex of MoleculeSimulators is set before Init is called so reactions can be set up correctly
        public virtual void Init (Reactor _reactor)
        {
            reactor = _reactor;

            name = GetSpecies() + "_" + name;
            interactionRadius = GetInteractionRadius();

            mover = gameObject.AddComponent<Mover>();
            mover.Init( reactor, GetDiffusionCoefficient(), GetCollisionRadius() );

            couldReactOnCollision = GetCouldReactOnCollision();
            reactor.RegisterComplex( this );
        }

        public virtual void SpawnMolecules (MoleculeInitData initData)
        {
            molecules = new Dictionary<string,List<Molecule>>();
            foreach (string moleculeName in initData.complexPattern.moleculePatterns.Keys)
            {
                if (!molecules.ContainsKey( moleculeName ))
                {
                    molecules.Add( moleculeName, new List<Molecule>() );
                }
                for (int i = 0; i < initData.complexPattern.moleculePatterns[moleculeName].Count; i++)
                {
                    molecules[moleculeName].Add( SpawnMolecule( initData.complexPattern.moleculePatterns[moleculeName][i], initData.moleculeTransforms[moleculeName][i] ) );
                }
            }
            ConnectBoundComponents();
            InitReactions( initData.relevantBindReactions, initData.relevantCollisionFreeReactions );
        }

        protected void InitReactions (BindReaction[] relevantBindReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.UpdateReactions( relevantBindReactions, relevantCollisionFreeReactions );
                }
            }
            UpdateCouldReactOnCollision();
        }

        protected virtual Molecule SpawnMolecule (MoleculePattern moleculePattern, RelativeTransform relativeTransform)
        {
            GameObject visualizationPrefab = moleculePattern.moleculeDef.visualizationPrefab;
            if (visualizationPrefab == null)
            {
                Debug.LogWarning( moleculePattern.moleculeDef.moleculeName + "'s molecule prefab is null!" );
                visualizationPrefab = Resources.Load( "DefaultMolecule" ) as GameObject;
            }

            GameObject moleculeObject = Instantiate( visualizationPrefab );

            moleculeObject.name = name + "_" + moleculePattern.moleculeDef.moleculeName;
            moleculeObject.transform.SetParent( theTransform );
            moleculeObject.transform.position = theTransform.TransformPoint( relativeTransform.position );
            moleculeObject.transform.rotation = theTransform.rotation * Quaternion.Euler( relativeTransform.rotation );

            Molecule molecule = moleculeObject.AddComponent<Molecule>();
            molecule.Init( moleculePattern, this );

            return molecule;
        }

        protected virtual void ConnectBoundComponents ()
        {
            Dictionary<string,MoleculeComponent> boundComponents = new Dictionary<string,MoleculeComponent>();
            foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
            {
                foreach (Molecule molecule in aTypeOfMolecule)
                {
                    foreach (List<MoleculeComponent> aTypeOfComponent in molecule.components.Values)
                    {
                        foreach (MoleculeComponent component in aTypeOfComponent)
                        {
                            if (!string.IsNullOrEmpty( component.lastBondName ) && !component.lastBondName.Contains( "+" ))
                            {
                                if (!boundComponents.ContainsKey( component.lastBondName ))
                                {
                                    boundComponents.Add( component.lastBondName, component );
                                }
                                else
                                {
                                    boundComponents[component.lastBondName].boundComponent = component;
                                    component.boundComponent = boundComponents[component.lastBondName];
                                    component.lastBondName = component.boundComponent.lastBondName = "+";
                                }
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
                foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
                {
                    aTypeOfMolecule.Shuffle();
                    foreach (Molecule molecule in aTypeOfMolecule)
                    {
                        if (molecule != null && molecule.couldReactOnCollision)
                        {
                            foreach (List<Molecule> aTypeOfMoleculeOther in other.molecules.Values)
                            {
                                aTypeOfMoleculeOther.Shuffle();
                                foreach (Molecule otherMolecule in aTypeOfMoleculeOther)
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
            }
        }

        bool IsNear (Complex other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
        }

        public Dictionary<string,List<Molecule>> GetMoleculesAtEndOfBond (MoleculeComponent component)
        {
            // TODO trace complex
            Dictionary<string,List<Molecule>> tracedMolecules = new Dictionary<string, List<Molecule>>();
            tracedMolecules.Add( component.molecule.definition.moleculeName, new List<Molecule>( new Molecule[]{component.molecule} ) );
            return tracedMolecules;
        }

        public void SetToProductState (ReactionCenter reactionCenter)
        {
            reactionCenter.productComplex.SetStateOfComplex( molecules );
            ConnectBoundComponents();
        }

        public void SetToProductState (ComplexPattern productComplex, Molecule molecule1, MoleculePattern productMolecule1, Molecule molecule2, MoleculePattern productMolecule2)
        {
            productComplex.SetStateOfComplex( molecule1, productMolecule1, molecule2, productMolecule2 );
            ConnectBoundComponents();
        }

        public virtual void UpdateReactions ()
        {
            BindReaction[] relevantBindReactions = reactor.GetRelevantBindReactions( molecules );
            CollisionFreeReaction[] relevantCollisionFreeReactions = reactor.GetRelevantCollisionFreeReactions( molecules );
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.UpdateReactions( relevantBindReactions, relevantCollisionFreeReactions );
                }
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
            List<Molecule> oldMoleculesOfThisType = new List<Molecule>( molecules[moleculeToRemove.definition.moleculeName] );
            molecules[moleculeToRemove.definition.moleculeName].Clear();

            for (int i = 0; i < oldMoleculesOfThisType.Count; i++)
            {
                if (oldMoleculesOfThisType[i] != moleculeToRemove)
                {
                    molecules[moleculeToRemove.definition.moleculeName].Add( oldMoleculesOfThisType[i] );
                }
            }
            
            if (GetNumberOfMolecules() < 1)
            {
                readyToBeDestroyed = true;
                mover.Destroy();
            }
        }

        public override string ToString ()
        {
            return "Complex " + name;
        }

        public int GetNumberOfMolecules ()
        {
            int n = 0;
            foreach (List<Molecule> aTypeOfMolecule in molecules.Values)
            {
                n += aTypeOfMolecule.Count;
            }
            return n;
        }
    }
}