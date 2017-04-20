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



	void Start () {

	    // Instantiate landscape
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
	            }
	        }
	    }

	    // Initialize all landscapes using threads !
	    Thread[] threadArray = new Thread[4];
	    Color[] pixels = atlasTexture.GetPixels();
	    int atlasHeight = atlasTexture.height;
	    int atlasWidth = atlasTexture.width;
	    for (int i = 0; i < threadArray.Length; i++)
	    {
             threadArray[i] = new Thread(() =>
             {
                 int landscapeToProcess = landscapeArray.Length / threadArray.Length;
                 int startOffset = i * landscapeArray.Length / threadArray.Length;

                 for (int j = 0; j < landscapeToProcess; j++)
                 {
                     Landscape landscape = landscapeArray[startOffset + j];

                     landscape.atlasWidth = atlasWidth;
                     landscape.atlasHeight = atlasHeight;
                     landscape.atlasTextureData = pixels;

                     landscape.Generate();
                     landscape.InitTexture();
                 }
             });
             threadArray[i].Start();
	    }

	    for (int i = 0; i < threadArray.Length; i++)
	    {
	        threadArray[i].Join();
	    }

	    // Once the data has been generated we can bind it to unity classes (mesh, texture, etc)
	    for (int i = 0; i < landscapeArray.Length; i++)
	    {
	        landscapeArray[i].BindDataToMesh();
	    }
	}

}
