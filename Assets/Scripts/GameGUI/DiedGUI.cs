using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameGUI
{
	public class DiedGUI : MonoBehaviour
	{
		private static DiedGUI singleton;

		public CanvasGroup group;
		public float delay = 2;
		public float fadeInTime = 2;

		public SlowIntegerCounter counterLivedTime;
		public SlowIntegerCounter counterKills;
		public SlowIntegerCounter counterDamageDealt;
		public SlowIntegerCounter counterDamageTaken;
		public SlowIntegerCounter counterScore;

		[SerializeField, HideInInspector]
		private Coroutine fadeInRoutine;

		private void OnEnable()
		{
			singleton = this;
		}

		private IEnumerator FadeInRoutine(WalkerStatistics stats)
		{
			yield return new WaitForSeconds(delay);

			group.blocksRaycasts = true;
			group.interactable = false;
			group.alpha = 0;

			if (fadeInTime > 0)
			{
				float start = Time.time;
				float t;
				while ((t = (Time.time - start)/fadeInTime) < 1f)
				{
					group.alpha = t;
					yield return null;
				}
			}

			group.alpha = 1;
			group.interactable = true;
			yield return new WaitForSeconds(1.4f);

			{
				const float inbetween = 0.6f;
				const float counterTime = 1.5f;

				int timeAlive = Mathf.CeilToInt(stats.TimeAliveTotal);
				int numOfKills = stats.NumOfKills;
				int damageDealt = stats.DamageDealt;
				int damageTaken = stats.DamageTaken;
				int score = Mathf.RoundToInt(50f * (damageDealt + numOfKills * numOfKills) / Mathf.Sqrt(damageTaken) + 24f * timeAlive / numOfKills);

				counterLivedTime.SetTargetValue(timeAlive, counterTime);
				yield return new WaitForSeconds(inbetween);
				counterKills.SetTargetValue(numOfKills, counterTime);
				yield return new WaitForSeconds(inbetween);
				counterDamageDealt.SetTargetValue(damageDealt, counterTime);
				yield return new WaitForSeconds(inbetween);
				counterDamageTaken.SetTargetValue(damageTaken, counterTime);

				yield return new WaitForSeconds(counterTime * 2);

				counterScore.SetTargetValue(score, counterTime * 3f);
			}

			fadeInRoutine = null;
		}

		public static void FadeInGameOverScreen(WalkerStatistics playerStatistics)
		{
			if (singleton.fadeInRoutine != null)
				singleton.StopCoroutine(singleton.fadeInRoutine);

			singleton.fadeInRoutine = singleton.StartCoroutine(singleton.FadeInRoutine(playerStatistics));
		}

		public void ActionRestart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void ActionExit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

	}
}
