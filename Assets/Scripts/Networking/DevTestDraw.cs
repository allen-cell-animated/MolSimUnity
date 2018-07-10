using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTestDraw : MonoBehaviour {

	public RakNetClient rknc;
	private List<GameObject> gobjs;
	private int testMax = 5000;

	// Use this for initialization
	void Start () {
		gobjs = new List<GameObject>();

		for(int i = 0; i < testMax; ++i)
		{
			GameObject gobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gobj.SetActive(false);
			gobjs.Add(gobj);
		}
	}

	// Update is called once per frame
	void Update () {
		if(rknc == null || !rknc.isStreamingSimulation)
		{
			return;
		}

		int i = 0;
		for(; i < rknc.AGENT_LIST.Count; ++i)
		{
			RakNetClient.AgentData ad = rknc.AGENT_LIST[i];
			gobjs[i].transform.position = new Vector3(ad.x, ad.y, ad.z);

			gobjs[i].SetActive(true);
		}

		for(;i < testMax; ++i)
		{
			gobjs[i].SetActive(false);
		}
	}
}
