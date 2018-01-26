using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class PhysicalDirectSimulator : DirectSimulator 
	{
		protected Collider theCollider;
		protected Rigidbody body;

		float forceMagnitude
		{
			get 
			{
				float meanForce = 5E6f * displacement;
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
			}
		}

		float torqueMagnitude
		{
			get 
			{
				float meanForce = 5E6f * displacement;
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
			}
		}

		void Start ()
		{
			AddRigidbodyCollider();
		}

		void AddRigidbodyCollider ()
		{
			theCollider = gameObject.AddComponent<SphereCollider>();
			body = gameObject.AddComponent<Rigidbody>();
			body.drag = 10f;
			body.useGravity = false;
		}

		protected override void DoRandomStep ()
		{
			body.velocity = body.angularVelocity = Vector3.zero;
			body.AddForce( forceMagnitude * Random.onUnitSphere );
			body.AddTorque( torqueMagnitude * Random.onUnitSphere );
		}
	}
}