using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AICS.AgentSim
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] Selecter lastSelectedObject;

        static InputManager _Instance;
        public static InputManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = GameObject.FindObjectOfType<InputManager>();
                }
                return _Instance;
            }
        }

        public void SelectObject (Selecter selectedObj)
        {
            lastSelectedObject = selectedObj;
            EventSystem.current.SetSelectedGameObject( lastSelectedObject.gameObject );
            World.Instance.observer.FocusOn( lastSelectedObject.transform );
        }

        void Update ()
        {
            if (Input.GetMouseButton( 0 ))
            {
                KeepObjectSelected();
            }
        }

        void KeepObjectSelected ()
        {
            if (lastSelectedObject != null)
            {
                lastSelectedObject.SetSelected( true );
            }
        }
    }
}