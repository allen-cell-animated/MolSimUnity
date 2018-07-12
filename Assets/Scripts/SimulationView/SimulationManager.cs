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

        RakNetClient _netClient = null;
        RakNetClient netClient
        {
          get
          {
            if(_netClient == null)
            {
              _netClient = GetComponent<RakNetClient>();
            }
            return _netClient;
          }
        }

        static SimulationManager _Instance;
        public static SimulationManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = GameObject.FindObjectOfType<SimulationManager>();
                }
                return _Instance;
            }
        }

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

        InputManager _inputManager;
        InputManager inputManager
        {
            get
            {
                if (_inputManager == null)
                {
                    _inputManager = GameObject.FindObjectOfType<InputManager>();
                }
                return _inputManager;
            }
        }

        void Start ()
        {
            visualizer.SpawnAgents( StartSimulation() );
            inputManager.CreateCustomUI( modelDef );
        }

        public Dictionary<string,AgentData> StartSimulation ()
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    reactor = (GameObject.Instantiate( Resources.Load( "DefaultReactor" ) as GameObject ) as GameObject).GetComponent<Reactor>();
                    return reactor.Init( modelDef );

                case SimulationType.ExternalSimulator:
                    return netClient.StartActinSimulation();
            }
            return null;
        }

        void Update ()
        {
            visualizer.UpdateAgents( UpdateSimulation() );
        }

        public Dictionary<string,AgentData> UpdateSimulation ()
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    return reactor.GetAgentTransforms();

                case SimulationType.ExternalSimulator:
                    return netClient.UpdateSimulation();
            }
            return null;
        }

        public void SetDT (float dT)
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    World.Instance.observer.currentScale = dT;
                    break;

                case SimulationType.ExternalSimulator:
                    netClient.UpdateTimeStep(dT);
                    break;
            }
        }

        public void SetRateParameter (ReactionDef reactionDef, float rate)
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    reactor.GetReactionForDefinition( reactionDef ).theoreticalRate = rate;
                    break;

                case SimulationType.ExternalSimulator:
                    //TODO
                    break;
            }
        }

        public void Restart ()
        {
            switch (simulationType)
            {
                case SimulationType.AgentSim :
                    reactor.StartCoroutine( "Restart" );
                    break;

                case SimulationType.ExternalSimulator:
                    netClient.StartCoroutine( "Restart" );
                    break;
            }
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

        public AgentData()
        {
          agentName = "Undefined";
          position = new Vector3(0,0,0);
          rotation = new Vector3(0,0,0);
        }
    }
}
