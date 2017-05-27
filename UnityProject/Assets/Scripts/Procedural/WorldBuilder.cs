using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    [SerializeField]
    private Texture2D atlasTexture;

    [SerializeField]
    private bool showDebug = false;

    Transform playerTransform;
    Vector3 lastDiscretePos;
    LandscapeMap landMap;
    List<Thread> threadPool;
    List<LandscapeWorker> workers;

    // TODO : we have to define a LODMax method !
    int lodMax = 3;
    float LODRadius = 100.0f;

    void Start()
    {
        // Find player by its tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Cannot find Player tag");
        }
        playerTransform = player.transform;

        // Dump atlas texture's pixels into pixel array
        if (atlasTexture == null)
        {
            Debug.LogError("Atlas texture shouldn't be null");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#else
                Application.Quit();
#endif
        }

        // Instantiates thread pool and workers
        workers = new List<LandscapeWorker>();
        threadPool = new List<Thread>();
        for (int i = 0; i < LandscapeConstants.THREAD_POOL_SIZE; i++)
        {
            LandscapeWorker worker = new LandscapeWorker();
            workers.Add(worker);
            threadPool.Add(new Thread(worker.ProcessLoop));
            threadPool[i].Start();
        }

        // Initialize last discrete position with current postion
        lastDiscretePos = WorldToDiscretePosition(playerTransform.position);

        // Generate first batch of landscapes
        landMap = new LandscapeMap(gameObject, atlasTexture);
    }

    void Update()
    {
        UpdateMap();
        UpdateLODs();
        LandscapeTrash.Flush();
    }

    Vector3 WorldToDiscretePosition(Vector3 v)
    {
        return new Vector3(
            Mathf.Floor((playerTransform.position.x + LandscapeConstants.LANDSCAPE_SIZE / 2) / LandscapeConstants.LANDSCAPE_SIZE),
            Mathf.Floor((playerTransform.position.y + LandscapeConstants.LANDSCAPE_SIZE / 2) / LandscapeConstants.DISCRETE_Y_UNIT), // unused
            Mathf.Floor((playerTransform.position.z + LandscapeConstants.LANDSCAPE_SIZE / 2) / LandscapeConstants.LANDSCAPE_SIZE)
        );
    }

    void UpdateMap()
    {
        Vector3 discretePos = WorldToDiscretePosition(playerTransform.position);
        Vector3 delta = discretePos - lastDiscretePos;

        // Handle changes on the XZ plane
        if (delta.x > 0)
        {
            for (int i = 0; i < delta.x; i++)
            {
                landMap.Shift(LandscapeMap.Direction.RIGHT);
            }
        }
        else if (delta.x < 0)
        {
            for (int i = 0; i < -delta.x; i++)
            {
                landMap.Shift(LandscapeMap.Direction.LEFT);
            }
        }

        if (delta.z < 0)
        {
            for (int i = 0; i < -delta.z; i++)
            {
                landMap.Shift(LandscapeMap.Direction.BACK);
            }
        }
        else if (delta.z > 0)
        {
            for (int i = 0; i < delta.z; i++)
            {
                landMap.Shift(LandscapeMap.Direction.FRONT);
            }
        }

        // Handle changes in height
        int deltaY = FindMapSize() - landMap.size;
        if (deltaY > 1)
        {
            landMap.Grow();
        }
        else if (deltaY < -1)
        {
            landMap.Shrink();
        }

        lastDiscretePos = discretePos;
    }

    float GetAngleFromPlayerForward(Vector3 pos)
    {
        return Vector2.Angle(
            new Vector2(playerTransform.position.x, playerTransform.position.z),
            new Vector2(pos.x, pos.z));
    }

    void UpdateLODs()
    {
        int workerIndex = 0;
        foreach (LinkedList<Landscape> line in landMap.GetMap())
        {
            foreach (Landscape land in line)
            {
                float radius = Vector3.SqrMagnitude(land.transform.position - playerTransform.position);
                float angleFromPlayerForward = GetAngleFromPlayerForward(land.transform.position);
                int newLod = getLandLOD(radius);
                
                if (land.GetLandscapeData().currentLOD != newLod && !land.isDirty)
                {
                    if (!land.isGeneratingMesh)
                    {
                        land.GetLandscapeData().nextLOD = newLod;

                        if (!land.isInQueue)
                        {
                            land.isInQueue = true;
                            workers[workerIndex].PushLandscape(land, radius, angleFromPlayerForward);
                            workerIndex = (workerIndex + 1) % workers.Count;                            
                        }
                    }
                }
            }
        }
    }

    /// <summary>Returns the LOD of a landscape function of his radius. Linear </summary>
    int getLandLOD(float radius)
    {
        if (radius <= LODRadius * LODRadius)
        {
            return 0;
        }
        else
        {
            int newLod = Mathf.RoundToInt(radius / (LODRadius * LODRadius));
            if (newLod >= LandscapeConstants.LOD_MAX)
            {
                return LandscapeConstants.LOD_MAX;
            }
            else return newLod;
        }
    }

    /// <summary>Returns the size that the map should have, depending on the player's height.</summary>
    int FindMapSize()
    {
        return Mathf.RoundToInt(Mathf.Lerp(LandscapeConstants.MIN_MAP_SIZE, LandscapeConstants.MAX_MAP_SIZE, playerTransform.position.y / LandscapeConstants.MAX_FLIGHT_HEIGHT));
    }

    void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = new Color(0, 1, 1, 0.75f);

            foreach (LinkedList<Landscape> line in landMap.GetMap())
            {
                foreach (Landscape land in line)
                {
                    Gizmos.DrawCube(land.transform.position, new Vector3(LandscapeConstants.LANDSCAPE_SIZE * 0.8f, 1, LandscapeConstants.LANDSCAPE_SIZE * 0.8f));
                }
            }
        }
    }
}