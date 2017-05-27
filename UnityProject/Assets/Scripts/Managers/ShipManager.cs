using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticWanderer.Managers
{
	public class ShipManager : MonoBehaviour
	{
		private GameManager gameManager { get { return GameManager.instance; } }

		[SerializeField]
		private GameObject ShipSceneRoot;

		[SerializeField]
		private bool enableWorldPreload = true;

		void Start()
		{
			gameManager.shipManager = this;

			if (gameManager.currentPhase != GamePhase.InsideShip)
			{
				ActivePhase(false);
			}

			if (enableWorldPreload)
			{
				gameManager.PreLoadWorld();
			}
		}

		public void ActivePhase(bool status)
		{
			ShipSceneRoot.SetActive(status);
		}

		public void PilotTheShip()
		{
			gameManager.SetPhase(GamePhase.OnPlanet);
		}
	}
}