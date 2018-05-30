using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.UI
{
	public enum SubSliderType
	{
		Min,
		Max
	}

	public class SubRangeSlider : Slider 
	{
		public SubSliderType type;

		RangeSlider _rangeSlider;
		public RangeSlider rangeSlider
		{
			get {
				if (_rangeSlider == null)
				{
					_rangeSlider = GetComponentInParent<RangeSlider>();
				}
				return _rangeSlider;
			}
		}

		protected override void Set (float input, bool sendCallback)
		{
			switch (type)
			{
			case SubSliderType.Min:
				if (input <= rangeSlider.maxSlider.value)
				{
					base.Set(input, sendCallback);
					rangeSlider.SetFillRect();
				}
				return;

			case SubSliderType.Max:
				if (input >= rangeSlider.minSlider.value)
				{
					base.Set(input, sendCallback);
					rangeSlider.SetFillRect();
				}
				return;

			default:
				return;
			}
		}
	}
}