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
			gobj.transform.SetParent(this.gameObject.transform);
			gobjs.Add(gobj);
		}
	}

	// Update is called once per frame
	void Update () {
		if(rknc == null || !rknc.IsStreamingSimulation())
		{
			return;
		}

		int i = 0;
		for(; i < rknc.AGENT_LIST.Count; ++i)
		{
			RakNetClient.NetAgentData ad = rknc.AGENT_LIST[i];
			gobjs[i].transform.position = new Vector3(ad.x, ad.y, ad.z);

			gobjs[i].SetActive(true);

			Renderer rend = gobjs[i].GetComponent<Renderer>();

			switch((int)ad.type)
			{
				case 0:
				{
				 rend.material.SetColor("_Color", Color.green);
				} break;
				case 1:
				{
				 rend.material.SetColor("_Color", Color.red);
				} break;
				case 2:
				{
				 rend.material.SetColor("_Color", Color.blue);
				} break;
			}
		}

		for(;i < testMax; ++i)
		{
			gobjs[i].SetActive(false);
		}
	}
}
