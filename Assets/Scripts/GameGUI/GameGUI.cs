using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGUI
{
	public class GameGUI : MonoBehaviour
	{
		private static GameGUI singleton;
		
		public GameObject healthbarPrefab;
		
		private void Awake()
		{
			singleton = this;
		}

		public static Healthbar CreateHealthbar(RingWalker walker)
		{
			var clone = Instantiate(singleton.healthbarPrefab, singleton.transform);
			Healthbar healthbar = clone.GetComponent<Healthbar>();
			healthbar.walker = walker;
			healthbar.UpdatePercentageFromWalker();

			return healthbar;
		}

	}

}