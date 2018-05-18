using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.UI
{
	public class RangeSlider : MonoBehaviour 
	{
		public SubRangeSlider minSlider;
		public SubRangeSlider maxSlider;
		public RectTransform fillRect;
		public float minValue = 0;
		public float maxValue = 1f;

		RectTransform _rectTransform;
		public RectTransform rectTransform
		{
			get {
				if (_rectTransform == null)
				{
					_rectTransform = GetComponent<RectTransform>();
				}
				return _rectTransform;
			}
		}

		void Awake ()
		{
			minSlider.type = SubSliderType.Min;
			minSlider.minValue = minValue;
			minSlider.maxValue = maxValue;
			maxSlider.type = SubSliderType.Max;
			maxSlider.minValue = minValue;
			maxSlider.maxValue = maxValue;
			SetFillRect();
		}

		public void SetFillRect ()
		{
			float sliderWidth = rectTransform.rect.width;
			float minHandlePosition = minSlider.value / maxValue * sliderWidth;
			float maxHandlePosition = maxSlider.value / maxValue * sliderWidth;

			float width = maxHandlePosition - minHandlePosition;
			float left = minHandlePosition + width / 2f;

			fillRect.anchoredPosition = new Vector2( left, fillRect.anchoredPosition.y );
			fillRect.sizeDelta = new Vector2( width, fillRect.sizeDelta.y );
		}
	}
}