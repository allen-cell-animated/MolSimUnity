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
        [SerializeField] protected ComponentReference[] reactionCenterReferences;
        protected ComponentPattern[] reactionCenter;

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
            reactionCenter = new ComponentPattern[reactionCenterReferences.Length];
            for (int i = 0; i < reactionCenter.Length; i++)
            {
                reactionCenter[i] = GetReactantComponentFromReference( reactionCenterReferences[i] );
            }
            #endregion
        }

        protected abstract bool ReactantAndProductAmountsAreCorrect ();

        ComponentPattern GetReactantComponentFromReference (ComponentReference reference)
        {
            if (reactantPatterns.Length > reference.complexIndex)
            {
                ComplexPattern complex = reactantPatterns[reference.complexIndex];
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

        public bool BimolecularReactionCenterIsComponents (MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            return (ComponentMatchesReactionCenter( component1, 0 ) && ComponentMatchesReactionCenter( component2, 1 ))
                || (ComponentMatchesReactionCenter( component1, 1 ) && ComponentMatchesReactionCenter( component2, 0 ));
        }

        public bool ComponentIsInReactionCenter (MoleculeComponent component)
        {
            for (int i = 0; i < reactionCenter.Length; i++)
            {
                if (ComponentMatchesReactionCenter( component, i ))
                {
                    return true;
                }
            }
            return false;
        }

        bool ComponentMatchesReactionCenter (MoleculeComponent component, int reactionCenterIndex)
        {
            return reactionCenter[reactionCenterIndex].MatchesState( component );
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
}