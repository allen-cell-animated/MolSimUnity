﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AICS.AgentSim;

namespace AICS.SimulationView
{
    public class InputManager : MonoBehaviour
    {
        public RectTransform parameterViewport;
        public GameObject pauseButton;
        public GameObject playButton;
        public Text totalTime;
        public Text fps;

        public Selecter lastSelectedObject;
        bool justSelected;
        float deltaTime;
        float parameterContentHeight;
        float parameterPrefabHeight = 110f;

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

        public void CreateCustomUI (ModelDef _modelDef)
        {
            CreateTimeParameter( _modelDef.scale );
            CreateRateParameters( _modelDef );
            parameterViewport.sizeDelta = new Vector2( parameterViewport.sizeDelta.x, parameterContentHeight );
        }

        void CreateTimeParameter (float _initialDT)
        {
            GameObject prefab = Resources.Load( "UI/TimeParameter" ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "TimeParameter prefab not found in Resources/UI" );
                return;
            }

            TimeParameter timeParameter = (Instantiate( prefab, parameterViewport ) as GameObject).GetComponent<TimeParameter>();
            timeParameter.GetComponent<RectTransform>().localPosition = new Vector3( 125f, -55f - parameterContentHeight, 0 );
            timeParameter.Init( _initialDT );
            parameterContentHeight += parameterPrefabHeight;
        }

        void CreateRateParameters (ModelDef _modelDef)
        {
            GameObject prefab = Resources.Load( "UI/RateParameter" ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "RateParameter prefab not found in Resources/UI" );
                return;
            }

            foreach (ReactionRateParameter parameter in _modelDef.adjustableParameters)
            {
                RateParameter rateParameter = (Instantiate( prefab, parameterViewport ) as GameObject).GetComponent<RateParameter>();
                rateParameter.GetComponent<RectTransform>().localPosition = new Vector3( 125f, -55f - parameterContentHeight, 0 );
                rateParameter.Init( parameter );
                parameterContentHeight += parameterPrefabHeight;
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
            UpdateFPS();
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
            totalTime.text = Helpers.FormatSIValue( World.Instance.time, 2, "s" );
        }

        void UpdateFPS ()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            fps.text = Mathf.Round( 1f / deltaTime ).ToString() + " fps";
        }

        public void Restart ()
        {
            SimulationManager.Instance.Restart();
        }
    }
}