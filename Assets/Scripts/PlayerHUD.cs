using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlayerHUD : MonoBehaviour {

	public PlayerController player;

	public RectTransform playerPosXY;
	public RectTransform playerPosX;
	public RectTransform playerPosY;

	private void Update()
	{
		if (!player) return;

		Vector2 pos = Camera.main.WorldToViewportPoint(player.transform.position);

		if (playerPosX) {
			playerPosX.anchorMin = playerPosX.anchorMin.SetX(pos.x);
			playerPosX.anchorMax = playerPosX.anchorMax.SetX(pos.x);
		}

		if (playerPosY) {
			playerPosY.anchorMin = playerPosY.anchorMin.SetY(pos.y);
			playerPosY.anchorMax = playerPosY.anchorMax.SetY(pos.y);
		}

		if (playerPosXY) {
			playerPosXY.anchorMin = pos;
			playerPosXY.anchorMax = pos;
		}
	}

}
