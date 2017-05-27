using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalacticWanderer.Managers
{
	public enum GamePhase
	{
		None, // Default value, should not be used.
		MainMenu,
		OnPlanet,
		InsideShip
	}

	public class GameManager : MonoBehaviour
	{
		public static GameManager instance = null;

		public GamePhase currentPhase = GamePhase.None;

		public ShipManager shipManager;
		public WorldManager worldManager;

		private void Awake()
		{
			SingletonPattern();
		}

		public void PreLoadWorld()
		{
			SceneManager.LoadScene(Consts.ScenePaths.SCENE_WORLD, LoadSceneMode.Additive);
		}

		public void ButtonLaunchGameplay()
		{
			SetPhase(GamePhase.InsideShip);
		}

		public void SetPhase(GamePhase newPhase)
		{
			GamePhase oldPhase = currentPhase;
			currentPhase = newPhase;

			switch (newPhase)
			{
			case GamePhase.MainMenu:
				SetPhase_AnyToMenu();
				break;

			case GamePhase.OnPlanet:
				SetPhase_ShipToWorld();
				break;

			case GamePhase.InsideShip:
				if (oldPhase == GamePhase.MainMenu)
				{
					SetPhase_MenuToShip();
				}
				else
				{
					SetPhase_WorldToShip();
				}
				break;

			default:
				Debug.LogError("GameManager error : phase '" + newPhase + "' is not handled.");
				break;
			}
		}

		#region SetPhase methods

		private void SetPhase_AnyToMenu()
		{
			SceneManager.LoadScene(Consts.ScenePaths.SCENE_MAINMENU, LoadSceneMode.Single);
		}

		private void SetPhase_ShipToWorld()
		{
			if (worldManager)
			{
				shipManager.ActivePhase(false);
				worldManager.ActivePhase(true);
			}
			else
			{
				SceneManager.LoadScene(Consts.ScenePaths.SCENE_WORLD, LoadSceneMode.Additive);
				shipManager.ActivePhase(false);
			}
		}

		private void SetPhase_MenuToShip()
		{
			SceneManager.LoadScene(Consts.ScenePaths.SCENE_SHIP, LoadSceneMode.Single);
		}

		private void SetPhase_WorldToShip()
		{
			if (shipManager)
			{
				worldManager.ActivePhase(false);
				shipManager.ActivePhase(true);
			}
			else
			{
				SceneManager.LoadScene(Consts.ScenePaths.SCENE_SHIP, LoadSceneMode.Additive);
				worldManager.ActivePhase(false);
			}
		}

		#endregion SetPhase methods

		/// <summary>Assure there is always only one instance of GameManager.</summary>
		private void SingletonPattern()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Destroy(gameObject);
			}

			DontDestroyOnLoad(gameObject);
		}
	}
}