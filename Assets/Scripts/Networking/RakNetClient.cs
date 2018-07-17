using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RakNet;

public class RakNetClient : MonoBehaviour {

	/**
	*	This needs to correspond to the values defined in the C++
	*	server program
	*/
	public enum messageId {
		ID_VIS_DATA_ARRIVE = DefaultMessageIDTypes.ID_USER_PACKET_ENUM,
		ID_VIS_DATA_REQUEST,
		ID_VIS_DATA_FINISH,
		ID_VIS_DATA_PAUSE,
		ID_VIS_DATA_RESUME,
		ID_VIS_DATA_ABORT,
		ID_UPDATE_TIME_STEP,
		ID_UPDATE_RATE_PARAM
	};

	/**
	*	What simulator should the C++ server use to evaluate
	*	agent behavior in this simulation?
	*/
	public enum simulator {
		DevTest = 1,
		Readdy
	};

	public enum visClientState {
		NotStreaming = 1,
		Paused,
		Streaming
	}

	/**
	*	Contains all the data needed to request a simulation from
	*	the C++ server
	*/
	struct SimRequestData {
		public SimRequestData(simulator s, float tSteps, float stepSize)
		{
			Simulator = s;
			nTimeSteps = tSteps;
			timeStepSize = stepSize;
		}

		public simulator Simulator;
		public float nTimeSteps;
		public float timeStepSize;
	};

	public struct NetAgentData
	{
		public float type;
		public float x, y, z;
		public float xrot, yrot, zrot;
	};

	/**
	*	Sending numerical values over the network is less hassle than sending
	*	full string names. This maps entities to an ID used to identify
	* a client side actor over the network.
	*
	*	The C++ simulator doesn't particularly care whether actin is defined
	* as an "actin" or a "type == 1" so long as the parameters are correct
	*/
	private Dictionary<int, string> m_agentMapping =
		new Dictionary<int, string>();

	Packet m_packet;
	RakPeerInterface m_client;

	public bool ToggleStateManually = false;
	public visClientState ClientState;

	/**
	*	Simulation parameters editable in the Unity Editor
	*/
	public int NumberOfTimeSteps = -1;
	public float StepSize = 1e-9f;

	private bool m_Connected = false;
	private visClientState m_ClientState = visClientState.NotStreaming;
	private RakNet.BitStream m_recieved_data;
	private RakNet.BitStream m_sent_data;

	SystemAddress serverAddr;

	public List<NetAgentData> AGENT_LIST = new List<NetAgentData>();

	// Use this for initialization
	void Start () {
		TryInit();
	}

	public void TryInit()
	{
		if(this.m_Connected)
		{
			return;
		}
		serverAddr = new SystemAddress("127.0.0.1", 60000);

		this.m_recieved_data = new RakNet.BitStream();
		this.m_sent_data = new RakNet.BitStream();

		this.m_client = RakPeer.GetInstance();
		this.m_client.Startup(1, new SocketDescriptor(), 1);
		this.m_client.Connect("127.0.0.1", 60000, "", 0);
		Debug.Log("Starting Client");
		this.m_Connected = true;
	}

	// Update is called once per frame
	void Update () {
		// Pause for the world instance
		if(AICS.AgentSim.World.Instance.paused &&
				this.m_ClientState == visClientState.Streaming)
		{
			SetState(visClientState.Paused);
		}

		// Resume for the world isntance
		if(!AICS.AgentSim.World.Instance.paused &&
				this.m_ClientState == visClientState.Paused)
		{
			SetState(visClientState.Streaming);
		}

		if(ToggleStateManually && ClientState != m_ClientState)
		{
			SetState(ClientState);
		}

		this.m_packet = this.m_client.Receive();
		if(this.m_packet == null)
		{
			return;
		}

		switch(this.m_packet.data[0])
		{
			case (byte)DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
			{
				Debug.Log("Our connection request has been accepted");
			}break;
			case (byte)messageId.ID_VIS_DATA_ARRIVE:
			{
				this.m_recieved_data.Reset();;
				this.m_recieved_data.Write(this.m_packet.data, this.m_packet.length);
				DeserializeSimData(ref this.m_recieved_data, ref this.AGENT_LIST);
			} break;
			case (byte)messageId.ID_VIS_DATA_FINISH:
			{
				Debug.Log("Request simulation data has finished");
				this.m_ClientState = visClientState.NotStreaming;
				ClientState = visClientState.NotStreaming;
			} break;
			default:
			{
					Debug.Log(
						String.Format(
							"Message with identifier {0} has arrived", this.m_packet.data[0]));
			} break;
		}
	}

	void OnDestroy()
	{
		Debug.Log("Destroying Client");
		RakPeerInterface.DestroyInstance(this.m_client);
	}

	/**
	*	The below functions relate to managing network
	*	simulation streaming state
	*/
	public bool IsStreamingSimulation()
	{
		return this.m_ClientState == visClientState.Streaming;
	}

	public void SetState(visClientState newState)
	{
		switch(newState)
		{
			case visClientState.NotStreaming:
			{
				if (this.m_ClientState == visClientState.Paused ||
				 		this.m_ClientState == visClientState.Streaming)
				{
					Debug.Log("Sending abort request");
					this.m_sent_data.Reset();
					this.m_sent_data.Write((byte)messageId.ID_VIS_DATA_ABORT);
					this.m_client.Send(
						this.m_sent_data, PacketPriority.HIGH_PRIORITY,
						PacketReliability.RELIABLE_ORDERED,
						(char)0, new AddressOrGUID(serverAddr), false);
				}
			} break;
			case visClientState.Streaming:
			{
				/**
				*	Resume streaming simulation ;results
				*/
				if (this.m_ClientState == visClientState.Paused)
				{
					Debug.Log("Sending resume request");
					this.m_sent_data.Reset();
					this.m_sent_data.Write((byte)messageId.ID_VIS_DATA_RESUME);
					this.m_client.Send(
						this.m_sent_data, PacketPriority.HIGH_PRIORITY,
						PacketReliability.RELIABLE_ORDERED,
						(char)0, new AddressOrGUID(serverAddr), false);
				}

				/**
				*	Start streaming simulation results
				*/
				if (this.m_ClientState == visClientState.NotStreaming)
				{
					Debug.Log("Sending data request");
					SimRequestData sr = new SimRequestData(
						simulator.DevTest,
						this.NumberOfTimeSteps,
						this.StepSize);
					SerializeVisDataRequest(ref this.m_sent_data, sr);

					this.m_client.Send(
						this.m_sent_data, PacketPriority.HIGH_PRIORITY,
						PacketReliability.RELIABLE_ORDERED,
						(char)0, new AddressOrGUID(serverAddr), false);

					Debug.Log(String.Format("Num Time Steps: {0}", sr.nTimeSteps));
					Debug.Log(String.Format("Time Step Size: {0}", sr.timeStepSize));
				}
			} break;
			case visClientState.Paused:
			{
				/**
				*	Pause streaming simulation
				*/
				if(this.m_ClientState == visClientState.Streaming)
				{
					Debug.Log("Sending pause request");
					this.m_sent_data.Reset();
					this.m_sent_data.Write((byte)messageId.ID_VIS_DATA_PAUSE);
					this.m_client.Send(
						this.m_sent_data, PacketPriority.HIGH_PRIORITY,
						PacketReliability.RELIABLE_ORDERED,
						(char)0, new AddressOrGUID(serverAddr), false);
				}
				else
				{
					// It doesn't make sense to pause a Simulation
					// that is anything other than currently running
					return;
				}
			} break;
			default:
			{
				Debug.Log(
					String.Format(
						"Unrecognized client state {0} requested in net client", newState));
			} break;
		}

		this.m_ClientState = newState;
	}

	/**
	*	Client side visualization functions
	*/
	private Dictionary<string, AICS.SimulationView.AgentData> m_outData =
			new Dictionary<string, AICS.SimulationView.AgentData>();

	public Dictionary<string, AICS.SimulationView.AgentData> StartActinSimulation(
		AICS.AgentSim.ModelDef modelDef
	)
	{
		this.m_agentMapping[0] = "Core";
		this.m_agentMapping[1] = "End";
		this.m_agentMapping[2] = "Monomer";

		this.StartCoroutine("Restart");

		for(int i = 0; i < 1000; ++i)
		{
			var ad = new AICS.SimulationView.AgentData();
			ad.agentName = "Actin";
			this.m_outData[i.ToString()] = ad;
		}

		return this.m_outData;
	}

	public Dictionary<string, AICS.SimulationView.AgentData> UpdateSimulation()
	{
		int i = 0;
		for(; i < this.AGENT_LIST.Count; ++i)
		{
			NetAgentData nad = this.AGENT_LIST[i];
			AICS.SimulationView.AgentData ad = new AICS.SimulationView.AgentData();

			ad.agentName = this.m_agentMapping[(int)nad.type];
			ad.position = new Vector3(nad.x, nad.y, nad.z);
			ad.rotation = new Vector3(nad.xrot, nad.yrot, nad.zrot);
			this.m_outData[i.ToString()] = ad;
		}

		for(; i < 1000; ++i)
		{
			this.m_outData.Remove(i.ToString());
		}

		return this.m_outData;
	}

	public IEnumerator Restart()
	{
		this.SetState(visClientState.NotStreaming);
		yield return new WaitForSeconds(0.3f);
		this.SetState(visClientState.Streaming);
	}

	public void UpdateTimeStep(float newTimeStep)
	{
		this.StepSize = newTimeStep;

		this.m_sent_data.Reset();
		this.m_sent_data.Write((byte)messageId.ID_UPDATE_TIME_STEP);
		this.m_sent_data.Write(newTimeStep);
		this.m_client.Send(
			this.m_sent_data, PacketPriority.HIGH_PRIORITY,
			PacketReliability.RELIABLE_ORDERED,
			(char)0, new AddressOrGUID(serverAddr), false);
	}

	public void UpdateRateParam(string paramName, float newRate)
	{
		RakNet.RakString rs = new RakNet.RakString(paramName);

		this.m_sent_data.Reset();
		this.m_sent_data.Write((byte)messageId.ID_UPDATE_RATE_PARAM);
		this.m_sent_data.Write(rs);
		this.m_sent_data.Write(newRate);
		this.m_client.Send(
			this.m_sent_data, PacketPriority.HIGH_PRIORITY,
			PacketReliability.RELIABLE_ORDERED,
			(char)0, new AddressOrGUID(serverAddr), false);
	}


	/**
	*	The below functions need to correspond with the C++ server functions
	* that serialize or deserialize the corresponding data
	*
	*	Word of warning: int data types have different sizes in C# & C++
	*	C++ sizeof(unsigned char) == sizeof(byte) C#
	* C++ sizeof(float) == sizeof(float) C#
	*
	*	I recommend using RakStrings to serialize string values
	*/
	void SerializeVisDataRequest(ref RakNet.BitStream bs, SimRequestData simReq)
	{
		bs.Reset();
		bs.Write((byte)messageId.ID_VIS_DATA_REQUEST);
		bs.Write((byte)simReq.Simulator);

		bs.Write(simReq.nTimeSteps);
		bs.Write(simReq.timeStepSize);
	}

	void DeserializeSimData(ref RakNet.BitStream bs, ref List<NetAgentData> outData)
	{
		byte msgHeader;
		bs.Read(out msgHeader);

		float naf = 0;
		bs.Read(out naf);

		int numAgents = (int)naf;
		outData.Clear();

		for(int i = 0; i < numAgents; ++i)
		{
			NetAgentData ad;
			bs.Read(out ad.type);
			bs.Read(out ad.x);
			bs.Read(out ad.y);
			bs.Read(out ad.z);
			bs.Read(out ad.xrot);
			bs.Read(out ad.yrot);
			bs.Read(out ad.zrot);
			outData.Add(ad);
		}
	}

}
