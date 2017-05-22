using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Class to use through threads.
/// The job of this class is to call landscape generation methods.
/// </summary>
public class LandscapeWorker {

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
    Queue<Landscape> inputs = new Queue<Landscape>();

    /// <summary>
    /// Simple switch to turn off the worker and close its calling thread
    /// </summary>
    bool isOn = true;

    /// <summary>
    /// Add a landscape to the worker
    /// </summary>
    /// <param name="land"></param>
    public void PushLandscape(Landscape land)
    {
        Debug.Log("Push : " + land);
        lock (syncLock)
        {
            inputs.Enqueue(land);
        }
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
        Landscape l = null;
        lock (syncLock)
        {
            l = inputs.Dequeue();
            l.isInQueue = false;
        }

        return l;
    }

    /// <summary>
    /// Threaded job
    /// </summary>
    public void ProcessLoop()
    {
        Debug.Log(GetHashCode() + " worker started ! ");
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
                    land.GetLandscapeData().GenerateMesh(land.nextLOD);
                    land.isDirty = true;
                }
                else
                {
                    Debug.LogError("Landscape shouldn't be null !");
                }
            }
        }
        Debug.Log(GetHashCode() + " worker stopped ! ");
    }

}
