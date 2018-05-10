using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AICS.AgentSim
{
    public class InputManager : MonoBehaviour 
    {
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

        public void SelectObject (GameObject obj)
        {
            EventSystem.current.SetSelectedGameObject( obj );
            World.Instance.observer.FocusOn( obj.transform );
        }
    }
}