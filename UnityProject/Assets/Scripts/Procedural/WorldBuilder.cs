using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public const int THREAD_COUNT = 2;
    public int size = 4;
    public GameObject landscapePrefab;
    public float chunkSize;
    public Texture2D atlasTexture;

    [HideInInspector]
    public Landscape[] landscapeArray;
    private float trueChunkSize;

    private LandscapeWorker[] workers;

    class LandscapeWorker
    {
        private int threadIndex;
        private int threadCount;
        private Vector2 atlasSize;
        private Color[] atlasData;
        private Landscape[] landscapeArray;
        public bool isDone = false;


        public LandscapeWorker(int index, int threadCount, Landscape[] landscapeArray, Vector2 atlasSize, Color[] atlasData)
        {
            threadIndex = index;
            this.landscapeArray = landscapeArray;
            this.atlasSize = atlasSize;
            this.atlasData = atlasData;
            this.threadCount = threadCount;
        }

        public void LandscapeGeneration()
        {
            int landscapeToProcess = landscapeArray.Length / threadCount;
            int startOffset = threadIndex * landscapeArray.Length / threadCount;
            Debug.Log("Thread " + threadIndex + " started ! (" + landscapeToProcess + " to process");


            for (int j = 0; j < landscapeToProcess; j++)
            {
                Debug.Log("Thread " + threadIndex + " working with landscape " + (startOffset + j));
                Landscape landscape = landscapeArray[startOffset + j];

                landscape.atlasWidth = (int)atlasSize.x;
                landscape.atlasHeight = (int)atlasSize.y;
                landscape.atlasTextureData = atlasData;

                landscape.Generate();

                Debug.Log("After generation : v = " + landscape.vertices.Length);

                landscape.InitTexture();
                Debug.Log("Thread " + threadIndex + " is done working with landscape " + (startOffset + j));
            }
            isDone = true;
        }

        public void FinalizeGeneration()
        {
            int landscapeToProcess = landscapeArray.Length / threadCount;
            int startOffset = threadIndex * landscapeArray.Length / threadCount;
            for (int i = 0; i < landscapeToProcess; i++)
            {
                landscapeArray[startOffset + i].BindDataToMesh();
            }
        }
    }



	void Start () {

	    // Instantiate landscapes and allocate the memory they need
	    landscapeArray = new Landscape[size * size];

	    for (int y = 0; y < size; y++)
	    {
	        for (int x = 0; x < size; x++)
	        {
	            GameObject landscapeGo = Instantiate(landscapePrefab);
	            Landscape landscape = landscapeGo.GetComponent<Landscape>();

	            if (landscape == null)
	            {
	                Debug.LogError("The instanciated gameobject should have a Landscape component !");
	            }
	            else
	            {
	                landscapeArray[y * size + x] = landscape;
	                landscape.size = chunkSize;
	                landscape.InitMeshFilterComponent();

                    landscapeGo.transform.position = new Vector3(x * chunkSize, 0, y * chunkSize);
	                landscape.SetPosition(landscapeGo.transform.position);
	                landscape.AllocateMemory();
	            }
	        }
	    }

        // Initialize all landscapes using threads !
        
	    Thread[] threadArray = new Thread[THREAD_COUNT];
        workers = new LandscapeWorker[THREAD_COUNT];
	    Color[] pixels = atlasTexture.GetPixels();
	    int atlasHeight = atlasTexture.height;
	    int atlasWidth = atlasTexture.width;
	    for (int i = 0; i < threadArray.Length; i++)
	    {
            workers[i] = new LandscapeWorker(i, threadArray.Length, landscapeArray, new Vector2(atlasWidth, atlasHeight), pixels);
            threadArray[i] = new Thread(workers[i].LandscapeGeneration);
            threadArray[i].Start();
	    }

        /*
	    for (int i = 0; i < threadArray.Length; i++)
	    {
	        threadArray[i].Join();
	    }

	    // Once the data has been generated we can bind it to unity classes (mesh, texture, etc)
	    for (int i = 0; i < landscapeArray.Length; i++)
	    {
	        landscapeArray[i].BindDataToMesh();
	    }
        */
	}

    void Update()
    {
        for (int i = 0; i < workers.Length; i++)
        {
            if (workers[i].isDone)
            {
                workers[i].FinalizeGeneration();
                workers[i].isDone = false;
            }
        }
    }

}
