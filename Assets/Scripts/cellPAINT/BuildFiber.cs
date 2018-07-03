using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildFiber : MonoBehaviour 
{
    public bool canCreate = false;
    public LayerMask planeLayer;
    public GameObject currentMonomerPrefab;
    public float currentMonomerLength;

    Vector3 lastCreatePoint;
    Transform currentFiber;

    FollowLookZoomCamera _camera;
    FollowLookZoomCamera cam
    {
        get
        {
            if (_camera == null)
            {
                _camera = GameObject.FindObjectOfType<FollowLookZoomCamera>();
            }
            return _camera;
        }
    }

    int _numFibers = 0;
    int numFibers
    {
        get
        {
            _numFibers++;
            return _numFibers;
        }
    }

    bool startedClick
    {
        get
        {
            return Input.GetButtonDown( "Fire1" ) && !EventSystem.current.IsPointerOverGameObject();
        }
    }

    bool dragging
    {
        get
        {
            return Input.GetButton( "Fire1" ) && !EventSystem.current.IsPointerOverGameObject();
        }
    }

    Vector3 cursorPosition3D
    {
        get
        {
            RaycastHit hit;
            if (Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, 1.5f * Camera.main.transform.GetChild( 0 ).localPosition.magnitude, planeLayer ))
            {
                return hit.point;
            }
            return Vector3.positiveInfinity;
        }
    }

    public void ToggleCreating ()
    {
        cam.canLook = canCreate;
        canCreate = !canCreate;
    }

	void Update () 
    {
        if (canCreate && (startedClick || dragging))
        {
            Vector3 _cursorPosition3D = cursorPosition3D;
            if (_cursorPosition3D.magnitude < 1e6f)
            {
                if (startedClick)
                {
                    if (Vector3.Distance( lastCreatePoint, _cursorPosition3D ) >= currentMonomerLength)
                    {
                        currentFiber = CreateFiber();
                    }
                    GameObject _newMonomer = CreateMonomer( _cursorPosition3D, Random.rotation );
                    if (_newMonomer != null)
                    {
                        lastCreatePoint = _cursorPosition3D;
                    }
                }
                else
                {
                    if (Vector3.Distance( lastCreatePoint, _cursorPosition3D ) >= currentMonomerLength)
                    {
                        GameObject _newMonomer = CreateMonomer( lastCreatePoint + currentMonomerLength * (_cursorPosition3D - lastCreatePoint).normalized, 
                                                                Quaternion.LookRotation( lastCreatePoint - _cursorPosition3D ) );
                        if (_newMonomer != null)
                        {
                            lastCreatePoint = _cursorPosition3D;
                        }
                    }
                }
            }
        }
	}

    Transform CreateFiber ()
    {
        GameObject newFiber = new GameObject( currentMonomerPrefab.name + "_Fiber_" + numFibers );
        newFiber.transform.SetParent( transform );
        newFiber.transform.localPosition = Vector3.zero;
        newFiber.transform.localRotation = Quaternion.identity;

        return newFiber.transform;
    }

    GameObject CreateMonomer (Vector3 _position, Quaternion _rotation)
    {
        if (currentMonomerPrefab == null)
        {
            return null;
        }

        GameObject newMonomer = Instantiate( currentMonomerPrefab ) as GameObject;
        newMonomer.transform.SetParent( currentFiber );
        newMonomer.transform.position = _position;
        newMonomer.transform.rotation = _rotation;

        ConnectMonomer( newMonomer );

        return newMonomer;
    }

    void ConnectMonomer (GameObject _lastMonomer)
    {
        GameObject _monomer;
        Rigidbody _lastMonomerBody = _lastMonomer.GetComponent<Rigidbody>();
        for (int i = currentFiber.childCount - 2; i >= Mathf.Max( 0, currentFiber.childCount - 4 ); i--)
        {
            _monomer = currentFiber.GetChild( i ).gameObject;

            if (i > currentFiber.childCount - 3)
            {
                CreateHinge( _monomer, _lastMonomerBody );
            }
            CreateSpring( _monomer, _lastMonomerBody, currentFiber.childCount - i - 1 );
        }
    }

    void CreateHinge (GameObject _object, Rigidbody _connectedBody)
    {
        HingeJoint _hinge = _object.AddComponent<HingeJoint>();
        _hinge.connectedBody = _connectedBody;
        _hinge.anchor = -currentMonomerLength / 2f * Vector3.forward;
        _hinge.axis = Vector3.back;
        _hinge.autoConfigureConnectedAnchor = false;
        _hinge.connectedAnchor = currentMonomerLength / 2f * Vector3.forward;
        _hinge.useLimits = true;
        JointLimits _limits = _hinge.limits;
        _limits.min = -2f;
        _limits.max = 2f;
        _hinge.limits = _limits;
        _hinge.enablePreprocessing = false;
    }

    void CreateSpring (GameObject _object, Rigidbody _connectedBody, int _monomersAway)
    {
        SpringJoint _spring = _object.AddComponent<SpringJoint>();
        _spring.connectedBody = _connectedBody;
        _spring.anchor = Vector3.zero;
        _spring.connectedAnchor = Vector3.zero;
        _spring.damper = 0.5f;
        _spring.minDistance = _spring.maxDistance = currentMonomerLength * _monomersAway;
    }
}
