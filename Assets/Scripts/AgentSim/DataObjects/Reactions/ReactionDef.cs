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
        [SerializeField] protected ComplexPattern[] reactantPatterns;
        [Tooltip( "for now, molecules should be in same order in products" )]
        [SerializeField] protected ComplexPattern[] productPatterns;
        //public BindingSiteReference[] relevantSiteReferences;
        [SerializeField] protected ReactionCenterReference[] reactionCenterReferences;
        protected ReactionCenter[] reactionCenters;

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
                reactionCenters[i] = new ReactionCenter( GetComponentFromReference( reactionCenterReferences[i].reactantReference, true ),
                                                         GetComponentFromReference( reactionCenterReferences[i].productReference, false ) );
            }
            #endregion
        }

        protected abstract bool ReactantAndProductAmountsAreCorrect ();

        ComponentPattern GetComponentFromReference (ComponentReference reference, bool reactant)
        {
            ComplexPattern[] patterns = reactant ? reactantPatterns : productPatterns;
            if (patterns.Length > reference.complexIndex)
            {
                ComplexPattern complex = patterns[reference.complexIndex];
                if (complex.moleculePatterns.Length > reference.moleculeIndex)
                {
                    MoleculePattern molecule = complex.moleculePatterns[reference.moleculeIndex];
                    if (molecule.components.ContainsKey( reference.componentName ) && molecule.components[reference.componentName].Count > reference.componentIndex)
                    {
                        return molecule.components[reference.componentName][reference.componentIndex];
                    }
                }
            }
            return null;
        }

        public bool ReactantsEqual (Molecule[] molecules1, Molecule[] molecules2)
        {
            return ((reactantPatterns[0].Matches( molecules1 ) && reactantPatterns[1].Matches( molecules2 )))
                 || (reactantPatterns[0].Matches( molecules2 ) && reactantPatterns[1].Matches( molecules1 ));

        }

        public bool ComplexIsReactant (ComplexPattern complexPattern)
        {
            foreach (ComplexPattern reactantPattern in reactantPatterns)
            {
                if (reactantPattern.Matches( complexPattern ))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ComplexIsReactant (Molecule[] molecules)
        {
            foreach (ComplexPattern reactantPattern in reactantPatterns)
            {
                if (reactantPattern.Matches( molecules ))
                {
                    return true;
                }
            }
            return false;
        }

        public bool BimolecularReactionCenterReactantsAreComponents (MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            return (ComponentMatchesReactantInReactionCenter( component1, 0 ) && ComponentMatchesReactantInReactionCenter( component2, 1 ))
                || (ComponentMatchesReactantInReactionCenter( component1, 1 ) && ComponentMatchesReactantInReactionCenter( component2, 0 ));
        }

        public bool ComponentIsReactantInReactionCenter (MoleculeComponent component)
        {
            for (int i = 0; i < reactionCenters.Length; i++)
            {
                if (ComponentMatchesReactantInReactionCenter( component, i ))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ComponentMatchesReactantInReactionCenter (MoleculeComponent component, int reactionCenterIndex)
        {
            return reactionCenters[reactionCenterIndex].reactant.MatchesState( component );
        }

        public abstract void React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null);

        protected void SetProductColor (Molecule[] molecules)
        {
            foreach (Molecule molecule in molecules)
            {
                molecule.SetColorForCurrentState();
            }
        }

        protected void AnimateReaction (Molecule[] molecules)
        {
            foreach (Molecule molecule in molecules)
            {
                molecule.AnimateReaction();
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
        public int moleculeIndex;
        public string componentName;
        public int componentIndex;

        public ComponentReference (int _complexIndex, int _moleculeIndex, string _componentName, int _componentIndex)
        {
            complexIndex = _complexIndex;
            moleculeIndex = _moleculeIndex;
            componentName = _componentName;
            componentIndex = _componentIndex;
        }
    }

    [System.Serializable]
    public class ReactionCenter
    {
        public ComponentPattern reactant;
        public ComponentPattern product;

        public ReactionCenter (ComponentPattern _reactant, ComponentPattern _product)
        {
            reactant = _reactant;
            product = _product;
        }
    }
}