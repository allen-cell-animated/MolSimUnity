using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ComplexSnapshot
    {
        [SerializeField] MoleculeSnapshot[] _moleculeSnapshots;
        public MoleculeSnapshot[] moleculeSnapshots
        {
            get
            {
                return _moleculeSnapshots;
            }
        }

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots)
            {
                moleculeSnapshot.Init();
            }
        }
        #endregion

        public ComplexSnapshot (MoleculeSnapshot[] theMoleculeSnapshots)
        {
            _moleculeSnapshots = theMoleculeSnapshots;
        }

        public virtual void SetStateOfComplex (Molecule[] molecules)
        {
            foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots) 
            {
                foreach (Molecule molecule in molecules)
                {
                    if (molecule.definition.Equals( moleculeSnapshot.moleculeDef ))
                    {
                        moleculeSnapshot.SetStateOfMolecule( molecule );
                    }
                }
            }
        }

        public bool IsSatisfiedBy (ComplexSnapshot other)
        {
            foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots)
            {
                bool stateFound = false;
                foreach (MoleculeSnapshot otherMoleculeSnapshot in other.moleculeSnapshots)
                {
                    if (moleculeSnapshot.IsSatisfiedBy( otherMoleculeSnapshot ))
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

        public bool IsSatisfiedBy (Molecule[] molecules)
        {
            foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots)
            {
                bool stateFound = false;
                foreach (Molecule molecule in molecules)
                {
                    if (moleculeSnapshot.IsSatisfiedBy( molecule ))
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
            ComplexSnapshot other = obj as ComplexSnapshot;
            if (other != null)
            {
                if (other.moleculeSnapshots.Length != moleculeSnapshots.Length)
                {
                    return false;
                }
                foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots)
                {
                    bool foundState = false;
                    if (moleculeSnapshot != null && moleculeSnapshot.moleculeDef != null)
                    {
                        foreach (MoleculeSnapshot otherMoleculeSnapshot in other.moleculeSnapshots)
                        {
                            if (otherMoleculeSnapshot != null && otherMoleculeSnapshot.moleculeDef != null
                                && otherMoleculeSnapshot.moleculeDef.Equals( moleculeSnapshot.moleculeDef ))
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
                if (moleculeSnapshots != null)
                {
                    foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots)
                    {
                        hash += 7919 * (moleculeSnapshot == null ? 1 : moleculeSnapshot.GetHashCode());
                    }
                }
                return hash;
            }
        }

        public override string ToString ()
        {
            string s = "";
            int i = 0;
            foreach (MoleculeSnapshot moleculeSnapshot in moleculeSnapshots)
            {
                s += moleculeSnapshot;
                if (i < moleculeSnapshots.Length - 1)
                {
                    s += " | ";
                }
                i++;
            }
            return s;
        }
    }
}
