using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class ReactionDef : ScriptableObject
    {
        public string description;
        [Tooltip( "per second" )] 
        public float rate;
        public ComplexPattern[] reactantPatterns;
        public ComplexPattern[] productPatterns;
        public ReactionCenter[] reactionCenters;
        [SerializeField] protected ReactionCenterReference[] reactionCenterReferences;

        public bool isBimolecular
        {
            get
            {
                return reactantPatterns.Length > 1;
            }
        }

        public void Init ()
        {
            if (!ReactantAndProductAmountsAreCorrect())
            {
                Debug.LogWarning( this + " doesn't have the correct amount of reactant and product species for a " + GetType() );
            }
            if (reactionCenterReferences.Length > 2 || reactionCenterReferences.Length < 1)
            {
                Debug.Log( this + " doesn't have the correct number of components in reaction center" );
            }

            #region for prototyping in inspector without writing custom property drawer etc
            foreach (ComplexPattern reactantPattern in reactantPatterns)
            {
                reactantPattern.Init();
            }
            foreach (ComplexPattern productPattern in productPatterns)
            {
                productPattern.Init();
            }
            reactionCenters = new ReactionCenter[reactionCenterReferences.Length];
            for (int i = 0; i < reactionCenters.Length; i++)
            {
                reactionCenters[i] = new ReactionCenter( this, reactionCenterReferences[i] );
            }
            #endregion
        }

        protected abstract bool ReactantAndProductAmountsAreCorrect ();

        protected ComplexPattern GetProductPatternForComponent (MoleculeComponent component)
        {
            for (int i = 0; i < reactionCenters.Length; i++)
            {
                if (reactionCenters[i].reactantComponent.Matches( component ))
                {
                    return reactionCenters[i].productComplex;
                }
            }
            return null;
        }

        public abstract bool React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null);

        protected void SetProductColor (Dictionary<string,List<Molecule>> molecules)
        {
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.SetColorForCurrentState();
                }
            }
        }

        protected void AnimateReaction (Dictionary<string,List<Molecule>> molecules)
        {
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.AnimateReaction();
                }
            }
        }

		public override string ToString()
		{
            return "Reaction " + description;
		}
	}

    [System.Serializable]
    public class ReactionCenterReference
    {
        public ComponentReference reactantReference;
        public ComponentReference productReference;

        public ReactionCenterReference (ComponentReference _reactantReference, ComponentReference _productReference)
        {
            reactantReference = _reactantReference;
            productReference = _productReference;
        }
    }

    [System.Serializable]
    public class ComponentReference
    {
        public int complexIndex;
        public string moleculeName;
        public int moleculeIndex;
        public string componentName;
        public int componentIndex;

        public ComponentReference (int _complexIndex, string _moleculeName, int _moleculeIndex, string _componentName, int _componentIndex)
        {
            complexIndex = _complexIndex;
            moleculeName = _moleculeName;
            moleculeIndex = _moleculeIndex;
            componentName = _componentName;
            componentIndex = _componentIndex;
        }
    }

    [System.Serializable]
    public class ReactionCenter
    {
        public ComplexPattern reactantComplex;
        public MoleculePattern reactantMolecule;
        public ComponentPattern reactantComponent;
        public ComplexPattern productComplex;
        public MoleculePattern productMolecule;
        public ComponentPattern productComponent;

        public ReactionCenter (ReactionDef reactionDef, ReactionCenterReference reference)
        {
            if (reactionDef.reactantPatterns.Length > reference.reactantReference.complexIndex)
            {
                reactantComplex = reactionDef.reactantPatterns[reference.reactantReference.complexIndex];

                if (reactantComplex.moleculePatterns.ContainsKey( reference.reactantReference.moleculeName )
                    && reactantComplex.moleculePatterns[reference.reactantReference.moleculeName].Count > reference.reactantReference.moleculeIndex)
                {
                    reactantMolecule = reactantComplex.moleculePatterns[reference.reactantReference.moleculeName][reference.reactantReference.moleculeIndex];

                    if (reactantMolecule.componentPatterns.ContainsKey( reference.reactantReference.componentName ) 
                        && reactantMolecule.componentPatterns[reference.reactantReference.componentName].Count > reference.reactantReference.componentIndex)
                    {
                        reactantComponent = reactantMolecule.componentPatterns[reference.reactantReference.componentName][reference.reactantReference.componentIndex];
                    }
                }
            }
            if (reactionDef.productPatterns.Length > reference.productReference.complexIndex)
            {
                productComplex = reactionDef.productPatterns[reference.productReference.complexIndex];

                if (productComplex.moleculePatterns.ContainsKey( reference.productReference.moleculeName )
                    && productComplex.moleculePatterns[reference.productReference.moleculeName].Count > reference.productReference.moleculeIndex)
                {
                    productMolecule = productComplex.moleculePatterns[reference.productReference.moleculeName][reference.productReference.moleculeIndex];

                    if (productMolecule.componentPatterns.ContainsKey( reference.productReference.componentName ) 
                        && productMolecule.componentPatterns[reference.productReference.componentName].Count > reference.productReference.componentIndex)
                    {
                        productComponent = productMolecule.componentPatterns[reference.productReference.componentName][reference.productReference.componentIndex];
                    }
                }
            }
        }
    }
}