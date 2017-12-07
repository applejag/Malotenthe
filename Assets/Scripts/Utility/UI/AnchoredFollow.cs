using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace GameGUI
{
	public class AnchoredFollow : MonoBehaviour
	{
		public Transform worldObject;

		public bool followX = true;
		public bool followY = true;

		private Vector2 FollowAnchor(Vector2 pos, Vector2 anchor)
		{
			if (followX) anchor.x = pos.x;
			if (followY) anchor.y = pos.y;
			return anchor;
		}

		public void JumpToTarget()
		{
			var rectTransform = transform as RectTransform;
			Debug.Assert(rectTransform != null, "rectTransform != null");

			Vector2 pos = Camera.main.WorldToViewportPoint(worldObject.transform.position);

			rectTransform.anchorMin = FollowAnchor(pos, rectTransform.anchorMin);
			rectTransform.anchorMax = FollowAnchor(pos, rectTransform.anchorMax);
		}

		private void LateUpdate()
		{
			JumpToTarget();
		}
	}

}