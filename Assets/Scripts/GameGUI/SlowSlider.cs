using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
	[ExecuteInEditMode]
	public sealed class SlowSlider : MonoBehaviour
	{
		public Image sliderSlowImage;
		public Image sliderInstantImage;
		
		public float sliderSpeed = 0.2f;

		[SerializeField, Range(0,1)]
		private float _valuePercentage;

		public float ValuePercentage
		{
			get { return _valuePercentage; }
			set
			{
				_valuePercentage = value;
				UpdateImages();
			}
		}

		private void UpdateImages()
		{
			if (!sliderInstantImage) return;

			sliderInstantImage.fillAmount = ValuePercentage;

			if (!sliderSlowImage) return;

			if (ValuePercentage > sliderSlowImage.fillAmount)
				sliderSlowImage.fillAmount = ValuePercentage;
			if (sliderSpeed <= 0)
				sliderSlowImage.fillAmount = ValuePercentage;
		}
		
		private void LateUpdate()
		{
			if (!sliderSlowImage) return;

			if (sliderSpeed <= 0)
				sliderSlowImage.fillAmount = ValuePercentage;
			else
				sliderSlowImage.fillAmount = Mathf.MoveTowards(sliderSlowImage.fillAmount, ValuePercentage,
					sliderSpeed * Time.deltaTime);
		}

		private void OnValidate()
		{
			UpdateImages();
		}

	}

}