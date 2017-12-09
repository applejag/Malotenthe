using System;
using System.Collections;
using System.Collections.Generic;
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
		spawnRandomRange = Mathf.Max(spawnRandomRange, 0);
	}

	private void OnValidate()
	{
		spawnInnerBoundary = Mathf.Clamp(spawnInnerBoundary, 0, spawnOuterBoundary);
		spawnOuterBoundary = Mathf.Max(spawnOuterBoundary, spawnInnerBoundary);
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

		for (int index = 0; index < layers.Length; index++)
		{
			Layer layer = layers[index];
			if (!layer.atlas) continue;

			// Fetch sprites
			var sprites = new Sprite[layer.atlas.spriteCount];
			layer.atlas.GetSprites(sprites);

			// Calculate position
			float radius = Mathf.Lerp(spawnInnerBoundary, spawnOuterBoundary, index == 0 ? 0 : (float)index / layers.Length);

			float angleMultiplier = 360f / layer.count;
			for (int count = 0; count < layer.count; count++)
			{
				// Calculate position
				float minAngle = angleMultiplier * count;
				float maxAngle = angleMultiplier * (count + 1);
				float angle = UnityEngine.Random.Range(minAngle, maxAngle);
				float radiusRandomness = UnityEngine.Random.Range(-0.5f, 0.5f) * spawnRandomRange;

				Vector3 position = RingObject.RingPosition(angle, radius + radiusRandomness);
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
	private void DrawGizmosRing(float radius, float theta = 10)
	{
		Vector3 lastPos = RingObject.RingPosition(0, radius);
		for (float angle = theta; angle < 360; angle += theta)
		{
			Vector3 nextPos = RingObject.RingPosition(angle, radius);
			Gizmos.DrawLine(lastPos, nextPos);
			lastPos = nextPos;
		}
		Gizmos.DrawLine(lastPos, RingObject.RingPosition(0, radius));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		DrawGizmosRing(spawnInnerBoundary);
		DrawGizmosRing(spawnOuterBoundary);
		for (int index = 0; index < layers.Length; index++)
		{
			Layer layer = layers[index];
			if (!layer.atlas) continue;

			float radius = Mathf.Lerp(spawnInnerBoundary, spawnOuterBoundary, index == 0 ? 0 : (float)index / (layers.Length-1));
			Gizmos.color = Color.blue;
			DrawGizmosRing(radius);
			Gizmos.color = new Color(1,0,1,0.5f);
			DrawGizmosRing(radius - spawnRandomRange * 0.5f);
			DrawGizmosRing(radius + spawnRandomRange * 0.5f);

			Gizmos.color = Color.yellow;
			for (int count = 0; count < layer.count; count++)
			{
				Vector3 pos = RingObject.RingPosition(360f * count / layer.count, radius);
				Gizmos.DrawRay(pos, Vector3.up);
				Gizmos.DrawSphere(pos, 0.5f);
			}
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
