using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AICS.AgentSim;

namespace AICS.SimulationView
{
    public class TimeParameter : Parameter
    {
        public void Init (float _initialDT)
        {
            value.text = _initialDT.ToString() + " s";
            slider.minValue = 1e-12f;
            slider.maxValue = 1e-6f;
            slider.value = 1e-9f;// _initialDT;
        }

        public override void ValueChanged (float newValue)
        {
            value.text = newValue.ToString() + " s";
            SimulationManager.Instance.SetDT( newValue );
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
