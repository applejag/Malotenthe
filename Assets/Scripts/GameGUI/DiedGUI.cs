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
		public float fadeInTime = 2;

		[SerializeField, HideInInspector]
		private Coroutine fadeInRoutine;

		private void OnEnable()
		{
			singleton = this;
		}

		private IEnumerator FadeInRoutine(float delay)
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

			fadeInRoutine = null;
		}

		public static void FadeInGameOverScreen(float delay = 0)
		{
			if (singleton.fadeInRoutine != null)
				singleton.StopCoroutine(singleton.fadeInRoutine);

			singleton.fadeInRoutine = singleton.StartCoroutine(singleton.FadeInRoutine(delay));
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
