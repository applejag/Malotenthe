using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
	[RequireComponent(typeof(Text))]
	public class SlowIntegerCounter : MonoBehaviour
	{
		private Text m_text;
		private Text Text { get { return m_text ?? (m_text = GetComponent<Text>()); } }
		[SerializeField, HideInInspector] private float timeStart = 0;
		[SerializeField, HideInInspector] private float timeEnd = 0;

		public AnimationCurve interpolation = AnimationCurve.EaseInOut(0, 0, 1, 1);
		public int minWidth = 10;

		[HideInInspector] public int currentValue = 0;
		[HideInInspector] public int startValue = 0;
		public int targetValue = 0;

		public Color colorLeading = Color.gray;
		public Color colorNumber = Color.white;

		public void SetTargetValue(int target, float time)
		{
			timeStart = Time.time;
			timeEnd = timeStart + time;

			startValue = currentValue;
			targetValue = target;

			enabled = true;
		}

		private void OnValidate()
		{
			minWidth = Mathf.Max(minWidth, 0);
			targetValue = Mathf.Max(targetValue, 0);
			currentValue = Mathf.Max(currentValue, 0);

			if (Text)
				SetTextWithFormat(targetValue);
		}

		private void Awake()
		{
			m_text = GetComponent<Text>();
		}

		private void Start()
		{
			currentValue = targetValue;
			SetTextWithFormat(currentValue);
		}

		private void Update()
		{
			float t = Mathf.InverseLerp(timeStart, timeEnd, Time.time);
			if (t >= 0.999f)
			{
				enabled = false;
				currentValue = targetValue;
			}
			else
			{
				currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, interpolation.Evaluate(t)));
			}

			SetTextWithFormat(currentValue);
		}

		private void SetTextWithFormat(int num)
		{
			num = Mathf.Max(num, 0);
			int leading = num == 0
				? minWidth
				: minWidth - Mathf.FloorToInt(Mathf.Log10(num)) - 1;

			Text.text = string.Format("<color=#{0}>{1}</color><color=#{2}>{3}</color>",
				ColorUtility.ToHtmlStringRGBA(colorLeading),
				leading > 0 ? new string('0', leading) : string.Empty,
				ColorUtility.ToHtmlStringRGBA(colorNumber),
				num > 0 ? num.ToString() : string.Empty);
		}
	}

}