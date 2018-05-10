using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using cakeslice;

namespace AICS.AgentSim
{
    [RequireComponent( typeof(Collider), typeof(Outline) )]
    public class Selecter : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        Outline _outline;
        Outline outline
        {
            get
            {
                if (_outline == null)
                {
                    _outline = GetComponentInChildren<Outline>( true );
                }
                return _outline;
            }
        }

        void OnMouseEnter ()
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject)
            {
                outline.enabled = true;
            }
        }

        void OnMouseExit ()
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject)
            {
                outline.enabled = false;
            }
        }

		void OnMouseUp()
		{
            InputManager.Instance.SelectObject( gameObject );
		}

        public void OnSelect (BaseEventData eventData)
        {
            outline.enabled = true;
            outline.color = 1;
        }

        public void OnDeselect (BaseEventData eventData)
        {
            outline.enabled = false;
            outline.color = 0;
        }
	}
}