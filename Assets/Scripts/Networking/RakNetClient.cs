using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RakNet;

public class RakNetClient : MonoBehaviour {

	public enum messageId {
		ID_VIS_DATA_ARRIVE = DefaultMessageIDTypes.ID_USER_PACKET_ENUM,
		ID_VIS_DATA_REQUEST
	};

	public enum simulator {
		DevTest = 1,
		Readdy
	};

	struct SimRequestData {
		public SimRequestData(simulator s, short tSteps, int stepSizeNs)
		{
			Simulator = s;
			nTimeSteps = tSteps;
			timeStepSize = stepSizeNs;
		}

		public simulator Simulator;
		public float nTimeSteps;
		public float timeStepSize;
	};

	public struct AgentData
	{
		public float type;
		public float x, y, z;
		public float xrot, yrot, zrot;
	};

	Packet m_packet;
	RakPeerInterface m_client;
	public bool isStreamingSimulation = false;
	public bool simRequestSent = false;

	private RakNet.BitStream m_recieved_data;
	private RakNet.BitStream m_sent_data;

	SystemAddress serverAddr;

	private int simStepCounter;
	private int requestedSteps;

	public List<AgentData> AGENT_LIST = new List<AgentData>();

	// Use this for initialization
	void Start () {
		serverAddr = new SystemAddress("127.0.0.1", 60000);

		this.m_recieved_data = new RakNet.BitStream();
		this.m_sent_data = new RakNet.BitStream();

		this.m_client = RakPeer.GetInstance();
		this.m_client.Startup(1, new SocketDescriptor(), 1);
		this.m_client.Connect("127.0.0.1", 60000, "", 0);
		Debug.Log("Starting Client");

		this.simStepCounter = 0;
		this.requestedSteps = 0;
	}

	// Update is called once per frame
	void Update () {
		if(isStreamingSimulation && !simRequestSent)
		{
			Debug.Log("Sending data request");
			SimRequestData sr = new SimRequestData(simulator.DevTest, 1000, 1);
			SerializeVisDataRequest(ref this.m_sent_data, sr);

			this.m_client.Send(
				this.m_sent_data, PacketPriority.HIGH_PRIORITY,
				PacketReliability.RELIABLE_ORDERED,
				(char)0, new AddressOrGUID(serverAddr), false);

			simRequestSent = true;
			Debug.Log(String.Format("Num Time Steps: {0}", sr.nTimeSteps));
			Debug.Log(String.Format("Time Step Size: {0}", sr.timeStepSize));
			this.simStepCounter = 0;
			this.requestedSteps = 1000;
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
				//Debug.Log("Simulation Results have arrived");
				simStepCounter++;

				//@NOTE This won't work when packets are dropped
				if(simStepCounter == requestedSteps)
				{
					Debug.Log("Request simulation data has finished");
				}

				this.m_recieved_data.Reset();
				this.m_recieved_data.Write(this.m_packet.data, this.m_packet.length);
				DeserializeSimData(ref this.m_recieved_data, ref this.AGENT_LIST);
			} break;
			default:
			{
					Debug.Log(String.Format("Message with identifier {0} has arrived", this.m_packet.data[0]));
			} break;
		}
	}

	void OnDestroy()
	{
		Debug.Log("Destroying Client");
		RakPeerInterface.DestroyInstance(this.m_client);
	}

	void SerializeVisDataRequest(ref RakNet.BitStream bs, SimRequestData simReq)
	{
		bs.Reset();
		bs.Write((byte)messageId.ID_VIS_DATA_REQUEST);
		bs.Write((byte)simReq.Simulator);

		bs.Write(simReq.nTimeSteps);
		bs.Write(simReq.timeStepSize);
	}

	void DeserializeSimData(ref RakNet.BitStream bs, ref List<AgentData> outData)
	{
		byte msgHeader;
		bs.Read(out msgHeader);

		float naf = 0;
		bs.Read(out naf);

		int numAgents = (int)naf;
		outData.Clear();
		Debug.Log(String.Format("{0} Agents arrived", numAgents));

		for(int i = 0; i < numAgents; ++i)
		{
			AgentData ad;
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
