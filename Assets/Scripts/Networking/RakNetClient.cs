using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RakNet;

public class RakNetClient : MonoBehaviour {
	Packet m_packet;
	RakPeerInterface m_client;

	// Use this for initialization
	void Start () {
		this.m_client = RakPeer.GetInstance();
		this.m_client.Startup(1, new SocketDescriptor(), 1);
		this.m_client.Connect("127.0.0.1", 60000, "", 0);
		Debug.Log("Starting Client");
	}

	// Update is called once per frame
	void Update () {
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
			default:
			{
					Debug.Log(String.Format("Message with identifier {0} has arrived", this.m_packet.data[0]));
			} break;
		}
	}

	void OnDestroy()
	{
		Debug.Log("Destroying Client");
		RakNet.RakPeerInterface.DestroyInstance(this.m_client);
	}
}
