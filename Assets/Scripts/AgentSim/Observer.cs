using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class Observer : MonoBehaviour 
	{
        [Tooltip( "meters" )]
		public float currentScale = 1e-9f;

        FollowLookZoomCamera _cameraController;
        FollowLookZoomCamera cameraController
        {
            get
            {
                if (_cameraController == null)
                {
                    _cameraController = GetComponentInParent<FollowLookZoomCamera>();
                }
                return _cameraController;
            }
        }

        public void FocusOn (Transform _target)
        {
            if (cameraController != null)
            {
                cameraController.target = _target;
            }
        }
	}
}