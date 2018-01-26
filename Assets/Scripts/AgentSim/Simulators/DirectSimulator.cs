using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class DirectSimulator : Simulator 
	{
		[Tooltip( "([Agent's scale] meters)^2 / s" )]
		[SerializeField] protected float diffusionCoefficient;

		protected Collider theCollider;
		protected Rigidbody body;

		float forceMagnitude
		{
			get {
				float meanForce = Time.deltaTime * 500000f * Mathf.Sqrt( diffusionCoefficient * agent.scale );
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
			}
		}

		float torqueMagnitude
		{
			get {
				float meanForce = Time.deltaTime * 500000f * Mathf.Sqrt( diffusionCoefficient * agent.scale );
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce);
			}
		}

		void Start ()
		{
			AddRigidbodyCollider();
		}

		protected virtual void AddRigidbodyCollider ()
		{
			theCollider = gameObject.AddComponent<SphereCollider>();
			body = gameObject.AddComponent<Rigidbody>();
			body.useGravity = false;
		}

		public override void SimulateTo (float time)
		{
			body.velocity = body.angularVelocity = Vector3.zero;
			body.AddForce( forceMagnitude * Random.onUnitSphere );
			body.AddTorque( torqueMagnitude * Random.onUnitSphere );
		}
	}
}