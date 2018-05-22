using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.AgentSim
{
    public class InputManager : MonoBehaviour
    {
        public Reactor reactor;

        public GameObject pauseButton;
        public GameObject playButton;
        public Text totalTime;

        public Selecter lastSelectedObject;
        bool justSelected;

        Vector2 mouseDelta = Vector2.zero;
        float mouseMoveThreshold = 0.1f;
        bool mouseMoved 
        {
            get
            {
                return mouseDelta.x > mouseMoveThreshold || mouseDelta.y > mouseMoveThreshold;
            }
        }

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
            SetLastSelectedObjectSelected( false );
            lastSelectedObject = selectedObj;
            SetLastSelectedObjectSelected( true );
            World.Instance.observer.FocusOn( lastSelectedObject.transform );
            justSelected = true;
        }

        void Update ()
        {
            if (Input.GetMouseButtonDown( 0 ))
            {
                mouseDelta = Vector2.zero;
            }
            else if (Input.GetMouseButton( 0 ))
            {
                mouseDelta += new Vector2( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ) );
                if (mouseMoved)
                {
                    OnDrag();
                }
            }
            else if (Input.GetMouseButtonUp( 0 ))
            {
                mouseDelta += new Vector2( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ) );
                if (!mouseMoved)
                {
                    OnClick();
                }
            }
            justSelected = false;

            if (!World.Instance.paused)
            {
                UpdateTime();
            }
        }

        void OnDrag ()
        {
            //keep selection while dragging to look
            SetLastSelectedObjectSelected( true );
        }

        void OnClick ()
        {
            //select nothing
            if (!justSelected)
            {
                ClearSelection();
            }
        }

        void SetLastSelectedObjectSelected (bool selected)
        {
            if (lastSelectedObject != null)
            {
                lastSelectedObject.SetSelected( selected );
            }
        }

        void ClearSelection ()
        {
            SetLastSelectedObjectSelected( false );
            lastSelectedObject = null;
            World.Instance.observer.FocusOn( World.Instance.transform );
        }

        public void TogglePaused (bool paused)
        {
            World.Instance.paused = paused;

            pauseButton.SetActive( !paused );
            playButton.SetActive( paused );
        }

        void UpdateTime ()
        {
            totalTime.text = Helpers.FormatTime( World.Instance.time, 0 );
        }

        public void Restart ()
        {
            reactor.Restart();
        }
    }
}