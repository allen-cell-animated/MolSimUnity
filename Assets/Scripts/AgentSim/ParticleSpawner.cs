using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSpawner : MonoBehaviour 
    {
        [Tooltip( "Molar" )]
        public float concentration;
        public ParticleSimulator particlePrefab;

        Container _container;
        public Container container
        {
            get
            {
                if (_container == null)
                {
                    _container = GetComponentInParent<Container>();
                }
                return _container;
            }
        }

        int amount
        {
            get
            {
                return Mathf.RoundToInt( concentration * container.volume * 6.022E29f );
            }
        }

        void Start ()
        {
            SpawnParticles();
        }

        protected virtual void SpawnParticles ()
        {
            if (particlePrefab == null)
            {
                Debug.LogWarning( name + "'s ParticleSpawner's particle prefab is null!" );
                return;
            }
            if (container == null)
            {
                Debug.LogWarning( name + "'s ParticleSpawner is not in a Container and so can't spawn particles" );
                return;
            }

            Transform particle;
            for (int i = 0; i < amount; i++)
            {
                particle = Instantiate( particlePrefab, transform ).transform;
                particle.position = container.GetRandomPointInBounds( 0.1f );
                particle.rotation = Random.rotation;
            }
        }
    }
}