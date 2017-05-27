using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticWanderer.Utils
{
	public class HideOnStart : MonoBehaviour
	{
		void Awake()
		{
			gameObject.SetActive(false);
		}
	}
}