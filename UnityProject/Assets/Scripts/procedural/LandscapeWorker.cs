using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Priority_Queue;
using UnityEngine;

/// <summary>
/// Class to use through threads.
/// The job of this class is to call landscape generation methods.
/// </summary>
public class LandscapeWorker
{

    private static int workerCounts = 0;

    private int id;

    /// <summary>
    /// Time to sleep in ms when there are no landscapes in the pipes
    /// </summary>
    const int THREAD_SLEEP_TIME = 500;

    /// <summary>
    /// Mutex object
    /// </summary>
    readonly object syncLock = new object();

    /// <summary>
    /// Landscape input pipeline
    /// </summary>
    FastPriorityQueue<Landscape> inputs = new FastPriorityQueue<Landscape>(400);

    /// <summary>
    /// Simple switch to turn off the worker and close its calling thread
    /// </summary>
    bool isOn = true;

    public LandscapeWorker()
    {
        id = workerCounts++;
    }

    /// <summary>
    /// Add a landscape to the worker
    /// </summary>
    /// <param name="land"></param>
    public void PushLandscape(Landscape land, float sqrDistanceFromPlayer, float angleFromPlayerForward)
    {
        float priority = GetPriorityFromDistanceAndAngle(sqrDistanceFromPlayer,angleFromPlayerForward);
        lock (syncLock)
        {
            inputs.Enqueue(land, priority);
        }
    }

    public void UpdateLandscapePriorityAndLOD(Landscape land, int lod, float sqrDistanceFromPlayer, float angleFromPlayerForward)
    {
        float priority = GetPriorityFromDistanceAndAngle(sqrDistanceFromPlayer,angleFromPlayerForward);
        lock (syncLock)
        {
            land.GetLandscapeData().nextLOD = lod;
            inputs.UpdatePriority(land, priority);
        }        
    }

    float GetPriorityFromDistanceAndAngle(float sqrDistanceFromPlayer, float angleFromPlayerForward)
    {
        return sqrDistanceFromPlayer * 0.1f + angleFromPlayerForward * angleFromPlayerForward;
    }

    /// <summary>
    /// Check if there are landscape to process
    /// </summary>
    /// <returns></returns>
    bool QueueIsEmpty()
    {
        bool val;
        lock (syncLock)
        {
            val = inputs.Count == 0;
        }

        return val;
    }

    /// <summary>
    /// Remove a landscape from the pipe
    /// </summary>
    /// <returns></returns>
    Landscape PopLandscape()
    {
        int debugCount;
        Landscape l = null;
        lock (syncLock)
        {
            l = inputs.Dequeue();
            debugCount = inputs.Count;
        }

        //Debug.Log("Worker " + id + " pop : " + l.cachedName + " ( size : " + debugCount + ")");

        return l;
    }

    /// <summary>
    /// Threaded job
    /// </summary>
    public void ProcessLoop()
    {
        Debug.Log("Worker " + id + " started ! ");
        while (isOn)
        {
            if (QueueIsEmpty())
            {
                Thread.Sleep(THREAD_SLEEP_TIME);
            }
            else
            {
                Landscape land = PopLandscape();
                if (land != null)
                {
                    land.isGeneratingMesh = true;
                    land.GetLandscapeData().GenerateMesh(land.GetLandscapeData().nextLOD);

                    if (!land.initialized)
                    {
                        land.GetLandscapeData().GenerateTexture();
                    }

                    land.isDirty = true;
                    land.isInQueue = false;
                    land.isGeneratingMesh = false;
                }
                else
                {
                    Debug.LogError("Worker " + id + " : Landscape shouldn't be null !");
                }
            }
        }
        Debug.Log("Worker " + id + " stopped ! ");
    }

}
