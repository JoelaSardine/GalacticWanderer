using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public int size = 4;
    public GameObject landscapePrefab;
    public float chunkSize;

    [HideInInspector]
    public Landscape[] landscapeArray;
    private float trueChunkSize;

	void Start () {
		landscapeArray = new Landscape[size * size];


	    Landscape.InitTexture();
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

                    landscapeGo.transform.position = new Vector3(x * chunkSize, 0, y * chunkSize);

	                landscape.InitRenderer();
	                landscape.Generate();
	                landscape.InitCollider();
	            }
	        }
	    }
	}
}
