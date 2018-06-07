using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule 
    {
        public string agentID;
        public Complex complex;
        public MoleculeDef definition;
        public Dictionary<string,List<MoleculeComponent>> components = new Dictionary<string,List<MoleculeComponent>>();
        public float collisionRadius;
        public float interactionRadius;
        public bool couldReactOnCollision;
        public bool stateWasUpdated;
        public RelativeTransform localTransform;

        public RelativeTransform worldTransform
        {
            get
            {
                return complex.reactor.GetWorldTransform( complex.position, complex.rotation, localTransform );
            }
        }

        public Vector3 position
        {
            get
            {
                return worldTransform.position;
            }
        }

        public Vector3 rotation
        {
            get
            {
                return worldTransform.rotation;
            }
        }

        public void SetWorldTransform (RelativeTransform worldTransform)
        {
            complex.SetWorldTransform( complex.reactor.GetParentWorldTransform( worldTransform, localTransform ) );
        }

        bool GetCouldReactOnCollision ()
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    if (component.couldReactOnCollision)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Molecule (MoleculePattern moleculePattern, RelativeTransform _localTransform, Complex _complex)
        {
            localTransform = new RelativeTransform( _localTransform );
            complex = _complex;
            definition = moleculePattern.moleculeDef;
            collisionRadius = interactionRadius = definition.radius;
            interactionRadius += 1f;
            CreateComponents( moleculePattern );
            SetColorForCurrentState();
        }

        protected void CreateComponents (MoleculePattern moleculePattern)
        {
            foreach (List<ComponentDef> aTypeOfComponent in definition.componentDefs.Values)
            {
                foreach (ComponentDef componentDef in aTypeOfComponent)
                {
                    CreateComponent( componentDef );
                }
            }
            moleculePattern.SetStateOfMolecule( this );
        }

        protected void CreateComponent (ComponentDef componentDef)
        {
            if (!components.ContainsKey( componentDef.componentName ))
            {
                components.Add( componentDef.componentName, new List<MoleculeComponent>() );
            }
            components[componentDef.componentName].Add( new MoleculeComponent( componentDef, this ) );
        }

        public virtual bool InteractWith (Molecule other)
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    if (component.couldReactOnCollision)
                    {
                        foreach (List<MoleculeComponent> aTypeOfComponentOther in other.components.Values)
                        {
                            foreach (MoleculeComponent otherComponent in aTypeOfComponentOther)
                            {
                                if (otherComponent.couldReactOnCollision && component.ReactWith( otherComponent ))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public virtual void MoveToComplex (Complex _complex)
        {
            RelativeTransform _worldTransform = new RelativeTransform( worldTransform );

            complex.RemoveMolecule( this );
            complex = _complex;

            localTransform = complex.reactor.GetLocalTransform( complex.position, complex.rotation, _worldTransform );
        }

        public void SetToProductState (MoleculePattern productMoleculePattern)
        {
            productMoleculePattern.SetStateOfMolecule( this );
            stateWasUpdated = true;
        }

        public virtual void UpdateReactions (BindReaction[] relevantBindReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    component.UpdateReactions( relevantBindReactions, relevantCollisionFreeReactions );
                }
            }
            couldReactOnCollision = GetCouldReactOnCollision();
        }

        public void SetColorForCurrentState ()
        {
            if (definition.colors != null)
            {
                foreach (MoleculePatternColor moleculeColor in definition.colors)
                {
                    if (moleculeColor.pattern.Matches( this ))
                    {
                        complex.reactor.ChangeColor( agentID, moleculeColor.color );
                        return;
                    }
                }
            }
        }

        public override string ToString ()
        {
            string s = "[" + definition.moleculeName + ":";
            int i = 0;
            int n = GetNumberOfComponents();
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    s += component;
                    if (i < n - 1)
                    {
                        s += ",";
                    }
                    i++;
                }
            }
            return s + "]";
        }

        int GetNumberOfComponents ()
        {
            int n = 0;
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                n += aTypeOfComponent.Count;
            }
            return n;
        }
	}
}