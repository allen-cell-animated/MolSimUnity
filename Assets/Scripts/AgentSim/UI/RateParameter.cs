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
            mapping = _parameter.mapping;
            Setup( _parameter.label, reactionDef.rate, _parameter.range.x, _parameter.range.y, false, " s⁻¹" );
        }

        public void ValueChanged (float _newValue)
        {
            SetValue( _newValue );
            SimulationManager.Instance.SetRateParameter( reactionDef, value );
            SimulationManager.Instance.SetRateParameter( this.label.text, value);
        }
    }
}
