using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.AgentSim;

namespace AICS.SimulationView
{
    public enum SimulationType
    {
        AgentSim,
        ExternalSimulator
    }

    [RequireComponent( typeof(Visualizer) )]
    public class SimulationManager : MonoBehaviour 
    {
        public SimulationType simulationType;
        public ModelDef modelDef;

        Reactor reactor;

        Visualizer _visualizer;
        Visualizer visualizer
        {
            get
            {
                if (_visualizer == null)
                {
                    _visualizer = GetComponent<Visualizer>();
                }
                return _visualizer;
            }
        }

        void Start ()
        {
            visualizer.SpawnAgents( StartSimulation() );
        }

        public Dictionary<string,AgentData> StartSimulation ()
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    reactor = (GameObject.Instantiate( Resources.Load( "DefaultReactor" ) as GameObject ) as GameObject).GetComponent<Reactor>();
                    return reactor.Init( modelDef );

                case SimulationType.ExternalSimulator:
                    return new Dictionary<string,AgentData>();
            }
            return null;
        }

        void Update ()
        {
            //visualizer.UpdateAgents(  );
        }

        public Dictionary<string,AgentData> UpdateSimulation ()
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    return reactor.Init( modelDef );

                case SimulationType.ExternalSimulator:
                    return new Dictionary<string,AgentData>();
            }
            return null;
        }
    }

    public class AgentData 
    {
        public string agentName;
        public Vector3 position;
        public Vector3 rotation;

        public AgentData (string _agentName, Vector3 _position, Vector3 _rotation)
        {
            agentName = _agentName;
            position = _position;
            rotation = _rotation;
        }
    }
}