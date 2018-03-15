using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Model : ScriptableObject
    {
        [Tooltip( "Meters" )] 
        public float scale = 1e-6f;
        [Tooltip( "Liters" )] 
        public float containerVolume;
        public ComplexConcentration[] complexes;
        public Reaction[] reactions;

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (ComplexConcentration complex in complexes)
            {
                complex.complexState.Init();
            }
            foreach (Reaction reaction in reactions)
            {
                reaction.Init();
            }
        }
        #endregion
    }

    [System.Serializable]
    public class ComplexConcentration
    {
        [Tooltip( "Molar" )] 
        public float concentration;
        public ComplexState complexState;

        public string species
        {
            get
            {
                return complexState.species;
            }
        }

        public int moleculeCount
        {
            get
            {
                return complexState.moleculeStates.Length;
            }
        }

        public ComplexConcentration (ComplexState _complexState, float _concentration)
        {
            complexState = _complexState;
            complexState.Init();
            concentration = _concentration;
        }
    }
}