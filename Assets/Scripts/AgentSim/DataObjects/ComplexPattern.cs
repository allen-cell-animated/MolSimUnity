using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ComplexPattern
    {
        [SerializeField] MoleculePattern[] _moleculePatterns;
        public MoleculePattern[] moleculePatterns
        {
            get
            {
                return _moleculePatterns;
            }
        }

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (MoleculePattern moleculePattern in moleculePatterns)
            {
                moleculePattern.Init();
            }
        }
        #endregion

        public ComplexPattern (MoleculePattern[] theMoleculePatterns)
        {
            _moleculePatterns = theMoleculePatterns;
        }

        public virtual void SetStateOfComplex (Molecule[] molecules)
        {
            foreach (MoleculePattern moleculePattern in moleculePatterns) 
            {
                foreach (Molecule molecule in molecules)
                {
                    if (molecule.definition.Equals( moleculePattern.moleculeDef ))
                    {
                        moleculePattern.SetStateOfMolecule( molecule );
                    }
                }
            }
        }

        public bool Matches (ComplexPattern other)
        {
            foreach (MoleculePattern moleculePattern in moleculePatterns)
            {
                bool stateFound = false;
                foreach (MoleculePattern otherMoleculePattern in other.moleculePatterns)
                {
                    if (moleculePattern.Matches( otherMoleculePattern ))
                    {
                        stateFound = true;
                        break;
                    }
                }
                if (!stateFound)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Matches (Molecule[] molecules)
        {
            foreach (MoleculePattern moleculePattern in moleculePatterns)
            {
                bool stateFound = false;
                foreach (Molecule molecule in molecules)
                {
                    if (moleculePattern.Matches( molecule ))
                    {
                        stateFound = true;
                        break;
                    }
                }
                if (!stateFound)
                {
                    return false;
                }
            }
            return true;
        }

		public override bool Equals (object obj)
		{
            ComplexPattern other = obj as ComplexPattern;
            if (other != null)
            {
                if (other.moleculePatterns.Length != moleculePatterns.Length)
                {
                    return false;
                }
                foreach (MoleculePattern moleculePattern in moleculePatterns)
                {
                    bool foundState = false;
                    if (moleculePattern != null && moleculePattern.moleculeDef != null)
                    {
                        foreach (MoleculePattern otherMoleculePattern in other.moleculePatterns)
                        {
                            if (otherMoleculePattern != null && otherMoleculePattern.moleculeDef != null
                                && otherMoleculePattern.moleculeDef.Equals( moleculePattern.moleculeDef ))
                            {
                                foundState = true;
                                break;
                            }
                        }
                    }
                    if (!foundState)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                int hash = 0;
                if (moleculePatterns != null)
                {
                    foreach (MoleculePattern moleculePattern in moleculePatterns)
                    {
                        hash += 7919 * (moleculePattern == null ? 1 : moleculePattern.GetHashCode());
                    }
                }
                return hash;
            }
        }

        public override string ToString ()
        {
            string s = "";
            int i = 0;
            foreach (MoleculePattern moleculePattern in moleculePatterns)
            {
                s += moleculePattern;
                if (i < moleculePatterns.Length - 1)
                {
                    s += " | ";
                }
                i++;
            }
            return s;
        }
    }
}
