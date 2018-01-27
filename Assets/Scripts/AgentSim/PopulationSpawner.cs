using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class PopulationSpawner : MonoBehaviour
    {
        public int amount;
        public Agent individualPrefab;

        Agent _agent;
        Agent agent
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

        void Start ()
        {
            SpawnIndividuals();
        }

        protected virtual void SpawnIndividuals ()
        {
            if (individualPrefab == null)
            {
                Debug.LogWarning( name + "'s population spawner's individual prefab is null!" );
                return;
            }
            if (agent.container == null)
            {
                Debug.LogWarning( name + "'s population spawner is not in a container and so can't spawn individuals" );
                return;
            }

            Transform individual;
            for (int i = 0; i < amount; i++)
            {
                individual = Instantiate( individualPrefab, transform ).transform;
                individual.position = agent.container.GetRandomPointInBounds( 0.1f );
                individual.rotation = Random.rotation;
            }
        }
    }
}