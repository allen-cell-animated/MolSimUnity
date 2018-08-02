using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System;
using Coe.WebSocketWrapper;

public class WebSocketClient : MonoBehaviour {

	/**
	*	This needs to correspond to the values defined in the C++
	*	server program
	*/
	public enum message_id {
		id_vis_data_arrive = 0,
	  id_vis_data_request,
	  id_vis_data_finish,
	  id_vis_data_pause,
	  id_vis_data_resume,
	  id_vis_data_abort,
	  id_update_time_step,
	  id_update_rate_param
	};

	class net_message {
		public void Write(byte data)
		{
			_msg[w_end++] = (char)data;
		}

		public string Get() { return new string(_msg); }

		private char[] _msg = new char[256];
		private int w_end = 0;
	}

	private bool m_connected = false;

	private WebSocketWrapper m_wsw;

	void HandleMessage(string s, WebSocketWrapper ws)
	{
		Debug.Log(string.Format("Received Message: {0}", s));
	}

	void HandleConnect(WebSocketWrapper ws)
	{
		Debug.Log("WebSocketClient Connected");
	}

	void HandleDisconnect(WebSocketWrapper ws)
	{
		Debug.Log("WebSocketClient Disconnected");
	}

	void StartRemoteSimulation(
		WebSocketWrapper ws)
	{
		net_message nm = new net_message();
		nm.Write((byte)message_id.id_vis_data_request);
		ws.SendMessage(nm.Get());
	}

	void StopRemoteSimulation(
		WebSocketWrapper ws)
	{
		net_message nm = new net_message();
		nm.Write((byte)message_id.id_vis_data_abort);
		ws.SendMessage(nm.Get());
	}

	public void TryConnect() {
		if(!this.m_connected)
		{
			this.m_wsw = WebSocketWrapper.Create("ws://127.0.0.1:9002");
			this.m_wsw.OnMessage(HandleMessage);
			this.m_wsw.OnConnect(HandleConnect);
			this.m_wsw.OnDisconnect(HandleDisconnect);

			this.m_wsw.Connect();
			this.m_connected = true;

			Thread.Sleep(1000);
			this.StartRemoteSimulation(this.m_wsw);
			//this.StopRemoteSimulation(this.m_wsw);
		}
	}

	public void TryDisconnect() {
		this.m_wsw.Disconnect();
	}

	// Use this for initialization
	void Start () {
		TryConnect();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnDestroy () {
		TryDisconnect();
	}
}
