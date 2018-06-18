using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.SimulationView
{
    public class Visualizer : MonoBehaviour 
    {
        [SerializeField] AgentPrefab[] agentPrefabs;

        Dictionary<string,GameObject> prefabs;
        Dictionary<string,GameObject> agents = new Dictionary<string,GameObject>();

        public void SpawnAgents (Dictionary<string,AgentData> initialAgents)
        {
            if (initialAgents != null)
            {
                InitPrefabs();

                foreach (string agentID in initialAgents.Keys)
                {
                    agents.Add( agentID, CreateAgentVisualization( agentID, initialAgents[agentID] ) );
                }
            }
        }

        void InitPrefabs ()
        {
            prefabs = new Dictionary<string,GameObject>();
            foreach (AgentPrefab prefab in agentPrefabs)
            {
                if (!prefabs.ContainsKey( prefab.agentName ))
                {
                    prefabs.Add( prefab.agentName, prefab.prefab );
                }
            }
        }

        GameObject CreateAgentVisualization (string agentID, AgentData agentData)
        {
            GameObject obj = null;
            if (!prefabs.ContainsKey( agentData.agentName ) || prefabs[agentData.agentName] == null)
            {
                Debug.LogWarning( "There's no prefab for " + agentData.agentName );
                obj = Instantiate( Resources.Load( "DefaultMolecule" ) as GameObject );
            }
            else 
            {
                obj = Instantiate( prefabs[agentData.agentName] );
            }
            obj.name = agentData.agentName + "_" + agentID;
            obj.transform.SetParent( transform );
            obj.transform.position = agentData.position;
            obj.transform.rotation = Quaternion.Euler( agentData.rotation );
            return obj;
        }

        public void UpdateAgents (Dictionary<string,AgentData> updatedAgents)
        {
            foreach (string id in agents.Keys)
            {
                if (agents.ContainsKey( id ))
                {
                    agents[id].transform.position = updatedAgents[id].position;
                    agents[id].transform.rotation = Quaternion.Euler( updatedAgents[id].rotation );
                }
            }
        }
    }

    [System.Serializable]
    public class AgentPrefab
    {
        public string agentName;
        public GameObject prefab;
    }
}
