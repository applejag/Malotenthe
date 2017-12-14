using UnityEngine;

namespace ExtensionMethods
{
	public static class GameObjectExtensions
	{
		public static void SetLayerRecursive(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;

			foreach (Transform child in gameObject.transform)
			{
				SetLayerRecursive(child.gameObject, layer);
			}
		}
	}
}