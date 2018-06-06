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
                    agents.Add( agentID, CreateAgentVisualization( initialAgents[agentID] ) );
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

        GameObject CreateAgentVisualization (AgentData agentData)
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
            obj.transform.SetParent( transform );
            obj.transform.position = agentData.position;
            obj.transform.rotation = Quaternion.Euler( agentData.rotation );
            return obj;
        }

        public void UpdateAgents (Dictionary<string,Transform> newTransforms)
        {
            foreach (string id in newTransforms.Keys)
            {
                if (agents.ContainsKey( id ))
                {
                    agents[id].transform.position = newTransforms[id].position;
                    agents[id].transform.rotation = newTransforms[id].rotation;
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
