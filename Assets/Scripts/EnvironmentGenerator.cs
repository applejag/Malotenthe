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
				if (!child.gameObject.activeSelf) continue;

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
			var atlas = layer.atlas as SpriteAtlas;
			Sprite[] sprites = atlas != null ? new Sprite[atlas.spriteCount] : null;
			if (atlas != null) atlas.GetSprites(sprites);

			// Calculate position
			float radius = Mathf.Lerp(spawnInnerBoundary, spawnOuterBoundary, index == 0 ? 0 : (float)index / (layers.Length-1)) + layer.rangeOffset;
			float halfRandomRange = 0.5f * spawnRandomRange + layer.rangeOffset;

			var container = new GameObject(layer.atlas.name);
			container.transform.SetParent(transform, false);

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

				if (atlas != null)
				{
					int spriteIndex = UnityEngine.Random.Range(0, sprites.Length);
					string cloneName = spritePrefab.name + " - " + sprites[spriteIndex].name;
					Sprite sprite = sprites[spriteIndex];
					if (cloneName.EndsWith("(Clone)"))
						cloneName = cloneName.Substring(0, cloneName.Length - 7);

					// Clone prefab
					GameObject clone = InstantiatePrefab(spritePrefab, position, rotation);
					clone.transform.SetParent(container.transform, true);
					clone.SetActive(true);
					clone.gameObject.name = cloneName;

					// Apply sprite
					var spriteRenderer = clone.GetComponent<SpriteRenderer>();
					spriteRenderer.sprite = sprite;
				}
				else if (layer.atlas is GameObject)
				{

					// Clone prefab
					var go = (GameObject) layer.atlas;
					GameObject clone = InstantiatePrefab(go, position, rotation);
					clone.transform.SetParent(container.transform, true);
					clone.SetActive(true);
				}
				else
				{
					// ???
					throw new ArgumentOutOfRangeException("What is this, a joke? Why you giving me an object of this type??", layer.atlas, "layer[" + index + "].atlas");
				}
			}
		}
	}

	private static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation)
	{

#if UNITY_EDITOR
		if (UnityEditor.PrefabUtility.GetPrefabType(prefab) == UnityEditor.PrefabType.Prefab
		    || UnityEditor.PrefabUtility.GetPrefabType(prefab) == UnityEditor.PrefabType.ModelPrefab)
		{
			var clone = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
			clone.transform.position = position;
			clone.transform.rotation = rotation;
			return clone;
		}
		else
#endif
			return Instantiate(prefab, position, rotation);
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
		for (var index = 0; index < layers.Length; index++)
		{
			Layer layer = layers[index];
			if (!layer.atlas) continue;

			float radius = Mathf.Lerp(spawnInnerBoundary, spawnOuterBoundary, index == 0 ? 0 : (float)index / (layers.Length-1)) + layer.positionOffset;
			Gizmos.color = Color.blue;
			DrawGizmosRing(radius, 0, layer.count);
			Gizmos.color = new Color(1,0,1,0.5f);
			DrawGizmosRing(radius, spawnRandomRange + layer.rangeOffset, layer.count);
		}
	}
#endif

	[Serializable]
	public struct Layer
	{
		public UnityEngine.Object atlas;
		public int count;
		public float positionOffset;
		public float rangeOffset;
	}
}
