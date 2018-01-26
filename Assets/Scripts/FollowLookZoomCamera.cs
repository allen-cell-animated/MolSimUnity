using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowLookZoomCamera : MonoBehaviour
{
	public Transform target;
	public float followSpeed = 1f;
	public float lookSpeed = 1.5f;
	public float timeToLookAtTarget = 1f;
	public Vector2 tiltLimits = new Vector2( -45f, 75f ); // limits of the x axis rotation of the pivot
	public float zoomSpeedScroll = 1f;
	public float zoomSpeedArrows = 0.1f;
	public Vector2 zoomLimits = new Vector2( -5f, -50f );

	float lookAngle = 0; // rig's y axis rotation.
	float tiltAngle = 0; // pivot's x axis rotation.
	Vector3 pivotEulers;
	bool animatingLook;
	float animationStartTime;
	bool animateTilt = false;

	static FollowLookZoomCamera _Instance;
	public static FollowLookZoomCamera Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = GameObject.FindObjectOfType<FollowLookZoomCamera>();
			}
			return _Instance;
		}
	}

	Transform _cam;
	Transform cam
	{
		get
		{
			if (_cam == null)
			{
				_cam = GetComponentInChildren<Camera>().transform;
			}
			return _cam;
		}
	}

	Transform _pivot;
	Transform pivot
	{
		get
		{
			if (_pivot == null)
			{
				_pivot = cam.parent;
			}
			return _pivot;
		}
	}

	Rigidbody _targetRigidbody;
	Rigidbody targetRigidbody
	{
		get
		{
			if (_targetRigidbody == null && target != null)
			{
				_targetRigidbody = target.GetComponent<Rigidbody>();
			}
			return _targetRigidbody;
		}
	}

	bool dragging
	{
		get
		{
			return Input.GetButton( "Fire1" ) && !EventSystem.current.IsPointerOverGameObject();
		}
	}

	bool targetIsPhysicsObject
	{
		get
		{
			return targetRigidbody != null && !targetRigidbody.isKinematic;
		}
	}

	void Awake()
	{
		pivotEulers = pivot.rotation.eulerAngles;
	}

	void Update () 
	{
		DragToLook();
		AnimateRotation();
		ScrollToZoom();
	}

	// ----------------------------------------------------------------------------------- Follow

	void FixedUpdate ()
	{
		if (targetIsPhysicsObject)
		{
			FollowTarget();
		}
	}

	void LateUpdate ()
	{
		if (!targetIsPhysicsObject)
		{
			FollowTarget();
		}
	}

	void FollowTarget ()
	{
		if (target != null) 
		{
			transform.position = Vector3.Lerp( transform.position, target.position, Time.deltaTime * followSpeed );
		}
	}

	// ----------------------------------------------------------------------------------- Look

	void DragToLook ()
	{
		if (dragging)
		{
			HandleRotationInput();
		}
	}

	void HandleRotationInput ()
	{
		var x = Input.GetAxis("Mouse X");
		var y = Input.GetAxis("Mouse Y");

		lookAngle += x * lookSpeed;
		tiltAngle -= y * lookSpeed;
		tiltAngle = Mathf.Clamp( tiltAngle, tiltLimits.x, tiltLimits.y );

		SetRotation();
	}

	void SetRotation ()
	{
		pivot.localRotation = goalTiltRotation;
		transform.localRotation = goalLookRotation;
	}

	public void LookAtTarget ()
	{
		Vector3 localTarget = cam.InverseTransformPoint( target.position );
		lookAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

		if (animateTilt)
		{
			tiltAngle = Mathf.Atan2( localTarget.y, localTarget.z ) * Mathf.Rad2Deg;
			tiltAngle = Mathf.Clamp( tiltAngle, tiltLimits.x, tiltLimits.y );
		}

		animatingLook = true;
		animationStartTime = Time.time;
	}

	Quaternion goalLookRotation
	{
		get
		{
			return Quaternion.Euler( 0f, lookAngle, 0f );
		}
	}

	Quaternion goalTiltRotation
	{
		get
		{
			return Quaternion.Euler( tiltAngle, pivotEulers.y , pivotEulers.z );
		}
	}

	void AnimateRotation ()
	{
		if (animatingLook && !dragging)
		{
			float t = (Time.time - animationStartTime) / timeToLookAtTarget;
			if (t <= 1f)
			{
				if (animateTilt)
				{
					pivot.localRotation = Quaternion.Slerp( pivot.localRotation, goalTiltRotation, t );
				}
				transform.localRotation = Quaternion.Slerp( transform.localRotation, goalLookRotation, t );
			}
			else
			{
				animatingLook = false;
			}
		}
	}

	// ----------------------------------------------------------------------------------- Zoom

	void ScrollToZoom ()
	{
		float scroll = Input.GetAxis( "Mouse ScrollWheel" );
		float arrow = Input.GetAxis( "Vertical" );

		float speed = zoomSpeedScroll;
		if (arrow != 0)
		{
			speed = zoomSpeedArrows;
		}

		if (scroll > 0 || arrow > 0)
		{
			if (cam.localPosition.z < zoomLimits.x)
			{
				cam.localPosition += speed * Vector3.forward;
			}
		}
		else if (scroll < 0 || arrow < 0)
		{
			if (cam.localPosition.z > zoomLimits.y)
			{
				cam.localPosition -= speed * Vector3.forward;
			}
		}
	}
}
