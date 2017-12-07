using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
	public class DamagePopup : MonoBehaviour
	{
		public Text text;
		public float lifeLength = 1;

		public AnimationCurve curveYPosition = AnimationCurve.EaseInOut(0, 0, 1, 50);
		public AnimationCurve curveAlpha = AnimationCurve.Linear(0, 1, 1, 0);

		[HideInInspector] public int damage;
		[SerializeField, HideInInspector] private float timeStart;

		private void Awake()
		{
			timeStart = Time.time;
		}

		private void Start()
		{
			text.text = "-" + Mathf.Abs(damage);
		}

		private void Update()
		{
			float lapsed = Time.time - timeStart;
			if (lapsed >= lifeLength)
			{
				Destroy(gameObject);
				return;
			}

			float lapsed01 = lapsed / lifeLength;

			Color color = text.color;
			color.a = curveAlpha.Evaluate(lapsed01);
			text.color = color;

			Vector2 pos = text.rectTransform.anchoredPosition;
			pos.y = curveYPosition.Evaluate(lapsed01);
			text.rectTransform.anchoredPosition = pos;
		}

	}

}