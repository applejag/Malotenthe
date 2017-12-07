using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
	public class GameGUI : MonoBehaviour
	{
		private static GameGUI singleton;
		
		public GameObject healthbarPrefab;
		public GameObject damagePopupPrefab;
		
		private void Awake()
		{
			singleton = this;
		}

		public static Healthbar CreateHealthbar(RingWalker walker)
		{
			var clone = Instantiate(singleton.healthbarPrefab, singleton.transform);

			var healthbar = clone.GetComponent<Healthbar>();
			healthbar.walker = walker;

			var anchor = clone.GetComponent<AnchoredFollow>();
			anchor.worldObject = walker.transform;

			return healthbar;
		}

		public static DamagePopup CreateDamagePopup(Vector3 position, int damage)
		{
			var clone = Instantiate(singleton.damagePopupPrefab, singleton.transform);

			var anchor = clone.GetComponent<AnchoredFollow>();
			anchor.worldPos = position;

			var popup = clone.GetComponent<DamagePopup>();
			popup.damage = damage;

			return popup;
		}

	}

}