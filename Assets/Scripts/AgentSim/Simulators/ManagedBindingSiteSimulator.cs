using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ManagedBindingSiteSimulator : BindingSiteSimulator 
    {
        public override void Init (BindingSitePopulation _population, MoleculeSimulator _molecule)
        {
            base.Init( _population, _molecule );
        }

        public override void SimulateFor (float dTime)
        {
            
        }
    }
}