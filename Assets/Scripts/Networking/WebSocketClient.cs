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
			this.m_wsw.SendMessage("Hello World!");
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
