using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlayerHUD : MonoBehaviour {

	public PlayerController player;

    [Header("Position rects")]
	public RectTransform playerPosXY;
	public RectTransform playerPosX;
	public RectTransform playerPosY;

    [Header("Healthbar")]
    public Image healthbarInstant;
    public Image healthbarSlow;
    public float healthbarSpeed;

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

#if UNITY_EDITOR
	    if (!UnityEditor.EditorApplication.isPlaying) return;
#endif

        if (healthbarInstant)
	    {
	        float healthPercentage = player.health / (float) player.maxHealth;
            
	        healthbarInstant.fillAmount = healthPercentage;

	        if (healthbarSlow)
	        {
	            if (healthbarSpeed <= 0)
	                healthbarSlow.fillAmount = healthPercentage;
                else
	            {
                    if (healthPercentage > healthbarSlow.fillAmount)
                        healthbarSlow.fillAmount = healthPercentage;
                
                    healthbarSlow.fillAmount = Mathf.MoveTowards(healthbarSlow.fillAmount, healthPercentage,
                        healthbarSpeed * Time.deltaTime * 0.01f);
	            }
            }
        }
	}

}
