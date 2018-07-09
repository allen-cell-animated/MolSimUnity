using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.AgentSim
{
    public class RateParameter: MonoBehaviour
    {
        public Text label;
        public Text value;
        public Slider slider;

        Reaction reaction;

        public void Init (ReactionRateParameter _parameter, Reaction _reaction)
        {
            reaction = _reaction;
            label.text = _parameter.label;
            value.text = reaction.theoreticalRate.ToString() + " s⁻¹";
            slider.minValue = _parameter.range.x;
            slider.maxValue = _parameter.range.y;
            slider.value = reaction.theoreticalRate;
        }

        public void ValueChanged (float newValue)
        {
            reaction.theoreticalRate = newValue;
            value.text = newValue.ToString() + " s⁻¹";
        }
    }
}
