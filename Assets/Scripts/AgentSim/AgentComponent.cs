using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [RequireComponent( typeof(Agent) )]
    public class AgentComponent : MonoBehaviour 
    {
        Agent _agent;
        public Agent agent
        {
            get
            {
                if (_agent == null)
                {
                    _agent = GetComponent<Agent>();
                }
                return _agent;
            }
        }
    }
}