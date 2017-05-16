using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public int size = 4;
    public GameObject landscapePrefab;
    public float chunkSize;
    public Texture2D atlasTexture;

    [HideInInspector]
    public Landscape[] landscapeArray;
    private float trueChunkSize;


    class LandscapeWorker
    {
        private int threadIndex;
        private int threadCount;
        private Vector2 atlasSize;
        private Color[] atlasData;
        private Landscape[] landscapeArray;

        public Logger logger = new Logger();

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
            logger.Log("Thread " + threadIndex + " started ! (" + landscapeToProcess + " to process");


            for (int j = 0; j < landscapeToProcess; j++)
            {
                logger.Log("Thread " + threadIndex + " working with landscape " + (startOffset + j));
                Landscape landscape = landscapeArray[startOffset + j];

                landscape.atlasWidth = (int)atlasSize.x;
                landscape.atlasHeight = (int)atlasSize.y;
                landscape.atlasTextureData = atlasData;

                landscape.Generate();
                logger.ConcatAndFlush(landscape.logger);

                logger.Log("After generation : v = " + landscape.vertices.Length);

                landscape.InitTexture();
                logger.Log("Thread " + threadIndex + " is done working with landscape " + (startOffset + j));
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
	    Thread[] threadArray = new Thread[4];
        LandscapeWorker[] workers = new LandscapeWorker[4];
	    Color[] pixels = atlasTexture.GetPixels();
	    int atlasHeight = atlasTexture.height;
	    int atlasWidth = atlasTexture.width;
	    for (int i = 0; i < threadArray.Length; i++)
	    {
            workers[i] = new LandscapeWorker(i, threadArray.Length, landscapeArray, new Vector2(atlasWidth, atlasHeight), pixels);
            threadArray[i] = new Thread(workers[i].LandscapeGeneration);
            threadArray[i].Start();
	    }

	    for (int i = 0; i < threadArray.Length; i++)
	    {
	        threadArray[i].Join();
            workers[i].logger.Display();
	    }

	    // Once the data has been generated we can bind it to unity classes (mesh, texture, etc)
	    for (int i = 0; i < landscapeArray.Length; i++)
	    {
	        landscapeArray[i].BindDataToMesh();
	    }
	}

}
