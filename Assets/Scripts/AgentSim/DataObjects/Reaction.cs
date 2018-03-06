using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reaction : ScriptableObject
    {
        public string description;
        [Tooltip( "per second" )] 
        public float rate;
        public IReactable[] reactants;
        public IReactable[] products;

        #region for prototyping in inspector without writing custom property drawer etc
        public MoleculeState[] singleReactants;
        public CompoundMoleculeState[] compoundReactants;
        public MoleculeState[] singleProducts;
        public CompoundMoleculeState[] compoundProducts;
        public MoleculeBindingSite[] relevantSites;

        public void Init ()
        {
            foreach (MoleculeState singleMolecule in singleReactants)
            {
                singleMolecule.Init();
            }
            reactants = new IReactable[singleReactants.Length + compoundReactants.Length];
            singleReactants.CopyTo( reactants, 0 );
            compoundReactants.CopyTo( reactants, singleReactants.Length );

            foreach (MoleculeState singleMolecule in singleProducts)
            {
                singleMolecule.Init();
            }
            products = new IReactable[singleProducts.Length + compoundProducts.Length];
            singleProducts.CopyTo( products, 0 );
            compoundProducts.CopyTo( products, singleProducts.Length );
        }
        #endregion

        public Reaction (IReactable[] _reactants, IReactable[] _products, float _rate, string _description = "")
        {
            description = _description;
            reactants = _reactants;
            products = _products;
            rate = _rate;
        }
    }

    [System.Serializable]
    public class MoleculeBindingSite
    {
        public Molecule molecule;
        public string bindingSiteID;

        public MoleculeBindingSite (Molecule _molecule, string _bindingSiteID)
        {
            molecule = _molecule;
            bindingSiteID = _bindingSiteID;
        }
    }

    [System.Serializable]
    public class MoleculeState : IReactable
    {
        public Molecule molecule;
        public Dictionary<string,string> bindingSiteStates;

        #region for prototyping in inspector without writing custom property drawer etc
        public SiteState[] siteStates;

        public void Init ()
        {
            bindingSiteStates = new Dictionary<string,string>();
            foreach (SiteState siteState in siteStates)
            {
                bindingSiteStates.Add( siteState.id, siteState.state );
            }
        }
        #endregion

        public MoleculeState (Molecule _molecule, Dictionary<string,string> _bindingSiteStates)
        {
            molecule = _molecule;
            bindingSiteStates = _bindingSiteStates;
        }

        public bool Matches (IReactable other)
        {
            MoleculeState otherSingleMoleculeState = other as MoleculeState;
            if (otherSingleMoleculeState != null && otherSingleMoleculeState.molecule.species == molecule.species)
            {
                foreach (KeyValuePair<string,string> otherComponent in otherSingleMoleculeState.bindingSiteStates)
                {
                    if (bindingSiteStates[otherComponent.Key] != otherComponent.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    //for prototyping in inspector without writing custom property drawer etc
    [System.Serializable]
    public class SiteState
    {
        public string id;
        public string state;
    }

    [System.Serializable]
    public class CompoundMoleculeState : IReactable
    {
        public MoleculeState parentMoleculeState;
        public MoleculeState childMoleculeState;

        public CompoundMoleculeState (MoleculeState _parentMoleculeState, MoleculeState _childMoleculeState)
        {
            parentMoleculeState = _parentMoleculeState;
            childMoleculeState = _childMoleculeState;
        }

        public bool Matches (IReactable other)
        {
            CompoundMoleculeState otherCompoundMoleculeState = other as CompoundMoleculeState;
            return otherCompoundMoleculeState != null 
                && otherCompoundMoleculeState.parentMoleculeState.Matches( parentMoleculeState )
                && otherCompoundMoleculeState.childMoleculeState.Matches( childMoleculeState );
        }
    }
}