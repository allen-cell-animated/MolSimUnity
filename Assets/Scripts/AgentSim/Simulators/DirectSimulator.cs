using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class DirectSimulator : Simulator 
	{
		[Tooltip( "([Agent's scale] meters)^2 / s" )]
		public float diffusionCoefficient;

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
            Vector3 moveStep = 2E3f * displacement * Random.onUnitSphere;
            if (agent.container.PointIsInBounds( transform.position + moveStep ))
            {
                transform.position += moveStep;
            }
            else if (agent.container.PointIsInBounds( transform.position - moveStep ))
            {
                transform.position -= moveStep;
            }
		}
	}
}