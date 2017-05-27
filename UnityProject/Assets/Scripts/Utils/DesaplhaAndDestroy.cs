using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesaplhaAndDestroy : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(fadeOutAndDestroy());
	}

	private IEnumerator fadeOutAndDestroy()
	{
		CanvasGroup canvas = GetComponent<CanvasGroup>();
		canvas.alpha = 1;
		while (canvas.alpha > 0)
		{
			canvas.alpha -= 0.02f;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject);
	}
}
