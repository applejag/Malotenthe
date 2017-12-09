using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
	[RequireComponent(typeof(SlowSlider))]
	public class Healthbar : MonoBehaviour
	{
		[SerializeField, HideInInspector] private SlowSlider slider;

		public RingWalker walker;

		[Header("Settings")]
		public float destroyAtDeathDelay = 5;

		[SerializeField, HideInInspector]
		private float dyingTime = 0;
		
		private void Awake()
		{
			slider = GetComponent<SlowSlider>();
		}

		private void Start()
		{
			UpdateSliderFromWalkerHealth();
		}

		public void UpdateSliderFromWalkerHealth()
		{	
			slider.ValuePercentage = walker.HealthPercentage;
		}

		private void LateUpdate()
		{
			if (!walker) return;

			UpdateSliderFromWalkerHealth();

			if (walker.IsDead && destroyAtDeathDelay >= 0)
			{
				dyingTime += Time.deltaTime;
				if (dyingTime >= destroyAtDeathDelay)
				{
					Destroy(gameObject);
				}
			}
		}

		private void OnValidate()
		{
			if (destroyAtDeathDelay < 0)
				destroyAtDeathDelay = -1;
		}

	}

}