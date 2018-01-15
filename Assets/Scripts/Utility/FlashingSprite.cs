using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashingSprite : MonoBehaviour
{
	private SpriteRenderer ren;

	public float blinkMinDuration = 0.05f;
	public float blinkMaxDuration = 0.15f;

	[Range(0,1)]
	public float blinkFrequency = 0.5f;

	public float minWait = 2;
	public float maxWait = 8;
	
	public Sprite[] sprites;

	private float timeLeft;

#if UNITY_EDITOR
	private void OnValidate()
	{
		blinkMinDuration = Mathf.Max(0, blinkMinDuration);
		blinkMaxDuration = Mathf.Max(0, blinkMaxDuration);
		minWait = Mathf.Max(0, minWait);
		maxWait = Mathf.Max(0, maxWait);
	}
#endif

	private void OnEnable()
	{
		ren = GetComponent<SpriteRenderer>();
	}

	private void Update ()
	{
		timeLeft -= Time.deltaTime;

		if (timeLeft <= 0)
		{
			if (Random.value <= blinkFrequency)
			{
				// Blink
				timeLeft += Random.Range(blinkMinDuration, blinkMaxDuration);
			}
			else
			{
				timeLeft += Random.Range(minWait, maxWait);
			}

			ren.sprite = RandomUnusedSprite();
		}
	}

	private Sprite RandomUnusedSprite()
	{
		if (sprites == null || sprites.Length == 0)
			return null;

		Sprite randomUnusedSprite = sprites[Random.Range(0, sprites.Length)];

		if (randomUnusedSprite == ren.sprite)
			return sprites.FirstOrDefault(s => s != ren.sprite) ?? randomUnusedSprite;

		return randomUnusedSprite;
	}
}
