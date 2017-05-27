using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticWanderer
{
	public class GameManTest : MonoBehaviour
	{
		public GameObject gm;

		void Start()
		{
			gm = GameObject.FindGameObjectWithTag("GameManager");
		}
	}
}