using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{

	public class Healthbar : MonoBehaviour
	{
		private RectTransform rectTransform;

		public RingWalker walker;
		public Image sliderInstant;
		public Image sliderSlow;

		[Header("Settings")]
		[Range(0,1)]
		public float speed = 0.2f;
		public float destroyAtDeathDelay = 5;

		public bool followX = true;
		public bool followY = true;
		
		private float _healthPercentage;

		public float HealthPercentage
		{
			get { return _healthPercentage; }
			set
			{
				_healthPercentage = value;
				SetSliders();
			}
		}

		private bool dying = false;

		private void Awake()
		{
			rectTransform = transform as RectTransform;
		}

		private void Start()
		{
			UpdatePercentageFromWalker();
		}

		private void SetSliders()
		{
			if (sliderInstant)
			{
				sliderInstant.fillAmount = HealthPercentage;
				if (sliderSlow)
				{
					if (HealthPercentage > sliderSlow.fillAmount)
						sliderSlow.fillAmount = HealthPercentage;
					if (speed <= 0)
						sliderSlow.fillAmount = HealthPercentage;
				}
			}
		}

		public void UpdatePercentageFromWalker()
		{
			HealthPercentage = walker.maxHealth == 0 ? 0 : walker.health / (float) walker.maxHealth;
		}

		private void LateUpdate()
		{
			if (walker)
			{
				Vector2 pos = Camera.main.WorldToViewportPoint(walker.transform.position);

				// Beautiful code :') *cough cough*
				Vector2 newAnchorMin = rectTransform.anchorMin;
				if (followX) newAnchorMin.x = pos.x;
				if (followY) newAnchorMin.y = pos.y;
				rectTransform.anchorMin = newAnchorMin;
				Vector2 newAnchorMax = rectTransform.anchorMax;
				if (followX) newAnchorMax.x = pos.x;
				if (followY) newAnchorMax.y = pos.y;
				rectTransform.anchorMax = newAnchorMax;

				UpdatePercentageFromWalker();
				
				if (speed > 0)
				{
					sliderInstant.fillAmount = HealthPercentage;

					sliderSlow.fillAmount = Mathf.MoveTowards(sliderSlow.fillAmount, HealthPercentage,
						speed * Time.deltaTime);
				}

				if (walker.Dead && !dying)
				{
					Destroy(gameObject, destroyAtDeathDelay);
					dying = true;
				}
			}
		}

		private void OnValidate()
		{
			destroyAtDeathDelay = Mathf.Max(0, destroyAtDeathDelay);
		}

	}

}