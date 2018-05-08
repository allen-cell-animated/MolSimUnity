using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public enum ReactionType
    {
        Bind,
        Release,
        StateChange,
        Destroy,
        Create
    }

    public class ReactionDef : ScriptableObject
    {
        public string description;
        [Tooltip( "per second" )] 
        public float rate;
        public ReactionType type;
        [Tooltip( "There must be one or two reaction centers" )] 
        public ReactionCenter[] reactionCenters;

        public void Init ()
        {
            foreach (ReactionCenter reactionCenter in reactionCenters)
            {
                reactionCenter.Init();
            }
        }

		public override string ToString()
		{
            return "Reaction " + description;
		}
	}

    [System.Serializable]
    public class ReactionCenter
    {
        public Reaction reaction;
        public string description; //for debugging in editor
        public ComplexPattern reactantComplex;
        public MoleculePattern reactantMolecule;
        public ComponentPattern reactantComponent;
        public ComplexPattern productComplex;
        public MoleculePattern productMolecule;
        public ComponentPattern productComponent;

        public void Init ()
        {
            reactantComplex.Init();
            reactantMolecule.Init();
            productComplex.Init();
            productMolecule.Init();
        }
    }
}