using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Our own garbage collector for Landscape instances
/// 
/// We want to be sure that we destroy instances that are not in a Landscape Worker
/// </summary>
public class LandscapeTrash {

    static private Queue<Landscape> landscapeToDestroy = new Queue<Landscape>();
	
    /// <summary>
    /// Mark a landscape to be destroyed
    /// </summary>
    /// <param name="land"></param>
    public static void Destroy(Landscape land)
    {
        land.gameObject.SetActive(false);
        landscapeToDestroy.Enqueue(land);
    }

    /// <summary>
    /// Destroys every landscapes that are not in a worker
    /// </summary>
	public static void Flush() {

        Queue<Landscape> toDestroyLater = new Queue<Landscape>();
        while (landscapeToDestroy.Count != 0)
        {
            Landscape l = landscapeToDestroy.Dequeue();
            if (l.isInQueue)
            {
                toDestroyLater.Enqueue(l);
            }
            else
            {
                GameObject.Destroy(l.gameObject);
            }
        }

        landscapeToDestroy = toDestroyLater;
	}
}
