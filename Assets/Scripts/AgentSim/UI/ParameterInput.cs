using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ParameterInput<T> : MonoBehaviour where T : Component
	{
		public Parameter dTime; // = 100, 100 ps --> 1 μs

		static T _Instance;
		public static T Instance
		{
			get {
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<T>();
				}
				return _Instance;
			}
		}

		void Start ()
		{
			Init();
		}

		public void Init ()
		{
			dTime.InitSlider();
			InitSliders();
		}

		public virtual void InitSliders () { }

		public void SetDTime (float _sliderValue)
		{
			dTime.Set( _sliderValue );
		}
	}
}