using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.AgentSim
{
    public class TimeParameter : Parameter
    {
        Observer _observer;
        Observer observer
        {
            get
            {
                if (_observer == null)
                {
                    _observer = GameObject.FindObjectOfType<Observer>();
                }
                return _observer;
            }
        }

        public void Init ()
        {
            value.text = observer.currentScale.ToString() + " s";
            slider.minValue = 1e-12f;
            slider.maxValue = 1e-6f;
            slider.value = 1e-9f;// observer.currentScale;
        }

        public override void ValueChanged (float newValue)
        {
            observer.currentScale = newValue;
            value.text = newValue.ToString() + " s";
        }
    }

    public abstract class Parameter : MonoBehaviour
    {
        public Text label;
        public Text value;
        public Slider slider;

        public abstract void ValueChanged (float newValue);
    }
}
