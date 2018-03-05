using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class BindingSiteSimulator : Simulator 
    {
        BindingSitePopulation population;
        public string state;

        public virtual void Init (BindingSitePopulation _population)
        {
            population = _population;
            state = population.initialState;
        }
    }
}