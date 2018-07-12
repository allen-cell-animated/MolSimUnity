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
            mapping = SliderMapping.Logarithmic;
            Setup( "dTime", _initialDT, 1e-12f, 1e-6f, true, "s" );
        }

        public void ValueChanged (float _newValue)
        {
            SetValue( _newValue );
            SimulationManager.Instance.SetDT( value );
        }
    }
}
