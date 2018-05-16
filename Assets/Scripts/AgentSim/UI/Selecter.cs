using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using cakeslice;

namespace AICS.AgentSim
{
    [RequireComponent( typeof(Collider), typeof(Outline) )]
    public class Selecter : MonoBehaviour
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

        bool mouseIsOver;

        void OnMouseEnter ()
        {
            if (InputManager.Instance.lastSelectedObject != this)
            {
                outline.enabled = true;
                mouseIsOver = true;
            }
        }

        void OnMouseExit ()
        {
            if (InputManager.Instance.lastSelectedObject != this)
            {
                outline.enabled = false;
                mouseIsOver = false;
            }
        }

		void OnMouseUp()
		{
            if (mouseIsOver)
            {
                InputManager.Instance.SelectObject( this );
            }
		}

        public void SetSelected (bool selected)
        {
            outline.enabled = selected;
            outline.color = selected ? 1 : 0;
        }
	}
}