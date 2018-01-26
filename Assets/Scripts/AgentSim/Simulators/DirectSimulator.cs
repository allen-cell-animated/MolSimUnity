using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class DirectSimulator : Simulator 
	{
		[Tooltip( "([Agent's scale] meters)^2 / s" )]
		[SerializeField] protected float diffusionCoefficient;

		protected float displacement
		{
			get 
			{
				return Time.deltaTime * Mathf.Sqrt( diffusionCoefficient * agent.scale );
			}
		}

		public override void SimulateTo (float time)
		{
			DoRandomStep();
		}

		protected virtual void DoRandomStep ()
		{
			transform.position += 2E3f * displacement * Random.onUnitSphere;
		}
	}
}