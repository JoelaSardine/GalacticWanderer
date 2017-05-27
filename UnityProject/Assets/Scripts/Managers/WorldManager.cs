using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticWanderer.Managers
{
	public class WorldManager : MonoBehaviour
	{
		private GameManager gameManager { get { return GameManager.instance; } }

		[SerializeField]
		private GameObject playerShip;

		private void Start()
		{
			gameManager.worldManager = this;

			if (gameManager.currentPhase != GamePhase.OnPlanet)
			{
				ActivePhase(false);
			}
		}

		public void EnterTheShip()
		{
			gameManager.SetPhase(GamePhase.InsideShip);
		}

		public void ActivePhase(bool status)
		{
			if (playerShip)
			{
				playerShip.SetActive(status);
			}
		}
	}
}