using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public interface IReactable
    {
        bool Matches (MoleculeSimulator _molecule);
    }
}