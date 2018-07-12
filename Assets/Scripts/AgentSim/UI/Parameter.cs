using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AICS.AgentSim;

namespace AICS.SimulationView
{
    public enum SliderMapping
    {
        Linear,
        Logarithmic
    }

    public abstract class Parameter : MonoBehaviour
    {
        public Text label;
        public Text valueLabel;
        public Slider slider;

        protected SliderMapping mapping;
        protected float value;
        float max;
        float min;
        bool formatInSIUnits;
        string units;

        protected void Setup (string _label, float _initialValue, float _min, float _max, bool _formatInSIUnits, string _units)
        {
            label.text = _label;
            value = _initialValue;
            min = _min;
            max = _max;
            slider.minValue = 0;
            slider.maxValue = 1f;
            formatInSIUnits = _formatInSIUnits;
            units = _units;

            slider.value = UnmapValue( value );
            UpdateValueLabel();
        }

        protected void SetValue (float _newValue)
        {
            value = MapValue( _newValue );
            UpdateValueLabel();
        }

        protected void UpdateValueLabel ()
        {
            valueLabel.text = formatInSIUnits ? Helpers.FormatSIValue( value, 2, units ) : Helpers.FormatRoundedValue( value, 2 ) + " " + units;
        }

        protected float MapValue (float _sliderValue)
        {
            switch (mapping) 
            {
                case SliderMapping.Linear :
                    return MapValueLinear( _sliderValue );
                case SliderMapping.Logarithmic :
                    return MapValueLogarithmic( _sliderValue );
                default :
                    return 0;
            }
        }

        float MapValueLinear (float _sliderValue)
        {
            return min + _sliderValue * (max - min);
        }

        float MapValueLogarithmic (float _sliderValue)
        {
            return Mathf.Pow( 10f, Mathf.Log10( min ) + _sliderValue * (Mathf.Log10( max ) - Mathf.Log10( min )) );
        }

        protected float UnmapValue (float _value)
        {
            switch (mapping) 
            {
                case SliderMapping.Linear :
                    return UnmapValueLinear( _value );
                case SliderMapping.Logarithmic :
                    return UnmapValueLogarithmic( _value );
                default :
                    return 0;
            }
        }

        float UnmapValueLinear (float _value)
        {
            return (_value - min) / (max - min);
        }

        float UnmapValueLogarithmic (float _value)
        {
            return (Mathf.Log10( _value ) - Mathf.Log10( min )) / (Mathf.Log10( max ) - Mathf.Log10( min ));
        }
    }
}
