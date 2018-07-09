using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AICS.AgentSim;

namespace AICS.SimulationView
{
    public class RateParameter : Parameter
    {
        ReactionDef reactionDef;

        public void Init (ReactionRateParameter _parameter)
        {
            reactionDef = _parameter.reactionDef;
            label.text = _parameter.label;
            value.text = reactionDef.rate.ToString() + " s⁻¹";
            slider.minValue = _parameter.range.x;
            slider.maxValue = _parameter.range.y;
            slider.value = reactionDef.rate;
        }

        public override void ValueChanged (float newValue)
        {
            value.text = newValue.ToString() + " s⁻¹";
            SimulationManager.Instance.SetRateParameter( reactionDef, newValue );
        }
    }
}
