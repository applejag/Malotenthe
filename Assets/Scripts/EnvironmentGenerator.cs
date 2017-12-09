using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class EnvironmentGenerator : MonoBehaviour
{
	[HideInInspector]
	public Layer[] layers;

	public float spawnInnerBoundary = 200;
	public float spawnOuterBoundary = 240;
	public float spawnRandomRange = 4;
	public GameObject spritePrefab;

	private void Reset()
	{
		spawnInnerBoundary = Mathf.Max(RingData.Radius - 20, 0);
		spawnOuterBoundary = Mathf.Max(RingData.Radius - 5, 0);
	}

	private void OnValidate()
	{
		spawnInnerBoundary = Mathf.Clamp(spawnInnerBoundary, 0, spawnOuterBoundary);
		spawnOuterBoundary = Mathf.Max(spawnOuterBoundary, spawnInnerBoundary);
		spawnRandomRange = Mathf.Max(spawnRandomRange, 0);
	}

	public void ClenseEnvironment()
	{
		bool anyDeleted;
		do
		{
			anyDeleted = false;
			foreach (Transform child in transform)
			{
				if (child != spritePrefab.transform)
				{
#if UNITY_EDITOR
					if (UnityEditor.EditorApplication.isPlaying)
						Destroy(child.gameObject);
					else
						DestroyImmediate(child.gameObject);
#else
					Destroy(child.gameObject);
#endif
					anyDeleted = true;
				}
			}
		} while (anyDeleted);
	}

	public void GenerateEnvironment()
	{
		if (spritePrefab == null)
		{
			Debug.LogError("Missing sprite prefab!");
			return;
		}
		
		float halfRandomRange = 0.5f * spawnRandomRange;

		for (int index = 0; index < layers.Length; index++)
		{
			Layer layer = layers[index];
			if (!layer.atlas) continue;

			// Fetch sprites
			var sprites = new Sprite[layer.atlas.spriteCount];
			layer.atlas.GetSprites(sprites);

			// Calculate position
			float radius = Mathf.Lerp(spawnInnerBoundary, spawnOuterBoundary, index == 0 ? 0 : (float)index / (layers.Length-1));

			float angleMultiplier = 360f / layer.count;
			for (int count = 0; count < layer.count; count++)
			{
				// Calculate position
				float minAngle = angleMultiplier * count;
				float maxAngle = angleMultiplier * (count + 1);
				float angle = UnityEngine.Random.Range(minAngle, maxAngle);
				float randomness = UnityEngine.Random.Range(-halfRandomRange, halfRandomRange);

				Vector3 position = RingObject.RingPosition(angle, radius + randomness);
				Quaternion rotation = RingObject.RingRotation(position);
				
				int spriteIndex = UnityEngine.Random.Range(0, sprites.Length);
				Sprite sprite = sprites[spriteIndex];
				string name = spritePrefab.name + " - " + sprites[spriteIndex].name;
				if (name.EndsWith("(Clone)")) name = name.Substring(0, name.Length - 7);

				// Clone prefab
				GameObject clone = Instantiate(spritePrefab, position, rotation);
				clone.transform.SetParent(transform, true);
				clone.SetActive(true);
				clone.gameObject.name = name;

				// Apply sprite
				var spriteRenderer = clone.GetComponent<SpriteRenderer>();
				spriteRenderer.sprite = sprite;
			}
		}
	}

#if UNITY_EDITOR
	private static void DrawGizmosRing(float radius, float width, int numOfSegments)
	{
		float halfWidth = width * 0.5f;
		const float y = 0.05f;
		Vector3 lastPos = RingObject.RingPositionY(0, radius + halfWidth, y);
		float theta = 360f / numOfSegments;

		for (float angle = theta; angle < 360; angle += theta)
		{
			Vector3 nextPos = RingObject.RingPositionY(angle, radius + halfWidth, y);
			Gizmos.DrawLine(lastPos, nextPos);

			if (Math.Abs(width) > 0.01f)
			{
				Vector3 innerLastPos = RingObject.RingPositionY(lastPos, radius - halfWidth, y);
				Vector3 innerNextPos = RingObject.RingPositionY(angle, radius - halfWidth, y);
				Gizmos.DrawLine(innerLastPos, innerNextPos);
				Gizmos.DrawLine(innerLastPos, lastPos);
			}
			lastPos = nextPos;
		}
		Gizmos.DrawLine(lastPos, RingObject.RingPosition(0, radius + halfWidth));

		if (Math.Abs(width) > 0.01f)
		{
			Vector3 innerLastPos = RingObject.RingPositionY(lastPos, radius - halfWidth, y);
			Vector3 innerNextPos = RingObject.RingPositionY(0, radius - halfWidth, y);
			Gizmos.DrawLine(innerLastPos, innerNextPos);
			Gizmos.DrawLine(innerLastPos, lastPos);
		}
	}

	private static void DrawGizmosRing(float radius)
	{
		DrawGizmosRing(radius, 0, Mathf.Max(Mathf.FloorToInt(radius * 0.15f), 6));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		DrawGizmosRing(spawnInnerBoundary-0.05f, 0, layers.Min(l => l.count));
		DrawGizmosRing(spawnOuterBoundary+0.05f, 0, layers.Max(l => l.count));
		for (int index = 0; index < layers.Length; index++)
		{
			Layer layer = layers[index];
			if (!layer.atlas) continue;

			float radius = Mathf.Lerp(spawnInnerBoundary, spawnOuterBoundary, index == 0 ? 0 : (float)index / (layers.Length-1));
			Gizmos.color = Color.blue;
			DrawGizmosRing(radius, 0, layer.count);
			Gizmos.color = new Color(1,0,1,0.5f);
			DrawGizmosRing(radius, spawnRandomRange, layer.count);
		}
	}
#endif

	[Serializable]
	public struct Layer
	{
		public SpriteAtlas atlas;
		public int count;
	}
}
