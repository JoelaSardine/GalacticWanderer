using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticWanderer.Managers
{
	public class MenuManager : MonoBehaviour
	{
		private GameManager gameManager { get { return GameManager.instance; } }

		[SerializeField]
		private Animator uiAnimator;

		[SerializeField]
		private AnimationClip closeMenuAnimation;

		private void Start()
		{
			gameManager.menuManager = this;
		}

		private void Update()
		{
			if (Input.anyKeyDown)
			{
				uiAnimator.SetTrigger("AnyKeyPressed");
			}
		}

		public void Button_StartGame()
		{
			StartCoroutine(StartGameCoroutine());
		}

		private IEnumerator StartGameCoroutine()
		{
			uiAnimator.SetTrigger("CloseMenu");
			yield return new WaitForSeconds(closeMenuAnimation.length);
			gameManager.SetPhase(GamePhase.InsideShip);
		}

		public void Button_QuitGame()
		{
			StartCoroutine(QuitCoroutine());
		}

		private IEnumerator QuitCoroutine()
		{
			uiAnimator.SetTrigger("CloseMenu");
			yield return new WaitForSeconds(closeMenuAnimation.length);
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}
	}
}